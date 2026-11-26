// SPDX-License-Identifier: BSD-2-Clause

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using EnsembleWeather.Serialization;
using EnsembleWeather.Tests.Support;

namespace EnsembleWeather.Tests;

/// <summary>The 0.1.0 routes' argument rules and the names they decode to, beyond what the shared vectors pin.</summary>
public sealed class ContractTests
{
    private static string Body(string name) => File.ReadAllText(Path.Combine(RepoPaths.Vectors, "bodies", name));

    private static (OureaClient Client, MockHandler Handler) Client(string body, string? apiKey = null)
    {
        var handler = new MockHandler(responder: _ => MockHandler.Json(200, body));
        var options = new OureaClientOptions { ApiKey = apiKey, Environment = _ => null, EnableCache = false };
        return (new OureaClient(new HttpClient(handler), options), handler);
    }

    private static string Query(MockHandler handler) => handler.Requests[^1].Uri.Query.TrimStart('?');

    [Theory]
    [InlineData(91, 0)]
    [InlineData(-90.5, 0)]
    [InlineData(0, 180.01)]
    [InlineData(double.NaN, 0)]
    [InlineData(0, double.PositiveInfinity)]
    public async Task ANearPointOutOfRangeIsRefusedBeforeSending(double lat, double lon)
    {
        (OureaClient client, MockHandler handler) = Client(Body("explore.json"));

        await Assert.ThrowsAsync<ArgumentException>(
            () => client.ExplorePageAsync(ExploreMetric.Snow5d, new ExploreOptions { Near = new GeoPoint(lat, lon) }));
        await Assert.ThrowsAsync<ArgumentException>(
            () => client.SearchLocationsAsync("hood", new SearchOptions { Near = new GeoPoint(lat, lon) }));
        Assert.Empty(handler.Requests);
    }

    [Theory]
    [InlineData(-116, 41, -125, 49)] // west > east
    [InlineData(-125, 49, -116, 41)] // south > north
    [InlineData(-181, 41, -116, 49)]
    [InlineData(-125, 41, -116, 91)]
    public async Task ABadBoundingBoxIsRefusedBeforeSending(double west, double south, double east, double north)
    {
        (OureaClient client, MockHandler handler) = Client(Body("alert-outlines.json"));

        await Assert.ThrowsAsync<ArgumentException>(
            () => client.GetAlertOutlinesAsync(new AlertOutlineOptions { BoundingBox = new BoundingBox(west, south, east, north) }));
        Assert.Empty(handler.Requests);
    }

    [Fact]
    public async Task NearAndBboxAreShortestNumbersWithLiteralCommas()
    {
        (OureaClient client, MockHandler handler) = Client(Body("alert-outlines.json"));

        await client.GetAlertOutlinesAsync(new AlertOutlineOptions { BoundingBox = new BoundingBox(-124.5, 41.25, -116, 49.000001) });
        Assert.Equal("bbox=-124.5,41.25,-116,49.000001", Query(handler));

        (client, handler) = Client(Body("explore.json"));
        await client.ExplorePageAsync(ExploreMetric.BaseDepth, new ExploreOptions { Near = new GeoPoint(0.1 + 0.2, -121) });
        Assert.Equal("metric=base_depth&near=0.30000000000000004,-121", Query(handler));
    }

    [Theory]
    [InlineData("true", "members=true")]
    [InlineData("plume", "members=plume")]
    [InlineData("false", "")]
    public async Task MembersTakesTrueFalseOrPlume(string value, string query)
    {
        (OureaClient client, MockHandler handler) = Client(Body("forecast-metric.json"));

        await client.GetLocationForecastAsync("or/mt-hood-meadows", new ForecastOptions { Members = ForecastMembers.Parse(value) });

        Assert.Equal(query, Query(handler));
    }

    [Fact]
    public async Task MembersAcceptsABoolean()
    {
        (OureaClient client, MockHandler handler) = Client(Body("forecast-metric.json"));

        await client.GetLocationForecastAsync("or/mt-hood-meadows", new ForecastOptions { Members = true });

        Assert.Equal("members=true", Query(handler));
    }

    [Theory]
    [InlineData("pool")]
    [InlineData("True")]
    [InlineData("")]
    public void AnyOtherMembersStringIsAUsageError(string value)
        => Assert.Throws<ArgumentException>(() => ForecastMembers.Parse(value));

