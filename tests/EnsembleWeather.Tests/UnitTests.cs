// SPDX-License-Identifier: BSD-2-Clause

using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using EnsembleWeather.Tests.Support;
using Microsoft.Extensions.DependencyInjection;

namespace EnsembleWeather.Tests;

public sealed class WireRuleTests
{
    [Theory]
    [InlineData(0.0625, 3, "0.063")] // banker's rounding would give 0.062
    [InlineData(1.005, 2, "1.00")] // 1.005 * 100 = 100.4999... in doubles
    [InlineData(2.5, 0, "3")]
    [InlineData(-2.5, 0, "-3")]
    [InlineData(-0.04, 1, "0.0")] // -0 becomes 0
    [InlineData(152.39999999999998, 2, "152.40")]
    [InlineData(1524.4, 0, "1524")]
    public void ThresholdsRoundHalfAwayFromZeroInDoubles(double value, int decimals, string expected)
        => Assert.Equal(expected, WireRules.FormatFixed(WireRules.Round(value, decimals), decimals));

    [Theory]
    [InlineData(45.3311, "45.3311")]
    [InlineData(-121.6649, "-121.6649")]
    [InlineData(1e-7, "0.0000001")]
    [InlineData(1.5e21, "1500000000000000000000")]
    [InlineData(-2.5e-5, "-0.000025")]
    [InlineData(0.1 + 0.2, "0.30000000000000004")]
    [InlineData(1661, "1661")]
    public void NumbersAreTheShortestRoundTripWithoutAnExponent(double value, string expected)
        => Assert.Equal(expected, WireRules.FormatNumber(value));

    [Theory]
    [InlineData("Mt. Hood & co/ñ", "", "Mt.%20Hood%20%26%20co%2F%C3%B1")]
    [InlineData("a+b~c_d-e", "", "a%2Bb~c_d-e")]
    [InlineData("snowfall:ge:6.000:24h,x", ":,", "snowfall:ge:6.000:24h,x")]
    public void PercentEncodingIsRfc3986(string value, string keep, string expected)
        => Assert.Equal(expected, WireRules.PercentEncode(value, keep));

    [Theory]
    [InlineData(6, ThresholdUnit.Inches, "snowfall", "metric", 152.4)]
    [InlineData(152.4, ThresholdUnit.Millimeters, "snowfall", "imperial", 6)]
    [InlineData(32, ThresholdUnit.Fahrenheit, "temperature", "metric", 0)]
    [InlineData(0, ThresholdUnit.Celsius, "temperature", "imperial", 32)]
    [InlineData(0, ThresholdUnit.Celsius, "temperature", "kelvin", 273.15)]
    [InlineData(273.15, ThresholdUnit.Kelvin, "temperature", "metric", 0)]
    [InlineData(5000, ThresholdUnit.Feet, "snow_level", "metric", 1524)]
    [InlineData(10, ThresholdUnit.MetersPerSecond, "wind_speed", "imperial", 22.369362920544)]
    public void CallerThresholdsConvertWithExactFactors(double value, ThresholdUnit unit, string variable, string units, double expected)
        => Assert.Equal(expected, WireRules.ToRequestUnits(new Threshold(value, unit), new ForecastVariable(variable), new UnitSystem(units)), 9);
}

public sealed class TermTests
{
    [Fact]
    public void AnAggregationThatDoesNotFitTheVariableIsRefused()
    {
        Assert.Throws<ArgumentException>(() => ProbabilityTerm.AtLeast(ForecastVariable.Snowfall, Threshold.Inches(6), aggregation: ProbabilityAggregation.Any));
        Assert.Throws<ArgumentException>(() => ProbabilityTerm.AtMost(ForecastVariable.Temperature, Threshold.Celsius(0), aggregation: ProbabilityAggregation.Total));
        Assert.Throws<ArgumentException>(() => ProbabilityTerm.AtLeast(ForecastVariable.Temperature, Threshold.Inches(1)));
        Assert.Throws<ArgumentException>(() => new ProbabilityTerm(new ForecastVariable("humidity"), ProbabilityOperator.AtLeast, Threshold.InRequestUnits(1)));
        Assert.Throws<ArgumentException>(() => new WindowTotalTerm(ForecastVariable.Temperature, ForecastWindow.Hours(24)));
    }

