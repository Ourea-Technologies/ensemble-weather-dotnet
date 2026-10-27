// SPDX-License-Identifier: BSD-2-Clause

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using EnsembleWeather.Tests.Support;

namespace EnsembleWeather.Tests;

/// <summary>
/// Runs every shared vector in <c>spec/vectors/</c> (BEHAVIOR.md section 13) through a mock transport. A vector this
/// runner cannot run fails; nothing is skipped.
/// </summary>
public sealed class VectorTests
{
    /// <summary>The number of vectors in the pinned spec; a change is deliberate and reviewed.</summary>
    private const int ExpectedVectorCount = 43;

    public static TheoryData<string> Vectors()
    {
        var data = new TheoryData<string>();
        foreach (string name in VectorNames())
        {
            data.Add(name);
        }

        return data;
    }

    [Fact]
    public void EveryVectorIsDiscovered()
        => Assert.Equal(ExpectedVectorCount, VectorNames().Count);

    [Theory]
    [MemberData(nameof(Vectors))]
    public async Task Vector(string name)
    {
        using JsonDocument doc = JsonDocument.Parse(File.ReadAllText(Path.Combine(RepoPaths.Vectors, name + ".json")));
        JsonElement vector = doc.RootElement;
        JsonElement client = vector.TryGetProperty("client", out JsonElement c) ? c : default;

        var handler = new MockHandler(vector.GetProperty("responses").EnumerateArray().Select(ReadResponse));
        var clock = new RecordingTimeProvider();
        var warnings = new List<string>();
        var registry = new DeprecationRegistry();
        bool sharesCache = Calls(vector).Any(call => call.TryGetProperty("with_api_key", out _));
        IResponseCache? sharedCache = sharesCache ? new MemoryResponseCache() : null;

        EnsembleWeatherOptions Options(string? apiKey)
        {
            var env = new Dictionary<string, string>();
            if (client.ValueKind == JsonValueKind.Object && client.TryGetProperty("env", out JsonElement e))
            {
                foreach (JsonProperty p in e.EnumerateObject())
                {
                    env[p.Name] = p.Value.GetString()!;
                }
            }

            var options = new EnsembleWeatherOptions
            {
                ApiKey = apiKey,
                TimeProvider = clock,
                Jitter = () => 1.0,
                Deprecations = registry,
                OnDeprecation = n => warnings.Add(n.ToString()),
                Environment = k => env.TryGetValue(k, out string? v) ? v : null,
                Cache = sharedCache,
            };
            if (client.ValueKind == JsonValueKind.Object)
            {
                if (client.TryGetProperty("base_url", out JsonElement b))
                {
                    options.BaseUrl = new Uri(b.GetString()!);
                }

                if (client.TryGetProperty("max_retries", out JsonElement m))
                {
                    options.MaxRetries = m.GetInt32();
                }

                if (client.TryGetProperty("cache", out JsonElement ca))
                {
                    options.EnableCache = ca.GetBoolean();
                }

                if (client.TryGetProperty("send_key_on_open_routes", out JsonElement s))
                {
                    options.SendApiKeyOnOpenRoutes = s.GetBoolean();
                }
            }

            return options;
        }

        string? defaultKey = client.ValueKind == JsonValueKind.Object && client.TryGetProperty("api_key", out JsonElement k)
            ? k.GetString()
            : null;
        using var http = new HttpClient(handler);
        var sdk = new EnsembleWeatherClient(http, Options(defaultKey));

        var results = new List<object?>();
        Exception? error = null;
        foreach (JsonElement call in Calls(vector))
        {
            EnsembleWeatherClient target = call.TryGetProperty("with_api_key", out JsonElement other)
                ? new EnsembleWeatherClient(http, Options(other.GetString()))
                : sdk;
            try
            {
                results.Add(await Invoke(target, call));
            }
            catch (Exception ex) when (ex is EnsembleWeatherException or ArgumentException)
            {
                error = ex;
                results.Add(null);
                break;
            }
        }

        CheckRequests(vector, handler.Requests);
        JsonElement expect = vector.GetProperty("expect");
        if (expect.TryGetProperty("error", out JsonElement expectedError))
        {
            CheckError(expectedError, error);
        }
        else if (error is not null)
        {
            Assert.Fail($"Unexpected error: {error}");
        }

        if (expect.TryGetProperty("result", out JsonElement single))
        {
            CheckResult(single, results[0]);
        }

        if (expect.TryGetProperty("results", out JsonElement many))
        {
            int i = 0;
            foreach (JsonElement r in many.EnumerateArray())
            {
                CheckResult(r, results[i++]);
            }
        }

        if (expect.TryGetProperty("requests", out JsonElement count))
        {
            Assert.Equal(count.GetInt32(), handler.Requests.Count);
        }

        if (expect.TryGetProperty("delays_s", out JsonElement delays))
        {
            Assert.Equal(
                delays.EnumerateArray().Select(d => d.GetDouble()).ToArray(),
                clock.Delays.Select(d => d.TotalSeconds).ToArray());
        }

        if (expect.TryGetProperty("items", out JsonElement items))
        {
            var slugs = ((IEnumerable)results[0]!).Cast<object>().Select(o => (string)Get(o, "slug")!).ToArray();
            Assert.Equal(items.EnumerateArray().Select(i => i.GetString()).ToArray(), slugs);
        }

        if (expect.TryGetProperty("warnings", out JsonElement expectedWarnings))
        {
            Assert.Equal(expectedWarnings.GetArrayLength(), warnings.Count);
            int i = 0;
            foreach (JsonElement w in expectedWarnings.EnumerateArray())
            {
                Assert.Contains(w.GetProperty("contains").GetString()!, warnings[i++], StringComparison.Ordinal);
            }
        }
    }