    [Fact]
    public async Task ThePlumeViewDecodesToColumns()
    {
        (OureaClient client, _) = Client(Body("forecast-plume.json"));

        ForecastResponse forecast = await client.GetLocationForecastAsync(
            "or/mt-hood-meadows", new ForecastOptions { Members = ForecastMembers.Plume });

        MemberPlume plume = Assert.IsType<MemberPlume>(forecast.Series.Plume);
        Assert.NotEmpty(plume.Models);
        Assert.Equal(plume.Bands.ValidTime.Count, plume.Bands.CumulativeSnowfall.P90.Count);
        Assert.Empty(forecast.Series.Timeseries);
    }

    [Fact]
    public async Task SearchSendsTheClientsUnitsAndOmitsItsDefaults()
    {
        (OureaClient client, MockHandler handler) = Client(Body("search-sorted.json"));
        var imperial = new OureaClient(
            new HttpClient(handler), new OureaClientOptions { Units = UnitSystem.Imperial, Environment = _ => null, EnableCache = false });

        await imperial.SearchLocationsAsync(
            "hood", new SearchOptions { Sort = ExploreMetric.Snow5d, Order = SortOrder.Descending, RadiusKm = 100 });
        Assert.Equal("q=hood&sort=snow_5d&units=imperial", Query(handler));

        await client.SearchLocationsAsync("hood", new SearchOptions { Order = SortOrder.Ascending, RadiusKm = 50 });
        Assert.Equal("order=asc&q=hood&radius_km=50", Query(handler));
    }

    [Fact]
    public async Task SortedHitsCarryTheirExploreValue()
    {
        (OureaClient client, _) = Client(Body("search-sorted.json"));

        SearchLocationsResponse hits = await client.SearchLocationsAsync("hood", new SearchOptions { Sort = ExploreMetric.Snow5d });

        Assert.Equal("or-mt-hood-meadows", hits.Items[0].Key);
        Assert.Equal("snow_5d", hits.Items[0].Explore?.Metric);
    }

    [Fact]
    public void AHitsExploreValueMayBeNull()
    {
        JsonNode body = JsonNode.Parse(Body("search-sorted.json"))!;
        body["items"]![0]!["explore"] = null;

        SearchLocationsResponse hits = JsonSerializer.Deserialize(body.ToJsonString(), EnsembleWeatherJsonContext.Default.SearchLocationsResponse)!;

        Assert.Null(hits.Items[0].Explore);
    }

    [Fact]
    public async Task ExploreFollowsTheCursorLazily()
    {
        string first = Body("explore.json");
        JsonNode last = JsonNode.Parse(first)!;
        last["next_cursor"] = null;
        var handler = new MockHandler([MockHandler.Json(200, first), MockHandler.Json(200, last.ToJsonString())]);
        var client = new OureaClient(new HttpClient(handler), new OureaClientOptions { Environment = _ => null, EnableCache = false });

        var items = new List<ExploreItem>();
        await foreach (ExploreItem item in client.ExploreAsync(ExploreMetric.Snow5d, new ExploreOptions { State = "WA" }))
        {
            items.Add(item);
        }

        Assert.Equal(2, handler.Requests.Count);
        Assert.Equal(4, items.Count);
        Assert.Contains("state=wa", handler.Requests[1].Uri.Query, StringComparison.Ordinal);
        Assert.Contains("cursor=", handler.Requests[1].Uri.Query, StringComparison.Ordinal);
    }

    [Fact]
    public async Task PlusRequiredIsAPermissionError()
    {
        var handler = new MockHandler([MockHandler.Json(403, """{"error":"PLUS_REQUIRED","message":"The snow trend needs Plus or Pro."}""")]);
        var client = new OureaClient(new HttpClient(handler), new OureaClientOptions { Environment = _ => null });

        PermissionDeniedException ex = await Assert.ThrowsAsync<PermissionDeniedException>(() => client.GetSnowTrendAsync("or/mt-hood-meadows"));

        Assert.Equal("PLUS_REQUIRED", ex.ErrorCode);
    }

    [Fact]
    public async Task SnowpackDefinitionsKeepTheirKeysAsSent()
    {
        (OureaClient client, _) = Client(Body("snowpack.json"));

        SnowpackResponse snowpack = await client.GetSnowpackAsync("or/mt-hood-meadows");

        JsonNode raw = JsonNode.Parse(Body("snowpack.json"))!["definitions"]!;
        Assert.Equal(raw["surface"]!.AsObject().Select(p => p.Key).Order(), snowpack.Definitions.Surface.Keys.Order());
        Assert.Equal(raw["layer_form"]!.AsObject().Select(p => p.Key).Order(), snowpack.Definitions.LayerForm.Keys.Order());
        Assert.Equal((string?)raw["layer_form"]!["buried_surface_hoar"], snowpack.Definitions.LayerForm["buried_surface_hoar"]);
    }

