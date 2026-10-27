// SPDX-License-Identifier: BSD-2-Clause

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Authentication;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using EnsembleWeather.Serialization;

namespace EnsembleWeather;

/// <summary>
/// The transport: canonical URL, credentials per route, the ETag cache, same-host redirects, retries with backoff,
/// the per-attempt timeout, the response-size bound, error mapping, rate-limit and deprecation headers.
/// </summary>
internal sealed class ApiPipeline
{
    /// <summary>A response body over this is an error, never an out-of-memory.</summary>
    internal const long MaxResponseBytes = 64L * 1024 * 1024;

    private const int MaxRedirects = 5;
    private static readonly TimeSpan MaxRetryAfter = TimeSpan.FromSeconds(60);
    private static readonly TimeSpan MaxBackoff = TimeSpan.FromSeconds(8);

    private readonly HttpClient _http;
    private readonly string? _apiKey;
    private readonly string _keyHash;
    private readonly Uri _baseUrl;
    private readonly string _basePrefix;
    private readonly IResponseCache? _cache;
    private readonly TimeProvider _time;
    private readonly Func<double> _jitter;
    private readonly DeprecationRegistry _deprecations;
    private readonly Action<DeprecationNotice> _onDeprecation;
    private readonly int _maxRetries;
    private readonly TimeSpan _timeout;
    private readonly bool _sendKeyOnOpenRoutes;
    private readonly string _userAgent;

    public ApiPipeline(HttpClient http, EnsembleWeatherOptions options, Func<string, string?> environment)
    {
        _http = http;
        string? key = options.ApiKey ?? environment("OUREA_API_KEY");
        _apiKey = string.IsNullOrEmpty(key) ? null : key;
        _keyHash = _apiKey is null ? "-" : Sha256Hex(_apiKey);

        Uri? baseUrl = options.BaseUrl;
        string? envBase = environment("OUREA_BASE_URL");
        if (baseUrl is null && !string.IsNullOrEmpty(envBase))
        {
            if (!Uri.TryCreate(envBase, UriKind.Absolute, out baseUrl))
            {
                throw new ArgumentException("OUREA_BASE_URL is not an absolute URL.", nameof(options));
            }
        }

        _baseUrl = baseUrl ?? EnsembleWeatherOptions.DefaultBaseUrl;
        if (!_baseUrl.IsAbsoluteUri || (_baseUrl.Scheme != Uri.UriSchemeHttps && _baseUrl.Scheme != Uri.UriSchemeHttp))
        {
            throw new ArgumentException("The base URL must be an absolute http(s) URL.", nameof(options));
        }

        _basePrefix = _baseUrl.GetLeftPart(UriPartial.Path).TrimEnd('/');

        if (options.MaxRetries < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(options), "MaxRetries cannot be negative.");
        }

        if (options.Timeout <= TimeSpan.Zero && options.Timeout != Timeout.InfiniteTimeSpan)
        {
            throw new ArgumentOutOfRangeException(nameof(options), "Timeout must be positive.");
        }