    private static List<string> VectorNames()
        => Directory.EnumerateFiles(RepoPaths.Vectors, "*.json", SearchOption.AllDirectories)
            .Where(p => !p.Contains(Path.DirectorySeparatorChar + "bodies" + Path.DirectorySeparatorChar, StringComparison.Ordinal))
            .Select(p => Path.GetRelativePath(RepoPaths.Vectors, p).Replace(Path.DirectorySeparatorChar, '/')[..^5])
            .OrderBy(n => n, StringComparer.Ordinal)
            .ToList();

    private static List<JsonElement> Calls(JsonElement vector)
        => vector.TryGetProperty("calls", out JsonElement calls) ? [.. calls.EnumerateArray()] : [vector.GetProperty("call")];

    private static CannedResponse ReadResponse(JsonElement r)
    {
        byte[] body = r.TryGetProperty("body_file", out JsonElement file)
            ? File.ReadAllBytes(Path.Combine(RepoPaths.Vectors, file.GetString()!))
            : r.TryGetProperty("body", out JsonElement raw) ? Encoding.UTF8.GetBytes(raw.GetString()!) : [];
        var headers = r.TryGetProperty("headers", out JsonElement h)
            ? h.EnumerateObject().Select(p => new KeyValuePair<string, string>(p.Name, p.Value.GetString()!)).ToList()
            : [];
        return new CannedResponse(r.GetProperty("status").GetInt32(), headers, body);
    }

    // ---- The per-language mapping: semantic args -> this SDK's API ------------------------------------------------