    [Fact]
    public async Task SnowTrendDaysCanBeNull()
    {
        (OureaClient client, _) = Client(Body("snow-trend.json"));

        SnowTrendResponse trend = await client.GetSnowTrendAsync("or/mt-hood-meadows");

        int nulls = JsonNode.Parse(Body("snow-trend.json"))!["grids"]!.AsArray()
            .SelectMany(g => g!["runs"]!.AsArray()).SelectMany(r => r!["days"]!.AsArray()).Count(d => d is null);
        Assert.Equal(nulls, trend.Grids.SelectMany(g => g.Runs).SelectMany(r => r.Days).Count(d => d is null));
        Assert.Equal(trend.Dates.Count, trend.Grids[0].Runs[0].Days.Count);
    }

    [Theory]
    [InlineData("alert-outlines.json", typeof(AlertOutlineCollection))]
    [InlineData("avalanche-zones.json", typeof(AvalancheZoneCollection))]
    public void GeoJsonRoundTripsWithEveryWireKey(string file, Type type)
    {
        JsonNode original = JsonNode.Parse(Body(file))!;
        var info = EnsembleWeatherJsonContext.Default.GetTypeInfo(type)!;

        object model = JsonSerializer.Deserialize(original.ToJsonString(), info)!;
        JsonNode written = JsonNode.Parse(JsonSerializer.Serialize(model, info))!;

        foreach (JsonNode? feature in original["features"]!.AsArray())
        {
            JsonNode copy = written["features"]![original["features"]!.AsArray().IndexOf(feature)]!;
            Assert.Equal(feature!["geometry"]!["coordinates"]!.ToJsonString(), copy["geometry"]!["coordinates"]!.ToJsonString());
            foreach (KeyValuePair<string, JsonNode?> property in feature["properties"]!.AsObject().Where(p => p.Value is not null))
            {
                Assert.True(copy["properties"]![property.Key] is not null, $"{file}: properties.{property.Key} was not written back");
            }
        }
    }

    /// <summary>
    /// A list the server omits (the snowpack plume's <c>models</c> for an anonymous caller, <c>timeseries</c> with
    /// <c>members=plume</c>) reads as empty, never <see langword="null"/>, on every model; only a list the spec makes
    /// nullable is declared nullable.
    /// </summary>
    [Fact]
    public void AnOmittedListReadsAsEmptyOnEveryModel()
    {
        var checkedTypes = 0;
        foreach (Type type in typeof(OureaClient).Assembly.GetExportedTypes().Where(t => t.IsClass && !t.IsAbstract && t.GetConstructor(Type.EmptyTypes) is not null))
        {
            System.Text.Json.Serialization.Metadata.JsonTypeInfo? info;
            try
            {
                info = EnsembleWeatherJsonContext.Default.GetTypeInfo(type);
            }
            catch (InvalidOperationException)
            {
                continue;
            }

            if (info is null || info.Kind != System.Text.Json.Serialization.Metadata.JsonTypeInfoKind.Object)
            {
                continue;
            }

            object model = JsonSerializer.Deserialize("{}", info)!;
            var nullability = new System.Reflection.NullabilityInfoContext();
            foreach (var property in type.GetProperties().Where(p => p.PropertyType.IsGenericType
                && p.PropertyType.GetGenericTypeDefinition() == typeof(IReadOnlyList<>)
                && nullability.Create(p).ReadState != System.Reflection.NullabilityState.Nullable))
            {
                Assert.True(property.GetValue(model) is not null, $"{type.Name}.{property.Name} is null when omitted");
            }

            checkedTypes++;
        }

        Assert.True(checkedTypes > 100, $"only {checkedTypes} model types were checked");
    }

    [Fact]
    public async Task CompositionIsSentWithoutTheKey()
    {
        (OureaClient client, MockHandler handler) = Client(Body("composition.json"), apiKey: "ow_pro");

        CompositionResponse composition = await client.GetCompositionAsync();

        Assert.Null(handler.Requests[0].Header("X-API-Key"));
        Assert.Equal(5, composition.ModelCount);
        Assert.NotEmpty(composition.Models);
    }

    [Fact]
    public async Task SeasonIncludesTheDailyRecordOnlyWhenAsked()
    {
        (OureaClient client, MockHandler handler) = Client(Body("season.json"));

        await client.GetSeasonAsync("or/mt-hood-meadows");
        Assert.Equal(string.Empty, Query(handler));

        SeasonResponse season = await client.GetSeasonAsync("or/mt-hood-meadows", new SeasonOptions { IncludeDaily = true, Units = UnitSystem.Imperial });
        Assert.Equal("include=daily&units=imperial", Query(handler));
        Assert.NotEmpty(season.Daily);
    }
}