    [Theory]
    [InlineData(0, 24, "24h")]
    [InlineData(12, 36, "12-36h")]
    [InlineData(0, 360, "360h")]
    public void WindowsUseTheWireForm(int start, int end, string expected)
        => Assert.Equal(expected, ForecastWindow.Between(start, end).ToString());

    [Theory]
    [InlineData(0, 0)]
    [InlineData(10, 5)]
    [InlineData(0, 361)]
    [InlineData(-1, 5)]
    public void InvalidWindowsAreRefused(int start, int end)
        => Assert.Throws<ArgumentOutOfRangeException>(() => ForecastWindow.Between(start, end));

    [Fact]
    public void QuantileRangesAreOnlyTheServedPairs()
    {
        var q = new Quantiles { P10 = -4, P25 = -3, P50 = -2, P75 = -1, P90 = 0 };

        Assert.Equal(new QuantileRange(10, -4, 90, 0), q.GetRange(10, 90));
        Assert.Equal(new QuantileRange(25, -3, 75, -1), q.GetRange(25, 75));
        Assert.Equal(-2, q.Median);
        Assert.Throws<ArgumentException>(() => q.GetRange(5, 95));
        Assert.Throws<ArgumentException>(() => q.GetRange(10, 75));
        Assert.Null(new Quantiles { P10 = 1 }.GetRange(10, 90));
    }

    [Theory]
    [InlineData("or/mt-hood-meadows", "or", "mt-hood-meadows")]
    [InlineData("OR/x", "OR", "x")]
    public void LocationIdsSplitOnTheirOneSlash(string text, string state, string slug)
    {
        LocationId id = LocationId.Parse(text);
        Assert.Equal(state, id.State);
        Assert.Equal(slug, id.Slug);
    }

    [Theory]
    [InlineData("mt-hood-meadows")]
    [InlineData("or/a/b")]
    [InlineData("/x")]
    [InlineData("or/")]
    public void MalformedLocationIdsAreRefused(string text)
        => Assert.Throws<ArgumentException>(() => LocationId.Parse(text));

    [Fact]
    public void ExtensibleEnumsKeepUnknownValues()
    {
        var later = new LocationType("a_type_added_later");
        Assert.False(later.IsKnown);
        Assert.Equal("a_type_added_later", later.ToString());
        Assert.True(LocationType.SkiArea.IsKnown);
        Assert.Equal(LocationType.SkiArea, new LocationType("ski_area"));
    }
}

public sealed class TransportTests
{
    private static readonly string Forecast = File.ReadAllText(Path.Combine(RepoPaths.Vectors, "bodies", "forecast-metric.json"));

    private static (EnsembleWeatherClient Client, RecordingTimeProvider Clock) Client(HttpMessageHandler handler, Action<EnsembleWeatherOptions>? configure = null)
    {
        var clock = new RecordingTimeProvider();
        var options = new EnsembleWeatherOptions { TimeProvider = clock, Jitter = () => 1, Environment = _ => null };
        configure?.Invoke(options);
        return (new EnsembleWeatherClient(new HttpClient(handler), options), clock);
    }

    [Fact]
    public async Task ConnectionErrorsAreRetried()
    {
        int calls = 0;
        var handler = new MockHandler(responder: _ => ++calls < 3 ? throw new HttpRequestException("reset") : MockHandler.Json(200, Forecast));
        (EnsembleWeatherClient client, RecordingTimeProvider clock) = Client(handler);

        await client.GetLocationForecastAsync("or/mt-hood-meadows");

        Assert.Equal(3, calls);
        Assert.Equal([0.5, 1.0], clock.Delays.Select(d => d.TotalSeconds));
    }

    [Fact]
    public async Task TlsVerificationFailuresAreNotRetried()
    {
        int calls = 0;
        var handler = new MockHandler(responder: _ =>
        {
            calls++;
            throw new HttpRequestException("tls", new System.Security.Authentication.AuthenticationException("bad certificate"));
        });
        (EnsembleWeatherClient client, _) = Client(handler);

        await Assert.ThrowsAsync<ServiceConnectionException>(() => client.GetLocationForecastAsync("or/mt-hood-meadows"));
        Assert.Equal(1, calls);
    }