    private static async Task<object?> Invoke(EnsembleWeatherClient client, JsonElement call)
    {
        string operation = call.GetProperty("operation").GetString()!;
        JsonElement a = call.GetProperty("args");
        bool iterate = Bool(a, "iterate") == true;
        switch (operation)
        {
            case "getLocationForecast":
                return await client.GetLocationForecastAsync(Location(a), Forecast(a));
            case "getPointForecast":
                return await client.GetPointForecastAsync(Num(a, "lat")!.Value, Num(a, "lon")!.Value, Num(a, "elevation_m"), Forecast(a));
            case "getCurrentConditions":
                return await client.GetCurrentConditionsAsync(Location(a));
            case "getCurrentByCoordinates":
                return await client.GetCurrentByCoordinatesAsync(Num(a, "lat")!.Value, Num(a, "lon")!.Value, Num(a, "elevation_m"));
            case "getLocationAlerts":
                return await client.GetLocationAlertsAsync(Location(a));
            case "getAlertsByCoordinates":
                return await client.GetAlertsByCoordinatesAsync(Num(a, "lat")!.Value, Num(a, "lon")!.Value);
            case "getNearbyLocations":
                return await client.GetNearbyLocationsAsync(Location(a), Nearby(a));
            case "getNearbyByCoordinates":
                return await client.GetNearbyByCoordinatesAsync(Num(a, "lat")!.Value, Num(a, "lon")!.Value, Nearby(a));
            case "searchLocations":
                return await client.SearchLocationsAsync(Str(a, "q")!, new SearchOptions { State = Str(a, "state"), County = Str(a, "county"), Limit = Int(a, "limit") });
            case "getAllLocations":
                return iterate ? await Collect(client.GetAllLocationsAsync(LocationList(a))) : await client.GetAllLocationsPageAsync(LocationList(a));
            case "getLocations":
                return iterate ? await Collect(client.GetLocationsAsync(Str(a, "state")!, LocationList(a))) : await client.GetLocationsPageAsync(Str(a, "state")!, LocationList(a));
            case "listCatalogLocations":
                var catalog = new CatalogLocationListOptions { Cursor = Str(a, "cursor"), Limit = Int(a, "limit"), Admin1 = Str(a, "admin1") };
                return iterate ? await Collect(client.GetCatalogLocationsAsync(catalog)) : await client.GetCatalogLocationsPageAsync(catalog);
            case "getCatalogLocation":
                return await client.GetCatalogLocationAsync(Str(a, "slug")!);
            case "listCatalogStations":
                var stations = new CatalogStationListOptions { Cursor = Str(a, "cursor"), Limit = Int(a, "limit") };
                return iterate ? await Collect(client.GetCatalogStationsAsync(stations)) : await client.GetCatalogStationsPageAsync(stations);
            case "getCatalogStation":
                return await client.GetCatalogStationAsync(Str(a, "slug")!);
            case "getAvailableRuns":
                return await client.GetAvailableRunsAsync(Int(a, "limit"));
            case "getStormReports":
                return await client.GetStormReportsAsync(Int(a, "limit"));
            case "getStormReport":
                return await client.GetStormReportAsync(a.GetProperty("id").GetInt64());
            case "getAttribution":
                return await client.GetAttributionAsync();
            default:
                throw new InvalidOperationException($"The vectors runner has no mapping for '{operation}'.");
        }
    }

    private static async Task<List<T>> Collect<T>(IAsyncEnumerable<T> items)
    {
        var list = new List<T>();
        await foreach (T item in items)
        {
            list.Add(item);
        }

        return list;
    }

    private static LocationId Location(JsonElement a)
        => a.TryGetProperty("location", out JsonElement l) ? LocationId.Parse(l.GetString()!) : new LocationId(Str(a, "state")!, Str(a, "name")!);

    private static ForecastOptions Forecast(JsonElement a)
    {
        var o = new ForecastOptions
        {
            Units = Str(a, "units") is string u ? new UnitSystem(u) : null,
            MaxHours = Int(a, "max_hours"),
            IncludePast = Bool(a, "include_past"),
            Models = Bool(a, "models"),
            Members = Bool(a, "members"),
            Daily = Bool(a, "daily"),
            MaxDays = Int(a, "max_days"),
            Hourly = Bool(a, "hourly"),
            Nowcast = Bool(a, "nowcast"),
        };
        if (a.TryGetProperty("prob", out JsonElement prob))
        {
            foreach (JsonElement t in prob.EnumerateArray())
            {
                o.Probabilities.Add(new ProbabilityTerm(
                    new ForecastVariable(t.GetProperty("variable").GetString()!),
                    new ProbabilityOperator(t.GetProperty("op").GetString()!),
                    Threshold(t.GetProperty("threshold")),
                    t.TryGetProperty("window", out JsonElement w) ? Window(w) : null,
                    Str(t, "aggregation") is string agg ? new ProbabilityAggregation(agg) : null));
            }
        }

        if (a.TryGetProperty("totals", out JsonElement totals))
        {
            foreach (JsonElement t in totals.EnumerateArray())
            {
                o.WindowTotals.Add(new WindowTotalTerm(new ForecastVariable(t.GetProperty("variable").GetString()!), Window(t.GetProperty("window"))));
            }
        }

        return o;
    }

    private static NearbyOptions Nearby(JsonElement a) => new() { Limit = Int(a, "limit"), RadiusKm = Num(a, "radius_km") };

    private static LocationListOptions LocationList(JsonElement a) => new() { Limit = Int(a, "limit"), Cursor = Str(a, "cursor") };

