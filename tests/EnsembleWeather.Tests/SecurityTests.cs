// SPDX-License-Identifier: BSD-2-Clause

using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using EnsembleWeather.Tests.Support;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace EnsembleWeather.Tests;

/// <summary>The SDK-side guarantees of SDK-STANDARD "Security".</summary>
public sealed class SecurityTests
{
    private const string Key = "ow_live_SECRET_must_never_leak";

    private static EnsembleWeatherClient Client(HttpMessageHandler handler, Action<EnsembleWeatherOptions>? configure = null)
    {
        var options = new EnsembleWeatherOptions
        {
            ApiKey = Key,
            TimeProvider = new RecordingTimeProvider(),
            Jitter = () => 0,
            Environment = _ => null,
            Deprecations = new DeprecationRegistry(),
        };
        configure?.Invoke(options);
        return new EnsembleWeatherClient(new HttpClient(handler), options);
    }

    public static TheoryData<int> ErrorStatuses() => new() { 400, 401, 403, 404, 429, 500, 503 };

    [Theory]
    [MemberData(nameof(ErrorStatuses))]
    public async Task TheKeyIsNeverInAnErrorMessageOrToString(int status)
    {
        // Even a server (or proxy) that echoes the key back in its message does not get it into the exception.
        var handler = new MockHandler(responder: _ => MockHandler.Json(status, $"{{\"error\":\"X\",\"message\":\"bad key {Key}\"}}"));
        using EnsembleWeatherClient client = Client(handler, o => o.MaxRetries = 0);

        EnsembleWeatherException ex = await Assert.ThrowsAnyAsync<EnsembleWeatherException>(
            () => client.GetLocationForecastAsync("or/mt-hood-meadows"));

        Assert.DoesNotContain(Key, ex.Message, StringComparison.Ordinal);
        Assert.DoesNotContain(Key, ex.ToString(), StringComparison.Ordinal);
        Assert.DoesNotContain(Key, ex.ApiMessage ?? string.Empty, StringComparison.Ordinal);
    }

    [Fact]
    public async Task TheKeyIsNeverInAConnectionError()
    {
        var handler = new ThrowingHandler(new HttpRequestException("connection reset"));
        using EnsembleWeatherClient client = Client(handler, o => o.MaxRetries = 0);

        ServiceConnectionException ex = await Assert.ThrowsAsync<ServiceConnectionException>(
            () => client.GetLocationForecastAsync("or/mt-hood-meadows"));

        Assert.DoesNotContain(Key, ex.ToString(), StringComparison.Ordinal);
    }

    [Fact]
    public void TheKeyIsNeverInToString()
    {
        var options = new EnsembleWeatherOptions { ApiKey = Key };
        using var client = new EnsembleWeatherClient(options);

        Assert.DoesNotContain(Key, client.ToString(), StringComparison.Ordinal);
        Assert.DoesNotContain(Key, options.ToString(), StringComparison.Ordinal);
    }

    [Fact]
    public async Task TheKeyIsNeverLogged()
    {
        var logs = new ListLogger();
        var services = new ServiceCollection();
        services.AddLogging(b => b.AddProvider(logs).SetMinimumLevel(LogLevel.Trace));
        services.AddEnsembleWeather(o => o.ApiKey = Key)
            .ConfigurePrimaryHttpMessageHandler(() => new MockHandler(responder: _ => MockHandler.Json(
                200,
                File.ReadAllText(Path.Combine(RepoPaths.Vectors, "bodies", "forecast-metric.json")),
                ("Deprecation", "@1798761600"),
                ("Link", "<https://docs.oureawx.com/changelog>; rel=\"deprecation\""))));
        using ServiceProvider provider = services.BuildServiceProvider();

        await provider.GetRequiredService<EnsembleWeatherClient>().GetStormReportsAsync();

        Assert.NotEmpty(logs.Messages);
        Assert.All(logs.Messages, m => Assert.DoesNotContain(Key, m, StringComparison.Ordinal));
    }

    [Theory]
    [InlineData(301, "https://elsewhere.example.test/api/v1/locations/or/mt-hood-meadows")]
    [InlineData(302, "https://elsewhere.example.test/api/v1/locations/or/mt-hood-meadows")]
    [InlineData(307, "https://api.oureawx.com.evil.test/api/v1/locations/or/mt-hood-meadows")]
    [InlineData(308, "http://api.oureawx.com/api/v1/locations/or/mt-hood-meadows")]
    [InlineData(308, "https://api.oureawx.com:8443/api/v1/locations/or/mt-hood-meadows")]
    public async Task NoKeyIsSentOnARedirectToAnotherOrigin(int status, string location)
    {
        var handler = new MockHandler(
        [
            new CannedResponse(status, [new("Location", location)], []),
            new CannedResponse(302, [new("Location", "https://api.oureawx.com/api/v1/locations/or/mt-hood-meadows")], []),
            MockHandler.Json(200, File.ReadAllText(Path.Combine(RepoPaths.Vectors, "bodies", "forecast-metric.json"))),
        ]);
        using EnsembleWeatherClient client = Client(handler);

        await client.GetLocationForecastAsync("or/mt-hood-meadows");

        Assert.Equal(Key, handler.Requests[0].Header("X-API-Key"));
        Assert.Null(handler.Requests[1].Header("X-API-Key"));
        Assert.Null(handler.Requests[2].Header("X-API-Key")); // once dropped, never re-added, even back on the API host
    }