        _maxRetries = options.MaxRetries;
        _timeout = options.Timeout;
        _cache = options.EnableCache ? options.Cache ?? new MemoryResponseCache() : null;
        _time = options.TimeProvider ?? TimeProvider.System;
        _jitter = options.Jitter ?? SecureJitter;
        _deprecations = options.Deprecations ?? DeprecationRegistry.Process;
        _onDeprecation = options.OnDeprecation ?? (n => Trace.TraceWarning(n.ToString()));
        _sendKeyOnOpenRoutes = options.SendApiKeyOnOpenRoutes;
        _userAgent = SdkInfo.UserAgent(options.ApplicationId);
    }

    /// <summary>The most recent rate-limit state the server reported.</summary>
    public RateLimitState? LastRateLimit { get; private set; }

    public Uri BaseUrl => _baseUrl;

    public async Task<T> SendAsync<T>(
        ApiRequest request, System.Text.Json.Serialization.Metadata.JsonTypeInfo<T> typeInfo, CancellationToken cancellationToken)
        where T : ApiResult
    {
        (byte[] body, ResponseMetadata metadata) = await SendRawAsync(request, cancellationToken).ConfigureAwait(false);
        T? result;
        try
        {
            result = JsonSerializer.Deserialize(body, typeInfo);
        }
        catch (JsonException ex)
        {
            throw new EnsembleWeatherException("The Ourea API sent a response body this SDK could not read.", ex);
        }

        if (result is null)
        {
            throw new EnsembleWeatherException("The Ourea API sent an empty response body.");
        }

        result.Metadata = metadata;
        return result;
    }

    internal async Task<(byte[] Body, ResponseMetadata Metadata)> SendRawAsync(ApiRequest request, CancellationToken cancellationToken)
    {
        string query = request.QueryString();
        var url = new Uri(_basePrefix + request.Path + (query.Length > 0 ? "?" + query : string.Empty));
        bool sendKey = _apiKey is not null && (!request.OpenRoute || _sendKeyOnOpenRoutes);
        string cacheKey = url.AbsoluteUri + "\n" + (sendKey ? _keyHash : "-");

        CachedResponse? cached = _cache is null
            ? null
            : await _cache.GetAsync(cacheKey, cancellationToken).ConfigureAwait(false);
        if (cached is not null && cached.IsFresh(_time.GetUtcNow()))
        {
            return (cached.Body, new ResponseMetadata(200, null, null, false, cached.EntityTag, null));
        }

        string? ifNoneMatch = cached?.EntityTag;
        for (int attempt = 0; ; attempt++)
        {
            bool canRetry = attempt < _maxRetries;
            Outcome outcome = await AttemptAsync(url, sendKey, ifNoneMatch, cancellationToken).ConfigureAwait(false);
            if (outcome.Failure is not null)
            {
                if (canRetry && outcome.Retryable)
                {
                    await DelayAsync(Backoff(attempt + 1), cancellationToken).ConfigureAwait(false);
                    continue;
                }

                throw outcome.Failure;
            }

            using HttpResponseMessage response = outcome.Response!;
            byte[] body = outcome.Body!;
            int status = (int)response.StatusCode;
            RateLimitState? rateLimit = ReadRateLimit(response);
            if (rateLimit is not null)
            {
                LastRateLimit = rateLimit;
            }

            string? requestId = Header(response, "X-Request-Id");
            DeprecationNotice? deprecation = ReadDeprecation(request.Operation, response);
            string? etag = response.Headers.ETag?.ToString() ?? Header(response, "ETag");
            CacheControlHeaderValue? cacheControl = response.Headers.CacheControl;

            if (status == 304 && cached is not null)
            {
                var refreshed = new CachedResponse(etag ?? cached.EntityTag, cached.Body, _time.GetUtcNow(), cacheControl?.MaxAge ?? cached.MaxAge);
                await _cache!.SetAsync(cacheKey, refreshed, cancellationToken).ConfigureAwait(false);
                return (cached.Body, new ResponseMetadata(304, rateLimit, requestId, true, refreshed.EntityTag, deprecation));
            }

            if (status >= 200 && status <= 299)
            {
                if (_cache is not null)
                {
                    bool storable = cacheControl?.NoStore != true && (etag is not null || cacheControl?.MaxAge > TimeSpan.Zero);
                    if (storable)
                    {
                        await _cache.SetAsync(cacheKey, new CachedResponse(etag, body, _time.GetUtcNow(), cacheControl?.MaxAge), cancellationToken).ConfigureAwait(false);
                    }
                    else if (cached is not null)
                    {
                        await _cache.RemoveAsync(cacheKey, cancellationToken).ConfigureAwait(false);
                    }
                }

                return (body, new ResponseMetadata(status, rateLimit, requestId, false, etag, deprecation));
            }

            ApiErrorDetails details = ReadError(status, body, response, rateLimit, requestId);
            bool retryableStatus = status is 429 or 502 or 503 or 504;
            bool honoursRetryAfter = status is 429 or 503 && details.RetryAfter is not null;
            if (retryableStatus && canRetry && (!honoursRetryAfter || details.RetryAfter <= MaxRetryAfter))
            {
                TimeSpan delay = honoursRetryAfter ? details.RetryAfter!.Value : Backoff(attempt + 1);
                await DelayAsync(delay, cancellationToken).ConfigureAwait(false);
                continue;
            }

            throw details.ToException();
        }
    }

    private async Task<Outcome> AttemptAsync(Uri url, bool sendKey, string? ifNoneMatch, CancellationToken cancellationToken)
    {
        using var attemptCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        if (_timeout != Timeout.InfiniteTimeSpan)
        {
            attemptCts.CancelAfter(_timeout);
        }

        Uri current = url;
        bool keyAllowed = sendKey;
        try
        {
            for (int hop = 0; ; hop++)
            {
                using HttpRequestMessage message = BuildMessage(current, keyAllowed, ifNoneMatch);
                HttpResponseMessage response = await _http
                    .SendAsync(message, HttpCompletionOption.ResponseHeadersRead, attemptCts.Token)
                    .ConfigureAwait(false);
                int status = (int)response.StatusCode;
                if (status is 301 or 302 or 307 or 308 && response.Headers.Location is Uri location)
                {
                    response.Dispose();
                    if (hop >= MaxRedirects)
                    {
                        return Outcome.Fail(new EnsembleWeatherException(
                            $"The Ourea API redirected more than {MaxRedirects} times."), retryable: false);
                    }

                    Uri next = location.IsAbsoluteUri ? location : new Uri(current, location);
                    if (!SameOrigin(next, _baseUrl))
                    {
                        keyAllowed = false;
                    }

                    if (status == 301)
                    {
                        Debug.WriteLine($"ensemble-weather: followed a 301 from {current.AbsolutePath}; an SDK-built URL should already be canonical.");
                    }

                    current = next;
                    continue;
                }

                byte[] body;
                try
                {
                    body = await ReadBoundedAsync(response.Content, attemptCts.Token).ConfigureAwait(false);
                }
                catch
                {
                    response.Dispose();
                    throw;
                }

                return Outcome.Ok(response, body);
            }
        }
        catch (OperationCanceledException ex) when (!cancellationToken.IsCancellationRequested)
        {
            return Outcome.Fail(new ServiceConnectionException("The request to the Ourea API timed out.", ex), retryable: true);
        }
        catch (HttpRequestException ex)
        {
            bool tls = IsTlsFailure(ex);
            string message = tls
                ? "The TLS connection to the Ourea API could not be verified."
                : "The Ourea API could not be reached.";
            return Outcome.Fail(new ServiceConnectionException(message, ex), retryable: !tls);
        }
        catch (IOException ex)
        {
            return Outcome.Fail(new ServiceConnectionException("The connection to the Ourea API failed.", ex), retryable: true);
        }
    }

    private HttpRequestMessage BuildMessage(Uri url, bool sendKey, string? ifNoneMatch)
    {
        var message = new HttpRequestMessage(HttpMethod.Get, url);
        message.Headers.TryAddWithoutValidation("User-Agent", _userAgent);
        message.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        if (sendKey)
        {
            message.Headers.TryAddWithoutValidation("X-API-Key", _apiKey);
        }

        if (ifNoneMatch is not null)
        {
            message.Headers.TryAddWithoutValidation("If-None-Match", ifNoneMatch);
        }

        return message;
    }

    private static async Task<byte[]> ReadBoundedAsync(HttpContent content, CancellationToken cancellationToken)
    {
        if (content.Headers.ContentLength > MaxResponseBytes)
        {
            throw new EnsembleWeatherException(
                $"The Ourea API response body is {content.Headers.ContentLength} bytes, over this SDK's {MaxResponseBytes}-byte bound.");
        }

#if NET5_0_OR_GREATER
        using Stream stream = await content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
#else
        using Stream stream = await content.ReadAsStreamAsync().ConfigureAwait(false);
#endif
        using var buffer = new MemoryStream();
        byte[] chunk = new byte[81920];
        while (true)
        {
#if NET5_0_OR_GREATER
            int read = await stream.ReadAsync(chunk.AsMemory(), cancellationToken).ConfigureAwait(false);
#else
            int read = await stream.ReadAsync(chunk, 0, chunk.Length, cancellationToken).ConfigureAwait(false);
#endif
            if (read == 0)
            {
                return buffer.ToArray();
            }

            if (buffer.Length + read > MaxResponseBytes)
            {
                throw new EnsembleWeatherException(
                    $"The Ourea API response body is over this SDK's {MaxResponseBytes}-byte bound.");
            }

            buffer.Write(chunk, 0, read);
        }
    }

    private ApiErrorDetails ReadError(int status, byte[] body, HttpResponseMessage response, RateLimitState? rateLimit, string? requestId)
    {
        string? code = null;
        string? message = null;
        try
        {
            ErrorBody? error = JsonSerializer.Deserialize(body, EnsembleWeatherJsonContext.Default.ErrorBody);
            code = error?.Error;
            message = error?.Message;
        }
        catch (JsonException)
        {
            // Not the API's JSON error (a proxy's text/plain 502, say): categorised by status, with no code.
        }

        return new ApiErrorDetails
        {
            StatusCode = status,
            ErrorCode = code,
            Message = Redact(message),
            RequestId = requestId,
            RetryAfter = ReadRetryAfter(response),
            RateLimit = rateLimit,
        };
    }

    /// <summary>Keeps the key out of any message even if a server or proxy echoed it.</summary>
    private string? Redact(string? text)
        => text is null || _apiKey is null ? text : text.Replace(_apiKey, "[redacted]");

    private TimeSpan? ReadRetryAfter(HttpResponseMessage response)
    {
        RetryConditionHeaderValue? retryAfter = response.Headers.RetryAfter;
        if (retryAfter?.Delta is TimeSpan delta)
        {
            return delta;
        }

        if (retryAfter?.Date is DateTimeOffset date)
        {
            TimeSpan wait = date - _time.GetUtcNow();
            return wait < TimeSpan.Zero ? TimeSpan.Zero : wait;
        }

        return null;
    }

    private static RateLimitState? ReadRateLimit(HttpResponseMessage response)
    {
        string? limit = Header(response, "RateLimit-Limit");
        string? remaining = Header(response, "RateLimit-Remaining");
        string? reset = Header(response, "RateLimit-Reset");
        string? policy = Header(response, "RateLimit-Policy");
        if (limit is null && remaining is null && reset is null && policy is null)
        {
            return null;
        }

        return new RateLimitState(
            ParseLong(limit),
            ParseLong(remaining),
            ParseLong(reset) is long seconds ? TimeSpan.FromSeconds(seconds) : null,
            policy);
    }

    private DeprecationNotice? ReadDeprecation(string operation, HttpResponseMessage response)
    {
        string? deprecation = Header(response, "Deprecation");
        string? sunset = Header(response, "Sunset");
        if (deprecation is null && sunset is null)
        {
            return null;
        }

        var notice = new DeprecationNotice(operation, deprecation, sunset, DeprecationLink(response));
        if (_deprecations.FirstTime(operation))
        {
            _onDeprecation(notice);
        }

        return notice;
    }

    private static string? DeprecationLink(HttpResponseMessage response)
    {
        if (!response.Headers.TryGetValues("Link", out IEnumerable<string>? values))
        {
            return null;
        }

        foreach (string value in values)
        {
            foreach (string part in value.Split(','))
            {
                int open = part.IndexOf('<');
                int close = part.IndexOf('>');
                if (open >= 0 && close > open
                    && part.Substring(close).Replace(" ", string.Empty).IndexOf("rel=\"deprecation\"", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    return part.Substring(open + 1, close - open - 1);
                }
            }
        }

        return null;
    }

    private static string? Header(HttpResponseMessage response, string name)
    {
        if (response.Headers.TryGetValues(name, out IEnumerable<string>? values)
            || response.Content.Headers.TryGetValues(name, out values))
        {
            return string.Join(", ", values);
        }

        return null;
    }

    private static long? ParseLong(string? text)
        => long.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out long v) ? v : null;

    /// <summary>Full jitter: a random delay in [0, min(8 s, 0.5 s × 2^(n-1))] before retry <paramref name="retry"/>.</summary>
    private TimeSpan Backoff(int retry)
    {
        double ceiling = Math.Min(MaxBackoff.TotalSeconds, 0.5 * Math.Pow(2, retry - 1));
        return TimeSpan.FromSeconds(ceiling * Math.Min(1, Math.Max(0, _jitter())));
    }

    private Task DelayAsync(TimeSpan delay, CancellationToken cancellationToken)
    {
        if (delay <= TimeSpan.Zero)
        {
            return Task.CompletedTask;
        }

#if NET8_0_OR_GREATER
        return Task.Delay(delay, _time, cancellationToken);
#else
        return _time.Delay(delay, cancellationToken);
#endif
    }

    private static bool SameOrigin(Uri a, Uri b)
        => string.Equals(a.Scheme, b.Scheme, StringComparison.OrdinalIgnoreCase)
            && string.Equals(a.IdnHost, b.IdnHost, StringComparison.OrdinalIgnoreCase)
            && a.Port == b.Port;

    private static bool IsTlsFailure(Exception ex)
    {
        for (Exception? e = ex; e is not null; e = e.InnerException)
        {
            if (e is AuthenticationException)
            {
                return true;
            }
        }

        return false;
    }

    private static double SecureJitter()
    {
        // Jitter is not a secret, but a CSPRNG keeps the security analyzers (CA5394) satisfied without a suppression.
        byte[] bytes = new byte[8];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(bytes);
        }

        return (BitConverter.ToUInt64(bytes, 0) >> 11) / (double)(1UL << 53);
    }

    private static string Sha256Hex(string value)
    {
        using var sha = SHA256.Create();
        byte[] hash = sha.ComputeHash(Encoding.UTF8.GetBytes(value));
        var builder = new StringBuilder(hash.Length * 2);
        foreach (byte b in hash)
        {
            builder.Append(b.ToString("x2", CultureInfo.InvariantCulture));
        }

        return builder.ToString();
    }

    private sealed class Outcome
    {
        public HttpResponseMessage? Response { get; private init; }

        public byte[]? Body { get; private init; }

        public EnsembleWeatherException? Failure { get; private init; }

        public bool Retryable { get; private init; }

        public static Outcome Ok(HttpResponseMessage response, byte[] body) => new() { Response = response, Body = body };

        public static Outcome Fail(EnsembleWeatherException failure, bool retryable) => new() { Failure = failure, Retryable = retryable };
    }
}