    private static Threshold Threshold(JsonElement t)
    {
        double value = t.GetProperty("value").GetDouble();
        ThresholdUnit unit = t.GetProperty("unit").GetString() switch
        {
            "mm" => ThresholdUnit.Millimeters,
            "in" => ThresholdUnit.Inches,
            "degC" => ThresholdUnit.Celsius,
            "degF" => ThresholdUnit.Fahrenheit,
            "K" => ThresholdUnit.Kelvin,
            "m" => ThresholdUnit.Meters,
            "ft" => ThresholdUnit.Feet,
            "m/s" => ThresholdUnit.MetersPerSecond,
            "mph" => ThresholdUnit.MilesPerHour,
            string other => throw new InvalidOperationException($"Unknown threshold unit '{other}'."),
            null => throw new InvalidOperationException("A threshold without a unit."),
        };
        return new Threshold(value, unit);
    }

    private static ForecastWindow Window(JsonElement w)
        => w.TryGetProperty("hours", out JsonElement h)
            ? ForecastWindow.Hours(h.GetInt32())
            : ForecastWindow.Between(w.GetProperty("start_hours").GetInt32(), w.GetProperty("end_hours").GetInt32());

    private static string? Str(JsonElement a, string name) => a.TryGetProperty(name, out JsonElement v) ? v.GetString() : null;

    private static int? Int(JsonElement a, string name) => a.TryGetProperty(name, out JsonElement v) ? v.GetInt32() : null;

    private static double? Num(JsonElement a, string name) => a.TryGetProperty(name, out JsonElement v) ? v.GetDouble() : null;

    private static bool? Bool(JsonElement a, string name) => a.TryGetProperty(name, out JsonElement v) ? v.GetBoolean() : null;

    // ---- Expectations -----------------------------------------------------------------------------------------

    private static void CheckRequests(JsonElement vector, List<RecordedRequest> requests)
    {
        List<JsonElement> expected;
        if (vector.TryGetProperty("expect_requests", out JsonElement many))
        {
            expected = many.EnumerateArray().ToList();
        }
        else
        {
            JsonElement one = vector.GetProperty("expect_request");
            if (one.ValueKind == JsonValueKind.Null)
            {
                Assert.Empty(requests);
                return;
            }

            expected = [one];
        }

        Assert.True(requests.Count >= expected.Count, $"Expected at least {expected.Count} requests, got {requests.Count}.");
        for (int i = 0; i < expected.Count; i++)
        {
            CheckRequest(expected[i], requests[i]);
        }
    }

    private static void CheckRequest(JsonElement e, RecordedRequest actual)
    {
        string query = actual.Uri.Query.Length > 0 ? actual.Uri.Query[1..] : string.Empty;
        Assert.Equal(e.GetProperty("method").GetString(), actual.Method);
        if (e.TryGetProperty("path", out JsonElement path))
        {
            Assert.Equal(path.GetString(), actual.Uri.AbsolutePath);
        }

        if (e.TryGetProperty("query_string", out JsonElement qs))
        {
            Assert.Equal(qs.GetString(), query);
        }

        ILookup<string, string> parsed = query.Split('&', StringSplitOptions.RemoveEmptyEntries).Select(p => p.Split('=', 2)).ToLookup(p => p[0], p => Uri.UnescapeDataString(p.Length > 1 ? p[1] : string.Empty));
        if (e.TryGetProperty("query", out JsonElement q))
        {
            foreach (JsonProperty p in q.EnumerateObject())
            {
                Assert.Equal([p.Value.GetString()!], parsed[p.Name].ToArray());
            }
        }

        if (e.TryGetProperty("query_absent", out JsonElement absent))
        {
            foreach (JsonElement n in absent.EnumerateArray())
            {
                Assert.False(parsed.Contains(n.GetString()!), $"Query parameter {n} must be absent.");
            }
        }

        if (e.TryGetProperty("headers", out JsonElement headers))
        {
            foreach (JsonProperty h in headers.EnumerateObject())
            {
                Assert.Equal(h.Value.GetString(), actual.Header(h.Name));
            }
        }

        if (e.TryGetProperty("headers_absent", out JsonElement noHeaders))
        {
            foreach (JsonElement n in noHeaders.EnumerateArray())
            {
                Assert.Null(actual.Header(n.GetString()!));
            }
        }

        if (e.TryGetProperty("headers_match", out JsonElement match))
        {
            foreach (JsonProperty h in match.EnumerateObject())
            {
                Assert.Matches(new Regex(h.Value.GetString()!), actual.Header(h.Name) ?? string.Empty);
            }
        }

        if (e.TryGetProperty("url_prefix", out JsonElement prefix))
        {
            Assert.StartsWith(prefix.GetString()!, actual.Uri.AbsoluteUri, StringComparison.Ordinal);
        }
    }