    [Fact]
    public async Task RetryAfterAsAnHttpDateIsHonoured()
    {
        var handler = new MockHandler(
        [
            MockHandler.Json(503, """{"error":"FORECAST_UNAVAILABLE","message":"x"}""", ("Retry-After", "Sun, 15 Nov 2026 00:00:07 GMT")),
            MockHandler.Json(200, Forecast),
        ]);
        (EnsembleWeatherClient client, RecordingTimeProvider clock) = Client(handler);

        await client.GetLocationForecastAsync("or/mt-hood-meadows");

        Assert.Equal([7.0], clock.Delays.Select(d => d.TotalSeconds));
    }

    [Fact]
    public async Task RetryAfterOnA504IsHonoured()
    {
        var handler = new MockHandler(
        [
            MockHandler.Json(504, """{"error":"TIMEOUT","message":"x"}""", ("Retry-After", "5")),
            MockHandler.Json(200, Forecast),
        ]);
        (EnsembleWeatherClient client, RecordingTimeProvider clock) = Client(handler);

        await client.GetLocationForecastAsync("or/mt-hood-meadows");

        Assert.Equal([5.0], clock.Delays.Select(d => d.TotalSeconds));
    }

    [Fact]
    public async Task AnErrorBodysRequestIdDocUrlParamAndNearestLocationAreKept()
    {
        const string body = """
            {"error":"PRO_REQUIRED","message":"Coordinates need a Pro key.","param":"lat",
             "request_id":"01JBQ6ZK3V8M4T2N7XQH5R9W0C",
             "doc_url":"https://docs.oureawx.com/features/errors/#pro_required",
             "nearest_location":{"slug":"mt-hood-meadows","url":"/api/v1/locations/or/mt-hood-meadows",
                                 "distance_km":4.25,"elevation_m":1635.6}}
            """;
        var handler = new MockHandler([MockHandler.Json(403, body, ("X-Request-Id", "header-id"))]);
        (EnsembleWeatherClient client, _) = Client(handler);

        PermissionDeniedException ex = await Assert.ThrowsAsync<PermissionDeniedException>(
            () => client.GetPointForecastAsync(45.3, -121.7));

        Assert.Equal("PRO_REQUIRED", ex.ErrorCode);
        Assert.Equal("01JBQ6ZK3V8M4T2N7XQH5R9W0C", ex.RequestId);
        Assert.Equal(new Uri("https://docs.oureawx.com/features/errors/#pro_required"), ex.DocUrl);
        Assert.Equal("lat", ex.Parameter);
        Assert.Equal("mt-hood-meadows", ex.NearestLocation?.Slug);
        Assert.Equal("/api/v1/locations/or/mt-hood-meadows", ex.NearestLocation?.Url);
    }

    [Theory]
    [InlineData("FORBIDDEN", 400)]
    [InlineData("GONE", 400)]
    [InlineData("MODEL_RUN_NOT_FOUND", 400)]
    [InlineData("AMBIGUOUS", 400)]
    [InlineData("INVALID_MAX_HOURS", 400)]
    public async Task WithdrawnCodesKeepTheirStringAndMapByStatus(string code, int status)
    {
        var handler = new MockHandler([MockHandler.Json(status, $$"""{"error":"{{code}}","message":"x"}""")]);
        (EnsembleWeatherClient client, _) = Client(handler);

        InvalidRequestException ex = await Assert.ThrowsAsync<InvalidRequestException>(
            () => client.GetLocationForecastAsync("or/mt-hood-meadows"));

        Assert.Equal(code, ex.ErrorCode);
    }

    [Fact]
    public async Task AuthUnavailableIsAServiceError()
    {
        var handler = new MockHandler(responder: _ => MockHandler.Json(503, """{"error":"AUTH_UNAVAILABLE","message":"x"}""", ("Retry-After", "1")));
        (EnsembleWeatherClient client, _) = Client(handler);

        await Assert.ThrowsAsync<ServiceUnavailableException>(() => client.GetLocationForecastAsync("or/mt-hood-meadows"));
    }

