// SPDX-License-Identifier: BSD-2-Clause

using System;
using System.Net.Http;
using System.Threading;
using EnsembleWeather;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>Registers <see cref="EnsembleWeatherClient"/> with a service collection.</summary>
public static class EnsembleWeatherServiceCollectionExtensions
{
    /// <summary>The name of the <see cref="HttpClient"/> the client uses, for further configuration.</summary>
    public const string HttpClientName = "EnsembleWeather";

    /// <summary>
    /// Adds <see cref="EnsembleWeatherClient"/> as a typed <see cref="HttpClient"/> client, configured through
    /// <see cref="IOptions{TOptions}"/> of <see cref="EnsembleWeatherOptions"/>.
    /// </summary>
    /// <remarks>
    /// The primary handler does not follow redirects (the SDK follows them itself and drops the API key on a
    /// redirect to another host) and the <see cref="HttpClient.Timeout"/> is infinite (the SDK times each attempt).
    /// One <see cref="IResponseCache"/> is shared by every client the container creates, so the <c>ETag</c> cache
    /// survives the factory's transient clients; register your own <see cref="IResponseCache"/> first to replace it.
    /// Deprecation notices go to <see cref="ILogger"/> as warnings, and the factory's request logging redacts the
    /// <c>X-API-Key</c> header.
    /// </remarks>
    /// <param name="services">The service collection.</param>
    /// <param name="configure">Configures the options (API key, units, ...); <see langword="null"/> for the defaults.</param>
    /// <returns>The <see cref="IHttpClientBuilder"/>, to add handlers or policies.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="services"/> is <see langword="null"/>.</exception>
    /// <example>
    /// <code>
    /// services.AddEnsembleWeather(o => o.ApiKey = configuration["Ourea:ApiKey"]);
    /// </code>
    /// </example>
    public static IHttpClientBuilder AddEnsembleWeather(
        this IServiceCollection services, Action<EnsembleWeatherOptions>? configure = null)
    {
        if (services is null)
        {
            throw new ArgumentNullException(nameof(services));
        }

        OptionsBuilder<EnsembleWeatherOptions> options = services.AddOptions<EnsembleWeatherOptions>();
        if (configure is not null)
        {
            options.Configure(configure);
        }

        services.TryAddSingleton<IResponseCache>(_ => new MemoryResponseCache());
        IHttpClientBuilder builder = services
            .AddHttpClient(HttpClientName)
            .ConfigureHttpClient(http => http.Timeout = Timeout.InfiniteTimeSpan)
            .ConfigurePrimaryHttpMessageHandler(CreateHandler)
            // IHttpClientFactory's trace logging prints request headers; the API key must never reach a log.
            .RedactLoggedHeaders(RedactedHeaders);
        builder.AddTypedClient<EnsembleWeatherClient>((http, provider) =>
            {
                EnsembleWeatherOptions configured = provider.GetRequiredService<IOptions<EnsembleWeatherOptions>>().Value;
                var effective = new EnsembleWeatherOptions
                {
                    ApiKey = configured.ApiKey,
                    BaseUrl = configured.BaseUrl,
                    Units = configured.Units,
                    Timeout = configured.Timeout,
                    MaxRetries = configured.MaxRetries,
                    EnableCache = configured.EnableCache,
                    Cache = configured.Cache ?? provider.GetRequiredService<IResponseCache>(),
                    SendApiKeyOnOpenRoutes = configured.SendApiKeyOnOpenRoutes,
                    ApplicationId = configured.ApplicationId,
                    TimeProvider = configured.TimeProvider ?? provider.GetService<TimeProvider>(),
                    OnDeprecation = configured.OnDeprecation ?? LogDeprecation(provider.GetService<ILoggerFactory>()),
                };
                return new EnsembleWeatherClient(http, effective);
            });
        return builder;
    }

    private static readonly string[] RedactedHeaders = ["X-API-Key", "Authorization", "Cookie", "Set-Cookie"];

    private static HttpMessageHandler CreateHandler()
    {
#if NET5_0_OR_GREATER
        return new SocketsHttpHandler
        {
            AllowAutoRedirect = false,
            AutomaticDecompression = System.Net.DecompressionMethods.All,
            SslOptions = { CertificateRevocationCheckMode = System.Security.Cryptography.X509Certificates.X509RevocationMode.Online },
        };
#else
        return new HttpClientHandler
        {
            AllowAutoRedirect = false,
            AutomaticDecompression = System.Net.DecompressionMethods.GZip | System.Net.DecompressionMethods.Deflate,
            CheckCertificateRevocationList = true,
        };
#endif
    }

    private static Action<DeprecationNotice>? LogDeprecation(ILoggerFactory? loggers)
    {
        if (loggers is null)
        {
            return null;
        }

        ILogger logger = loggers.CreateLogger(typeof(EnsembleWeatherClient).FullName!);
        return notice => Log.Deprecated(logger, notice.Operation, notice.Deprecation, notice.Sunset, notice.Link);
    }

    private static class Log
    {
        private static readonly Action<ILogger, string, string?, string?, string?, Exception?> DeprecatedMessage =
            LoggerMessage.Define<string, string?, string?, string?>(
                LogLevel.Warning,
                new EventId(1, "Deprecated"),
                "Ourea API deprecation on {Operation}: Deprecation={Deprecation}, Sunset={Sunset}; see {Link}");

        public static void Deprecated(ILogger logger, string operation, string? deprecation, string? sunset, string? link)
            => DeprecatedMessage(logger, operation, deprecation, sunset, link, null);
    }
}