    private static void CheckError(JsonElement expected, Exception? actual)
    {
        Assert.NotNull(actual);
        Assert.Equal(expected.GetProperty("category").GetString(), Category(actual));
        var api = actual as EnsembleWeatherException;
        if (expected.TryGetProperty("code", out JsonElement code))
        {
            Assert.Equal(code.ValueKind == JsonValueKind.Null ? null : code.GetString(), api?.ErrorCode);
        }

        if (expected.TryGetProperty("status", out JsonElement status))
        {
            Assert.Equal(status.GetInt32(), api?.StatusCode);
        }

        if (expected.TryGetProperty("retry_after", out JsonElement retryAfter))
        {
            Assert.Equal(retryAfter.GetDouble(), api?.RetryAfter?.TotalSeconds);
        }

        if (expected.TryGetProperty("string_excludes", out JsonElement excludes))
        {
            Assert.DoesNotContain(excludes.GetString()!, actual.Message, StringComparison.Ordinal);
            Assert.DoesNotContain(excludes.GetString()!, actual.ToString(), StringComparison.Ordinal);
        }
    }

    internal static string Category(Exception e) => e switch
    {
        AuthenticationFailedException => "authentication",
        PermissionDeniedException => "permission",
        ResourceNotFoundException => "not_found",
        InvalidRequestException => "invalid_request",
        OutsideCoverageException => "outside_coverage",
        RateLimitExceededException => "rate_limited",
        ServiceUnavailableException => "unavailable",
        ServiceConnectionException => "connection",
        ArgumentException => "usage",
        _ => "other",
    };

    private static void CheckResult(JsonElement expected, object? result)
    {
        Assert.NotNull(result);
        foreach (JsonProperty p in expected.EnumerateObject())
        {
            object? actual = Resolve(result, p.Name);
            AssertValue(p.Value, actual, p.Name);
        }
    }

    private static object? Resolve(object root, string path)
    {
        var metadata = ((ApiResult)root).Metadata;
        if (path == "revalidated")
        {
            return metadata.Revalidated;
        }

        if (path.StartsWith("rate_limit.", StringComparison.Ordinal))
        {
            RateLimitState? rl = metadata.RateLimit;
            return path["rate_limit.".Length..] switch
            {
                "limit" => rl?.Limit,
                "remaining" => rl?.Remaining,
                "reset" => rl?.Reset?.TotalSeconds,
                "policy" => rl?.Policy,
                string other => throw new InvalidOperationException($"Unknown rate_limit field {other}."),
            };
        }

        object? current = root;
        foreach (Match m in Regex.Matches(path, @"([a-z0-9_]+)|\[(\d+)\]"))
        {
            if (current is null)
            {
                return null;
            }

            current = m.Groups[1].Success
                ? Get(current, m.Groups[1].Value)
                : ((IList)current)[int.Parse(m.Groups[2].Value, CultureInfo.InvariantCulture)];
        }

        return current;
    }

    /// <summary>Reads a property by its JSON name, the one per-language mapping of the result paths.</summary>
    internal static object? Get(object target, string jsonName)
    {
        PropertyInfo? property = target.GetType().GetProperties()
            .FirstOrDefault(p => p.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name == jsonName);
        return property is null
            ? throw new InvalidOperationException($"{target.GetType().Name} has no property for '{jsonName}'.")
            : property.GetValue(target);
    }

    private static void AssertValue(JsonElement expected, object? actual, string path)
    {
        switch (expected.ValueKind)
        {
            case JsonValueKind.Null:
                Assert.True(actual is null, $"{path}: expected absent, got {actual}");
                break;
            case JsonValueKind.Number:
                Assert.NotNull(actual);
                Assert.Equal(expected.GetDouble(), Convert.ToDouble(actual, CultureInfo.InvariantCulture), 9);
                break;
            case JsonValueKind.True or JsonValueKind.False:
                Assert.Equal(expected.GetBoolean(), actual);
                break;
            case JsonValueKind.String:
                Assert.Equal(expected.GetString(), actual?.ToString());
                break;
            case JsonValueKind.Object when expected.TryGetProperty("unknown", out JsonElement raw):
                Assert.NotNull(actual);
                Assert.Equal(raw.GetString(), actual.ToString());
                Assert.False((bool)actual.GetType().GetProperty("IsKnown")!.GetValue(actual)!, $"{path}: expected the unknown case");
                break;
            default:
                throw new InvalidOperationException($"{path}: unsupported expectation {expected}.");
        }
    }
}