    [Fact]
    public async Task BackoffIsCappedAtEightSeconds()
    {
        var handler = new MockHandler(responder: _ => MockHandler.Json(502, "{}"));
        (EnsembleWeatherClient client, RecordingTimeProvider clock) = Client(handler, o => o.MaxRetries = 6);

        await Assert.ThrowsAsync<ServiceUnavailableException>(() => client.GetLocationForecastAsync("or/mt-hood-meadows"));

        Assert.Equal([0.5, 1, 2, 4, 8, 8], clock.Delays.Select(d => d.TotalSeconds));
    }

    [Fact]
    public async Task RateLimitStateIsOnTheResultAndTheClient()
    {
        var handler = new MockHandler([MockHandler.Json(200, Forecast, ("RateLimit-Limit", "60"), ("RateLimit-Remaining", "12"), ("RateLimit-Reset", "30"), ("RateLimit-Policy", "60;w=60"), ("X-Request-Id", "req-1"))]);
        (EnsembleWeatherClient client, _) = Client(handler);

        ForecastResponse forecast = await client.GetLocationForecastAsync("or/mt-hood-meadows");

        Assert.Equal(12, forecast.Metadata.RateLimit!.Remaining);
        Assert.Equal(TimeSpan.FromSeconds(30), forecast.Metadata.RateLimit.Reset);
        Assert.Equal("60;w=60", client.RateLimit!.Policy);
        Assert.Equal("req-1", forecast.Metadata.RequestId);
    }

    [Fact]
    public async Task AFreshMaxAgeEntryIsServedWithoutARequest()
    {
        var handler = new MockHandler([MockHandler.Json(200, Forecast, ("Cache-Control", "max-age=60"), ("ETag", "\"e\""))]);
        (EnsembleWeatherClient client, _) = Client(handler);

        await client.GetLocationForecastAsync("or/mt-hood-meadows");
        await client.GetLocationForecastAsync("or/mt-hood-meadows");

        Assert.Single(handler.Requests);
    }

    [Fact]
    public async Task TheUserAgentNamesTheSdkAndTheApplication()
    {
        var handler = new MockHandler([MockHandler.Json(200, Forecast)]);
        (EnsembleWeatherClient client, _) = Client(handler, o => o.ApplicationId = "my-app/1.2");

        await client.GetLocationForecastAsync("or/mt-hood-meadows");

        Assert.Matches(@"^ensemble-weather-dotnet/0\.1\.0 \(\.NET/\d+\.\d+\.\d+\) my-app/1\.2$", handler.Requests[0].Header("User-Agent"));
    }

    [Fact]
    public void TheSdkVersionMatchesThePackage()
    {
        string? informational = typeof(EnsembleWeatherClient).Assembly
            .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion;
        Assert.StartsWith(SdkInfo.Version, informational, StringComparison.Ordinal);
    }

    [Fact]
    public async Task TheBaseUrlComesFromTheEnvironmentWhenNotSet()
    {
        var handler = new MockHandler([MockHandler.Json(200, Forecast)]);
        using var client = new EnsembleWeatherClient(new HttpClient(handler), new EnsembleWeatherOptions
        {
            Environment = k => k == "OUREA_BASE_URL" ? "http://127.0.0.1:5099" : null,
        });

        await client.GetLocationForecastAsync("or/mt-hood-meadows");

        Assert.Equal("http://127.0.0.1:5099/api/v1/locations/or/mt-hood-meadows", handler.Requests[0].Uri.AbsoluteUri);
    }
}

public sealed class CacheTests
{
    private static CachedResponse Entry(int bytes) => new("\"e\"", new byte[bytes], DateTimeOffset.UnixEpoch, null);

    [Fact]
    public async Task TheLruEvictsTheLeastRecentlyUsedEntryByCount()
    {
        var cache = new MemoryResponseCache(maxEntries: 2);
        await cache.SetAsync("a", Entry(1));
        await cache.SetAsync("b", Entry(1));
        await cache.GetAsync("a");
        await cache.SetAsync("c", Entry(1));

        Assert.NotNull(await cache.GetAsync("a"));
        Assert.Null(await cache.GetAsync("b"));
        Assert.Equal(2, cache.Count);
    }