    [Fact]
    public async Task RedirectsAreBoundedAtFive()
    {
        var handler = new MockHandler(responder: _ => new CannedResponse(302, [new("Location", "/api/v1/attribution")], []));
        using EnsembleWeatherClient client = Client(handler);

        await Assert.ThrowsAsync<EnsembleWeatherException>(() => client.GetAttributionAsync());
        Assert.Equal(6, handler.Requests.Count);
    }

    [Fact]
    public void NoOptionDisablesTlsVerification()
    {
        string[] forbidden = ["tls", "ssl", "certificate", "insecure", "verify", "validation", "callback", "handler"];
        foreach (PropertyInfo property in typeof(EnsembleWeatherOptions).GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            Assert.DoesNotContain(forbidden, f => property.Name.Contains(f, StringComparison.OrdinalIgnoreCase));
        }
    }

    [Fact]
    public void TheDefaultHandlerKeepsTlsVerificationAndDoesNotFollowRedirects()
    {
        using var client = new EnsembleWeatherClient(new EnsembleWeatherOptions { Environment = _ => null });
        var http = (HttpClient)typeof(EnsembleWeatherClient).GetField("_http", BindingFlags.NonPublic | BindingFlags.Instance)!.GetValue(client)!;
        var handler = (SocketsHttpHandler)typeof(HttpMessageInvoker).GetField("_handler", BindingFlags.NonPublic | BindingFlags.Instance)!.GetValue(http)!;

        Assert.Null(handler.SslOptions.RemoteCertificateValidationCallback);
        Assert.False(handler.AllowAutoRedirect);
    }

    [Fact]
    public async Task ABodyOverTheBoundIsAnErrorNotAnOutOfMemory()
    {
        var handler = new StreamHandler(() => new HttpResponseMessage(System.Net.HttpStatusCode.OK)
        {
            Content = new StreamContent(new EndlessStream()),
        });
        using EnsembleWeatherClient client = Client(handler);

        EnsembleWeatherException ex = await Assert.ThrowsAsync<EnsembleWeatherException>(() => client.GetAttributionAsync());

        Assert.Contains("byte bound", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public async Task ADeclaredLengthOverTheBoundIsRefusedBeforeReading()
    {
        var stream = new EndlessStream();
        var handler = new StreamHandler(() =>
        {
            var content = new StreamContent(stream);
            content.Headers.ContentLength = ApiPipeline.MaxResponseBytes + 1;
            return new HttpResponseMessage(System.Net.HttpStatusCode.OK) { Content = content };
        });
        using EnsembleWeatherClient client = Client(handler);

        await Assert.ThrowsAsync<EnsembleWeatherException>(() => client.GetAttributionAsync());
        Assert.Equal(0, stream.BytesRead);
    }

    [Fact]
    public async Task JsonNeverSelectsATypeFromTheBody()
    {
        // A "$type" (or any metadata) field is just an unknown field: models are fixed, source-generated types.
        const string body = """{"$type":"System.IO.FileInfo, System.IO.FileSystem","$id":"1","providers":[]}""";
        var handler = new MockHandler([MockHandler.Json(200, body)]);
        using EnsembleWeatherClient client = Client(handler);

        AttributionRegistry registry = await client.GetAttributionAsync();

        Assert.IsType<AttributionRegistry>(registry);
        Assert.Empty(registry.Providers);
    }

    private sealed class ThrowingHandler(Exception exception) : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            => Task.FromException<HttpResponseMessage>(exception);
    }

    private sealed class StreamHandler(Func<HttpResponseMessage> create) : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            => Task.FromResult(create());
    }

    /// <summary>A body that never ends (it yields '[' forever), to prove the bound without allocating it up front.</summary>
    private sealed class EndlessStream : Stream
    {
        public long BytesRead { get; private set; }

        public override bool CanRead => true;

        public override bool CanSeek => false;

        public override bool CanWrite => false;

        public override long Length => throw new NotSupportedException();

        public override long Position { get => BytesRead; set => throw new NotSupportedException(); }

        public override int Read(byte[] buffer, int offset, int count)
        {
            Array.Fill(buffer, (byte)'[', offset, count);
            BytesRead += count;
            return count;
        }

        public override void Flush()
        {
        }

        public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();

        public override void SetLength(long value) => throw new NotSupportedException();

        public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();
    }

    private sealed class ListLogger : ILoggerProvider, ILogger
    {
        public List<string> Messages { get; } = [];

        public ILogger CreateLogger(string categoryName) => this;

        public IDisposable? BeginScope<TState>(TState state)
            where TState : notnull => null;

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            lock (Messages)
            {
                Messages.Add(formatter(state, exception) + exception);
            }
        }

        public void Dispose()
        {
        }
    }
}