    [Fact]
    public async Task TheLruIsBoundedByBytes()
    {
        var cache = new MemoryResponseCache(maxBytes: 10);
        await cache.SetAsync("a", Entry(6));
        await cache.SetAsync("b", Entry(6));
        await cache.SetAsync("huge", Entry(11));

        Assert.Null(await cache.GetAsync("a"));
        Assert.NotNull(await cache.GetAsync("b"));
        Assert.Null(await cache.GetAsync("huge"));
    }

    [Fact]
    public void TheDefaultsAre256EntriesAnd32MiB()
    {
        var cache = new MemoryResponseCache();
        Assert.Equal(256, cache.MaxEntries);
        Assert.Equal(32L * 1024 * 1024, cache.MaxBytes);
    }
}

public sealed class DependencyInjectionTests
{
    [Fact]
    public async Task AddEnsembleWeatherRegistersATypedClientWithASharedCache()
    {
        var handler = new MockHandler(responder: _ => MockHandler.Json(
            200, File.ReadAllText(Path.Combine(RepoPaths.Vectors, "bodies", "attribution.json")), ("ETag", "\"a\"")));
        var services = new ServiceCollection();
        services.AddEnsembleWeather(o =>
        {
            o.ApiKey = "ow_di";
            o.Units = UnitSystem.Imperial;
        }).ConfigurePrimaryHttpMessageHandler(() => handler);
        using ServiceProvider provider = services.BuildServiceProvider();

        EnsembleWeatherClient first = provider.GetRequiredService<EnsembleWeatherClient>();
        EnsembleWeatherClient second = provider.GetRequiredService<EnsembleWeatherClient>();
        await first.GetAttributionAsync();
        await second.GetAttributionAsync();

        Assert.NotSame(first, second);
        Assert.Equal("\"a\"", handler.Requests[1].Header("If-None-Match")); // the cache outlives the transient clients
    }

    [Fact]
    public void ThePrimaryHandlerDoesNotFollowRedirects()
    {
        var services = new ServiceCollection();
        services.AddEnsembleWeather();
        using ServiceProvider provider = services.BuildServiceProvider();
        var factory = provider.GetRequiredService<IHttpMessageHandlerFactory>();

        HttpMessageHandler handler = factory.CreateHandler(EnsembleWeatherServiceCollectionExtensions.HttpClientName);
        while (handler is DelegatingHandler delegating)
        {
            handler = delegating.InnerHandler!;
        }

        Assert.False(Assert.IsType<SocketsHttpHandler>(handler).AllowAutoRedirect);
    }
}

public sealed class RepositoryTests
{
    [Fact]
    public void TheReadmeQuickstartIsTheQuickstartExample()
    {
        string readme = File.ReadAllText(Path.Combine(RepoPaths.Root, "README.md")).Replace("\r\n", "\n", StringComparison.Ordinal);
        string section = readme[readme.IndexOf("## Quickstart", StringComparison.Ordinal)..];
        int start = section.IndexOf("```csharp\n", StringComparison.Ordinal) + "```csharp\n".Length;
        string code = section[start..section.IndexOf("```", start, StringComparison.Ordinal)];
        string example = File.ReadAllText(Path.Combine(RepoPaths.Root, "examples", "Quickstart", "Program.cs")).Replace("\r\n", "\n", StringComparison.Ordinal);

        Assert.Equal(example, code);
    }

    [Theory]
    [InlineData("README.md")]
    [InlineData("LICENSE")]
    [InlineData("CHANGELOG.md")]
    [InlineData("CONTRIBUTING.md")]
    [InlineData("SECURITY.md")]
    [InlineData("CODE_OF_CONDUCT.md")]
    [InlineData(".github/workflows/ci.yml")]
    [InlineData(".github/workflows/live.yml")]
    [InlineData(".github/workflows/release.yml")]
    [InlineData(".github/workflows/security.yml")]
    [InlineData(".github/workflows/codeql.yml")]
    [InlineData(".github/workflows/scorecard.yml")]
    [InlineData(".github/dependabot.yml")]
    [InlineData(".github/ISSUE_TEMPLATE/config.yml")]
    [InlineData(".github/pull_request_template.md")]
    [InlineData("spec/VERSION")]
    [InlineData("scripts/sync-spec")]
    public void EverySdkStandardFileExists(string path)
        => Assert.True(File.Exists(Path.Combine(RepoPaths.Root, path)), $"{path} is missing");
}
