// SPDX-License-Identifier: BSD-2-Clause

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization.Metadata;
using EnsembleWeather.Serialization;
using EnsembleWeather.Tests.Support;
using YamlDotNet.Core;
using YamlDotNet.RepresentationModel;

namespace EnsembleWeather.Tests;

/// <summary>
/// Spec drift (SDK-STANDARD "Tests"): every operation in the pinned OpenAPI spec is exposed, and the models
/// round-trip every JSON response example in it.
/// </summary>
public sealed class SpecDriftTests
{
    /// <summary>operationId -> the client method(s) that expose it. getMapsToken is out of scope (BEHAVIOR.md 3).</summary>
    private static readonly Dictionary<string, string[]> Exposed = new()
    {
        ["getLocationForecast"] = ["GetLocationForecastAsync"],
        ["getPointForecast"] = ["GetPointForecastAsync"],
        ["getCurrentConditions"] = ["GetCurrentConditionsAsync"],
        ["getCurrentByCoordinates"] = ["GetCurrentByCoordinatesAsync"],
        ["getLocationAlerts"] = ["GetLocationAlertsAsync"],
        ["getAlertsByCoordinates"] = ["GetAlertsByCoordinatesAsync"],
        ["getNearbyLocations"] = ["GetNearbyLocationsAsync"],
        ["getNearbyByCoordinates"] = ["GetNearbyByCoordinatesAsync"],
        ["searchLocations"] = ["SearchLocationsAsync"],
        ["getAllLocations"] = ["GetAllLocationsPageAsync", "GetAllLocationsAsync"],
        ["getLocations"] = ["GetLocationsPageAsync", "GetLocationsAsync"],
        ["listCatalogLocations"] = ["GetCatalogLocationsPageAsync", "GetCatalogLocationsAsync"],
        ["getCatalogLocation"] = ["GetCatalogLocationAsync"],
        ["listCatalogStations"] = ["GetCatalogStationsPageAsync", "GetCatalogStationsAsync"],
        ["getCatalogStation"] = ["GetCatalogStationAsync"],
        ["getAvailableRuns"] = ["GetAvailableRunsAsync"],
        ["getStormReports"] = ["GetStormReportsAsync"],
        ["getStormReport"] = ["GetStormReportAsync"],
        ["getAttribution"] = ["GetAttributionAsync"],
    };

    private static readonly HashSet<string> OutOfScope = ["getMapsToken"];

    /// <summary>Component schema -> the model it deserialises into.</summary>
    private static readonly Dictionary<string, Type> Models = new()
    {
        ["ForecastResponse"] = typeof(ForecastResponse),
        ["CurrentResponse"] = typeof(CurrentConditionsResponse),
        ["AlertsResponse"] = typeof(AlertsResponse),
        ["NearbyLocationsResponse"] = typeof(NearbyLocationsResponse),
        ["NearbyPointResponse"] = typeof(NearbyPointResponse),
        ["SearchLocationsResponse"] = typeof(SearchLocationsResponse),
        ["LocationsResponse"] = typeof(LocationsResponse),
        ["PublicCatalogLocationPage"] = typeof(CatalogLocationPage),
        ["PublicCatalogLocationDetail"] = typeof(CatalogLocationDetail),
        ["PublicCatalogStationPage"] = typeof(CatalogStationPage),
        ["PublicCatalogStationDetail"] = typeof(CatalogStationDetail),
        ["RunsResponse"] = typeof(AvailableRunsResponse),
        ["StormReportsResponse"] = typeof(StormReportsResponse),
        ["StormReportCard"] = typeof(StormReport),
        ["AttributionRegistry"] = typeof(AttributionRegistry),
    };

    [Fact]
    public void EveryOperationInTheSpecIsExposed()
    {
        JsonNode spec = OpenApi.Value;
        var operations = spec["paths"]!.AsObject()
            .SelectMany(p => p.Value!.AsObject())
            .Select(m => (string?)m.Value!["operationId"])
            .OfType<string>()
            .ToList();
        Assert.NotEmpty(operations);

        var methods = typeof(EnsembleWeatherClient).GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Select(m => m.Name)
            .ToHashSet();
        foreach (string operation in operations.Where(o => !OutOfScope.Contains(o)))
        {
            Assert.True(Exposed.TryGetValue(operation, out string[]? names), $"The spec has '{operation}', which the SDK does not expose.");
            foreach (string name in names!)
            {
                Assert.Contains(name, methods);
            }
        }

        Assert.Empty(Exposed.Keys.Except(operations));
    }

    public static TheoryData<string, string, string> Examples()
    {
        var data = new TheoryData<string, string, string>();
        foreach ((string operation, string example, string schema, _) in ResponseExamples())
        {
            data.Add(operation, example, schema);
        }

        return data;
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void ModelsRoundTripTheSpecExample(string operation, string example, string schema)
    {
        JsonNode original = ResponseExamples().Single(e => e.Operation == operation && e.Example == example).Value;
        JsonTypeInfo info = EnsembleWeatherJsonContext.Default.GetTypeInfo(Models[schema])!;
        object model = JsonSerializer.Deserialize(original.ToJsonString(), info)!;
        JsonNode written = JsonNode.Parse(JsonSerializer.Serialize(model, info))!;
        AssertKnownFieldsSurvive(original, written, info, operation + " " + example);
    }

    [Fact]
    public void EverySchemaAnOperationReturnsHasAModel()
    {
        foreach ((string operation, _, string schema, _) in ResponseExamples())
        {
            Assert.True(Models.ContainsKey(schema), $"{operation} returns {schema}, which has no model.");
        }

        JsonNode spec = OpenApi.Value;
        foreach (KeyValuePair<string, JsonNode?> path in spec["paths"]!.AsObject())
        {
            foreach (KeyValuePair<string, JsonNode?> method in path.Value!.AsObject())
            {
                string? id = (string?)method.Value!["operationId"];
                if (id is null || OutOfScope.Contains(id))
                {
                    continue;
                }

                string? reference = (string?)method.Value["responses"]?["200"]?["content"]?["application/json"]?["schema"]?["$ref"];
                Assert.NotNull(reference);
                Assert.True(Models.ContainsKey(reference!.Split('/')[^1]), $"{id} returns {reference}, which has no model.");
            }
        }
    }

    private static readonly Lazy<JsonNode> OpenApi = new(() => YamlToJson(File.ReadAllText(RepoPaths.OpenApi)));

    private static IEnumerable<(string Operation, string Example, string Schema, JsonNode Value)> ResponseExamples()
    {
        foreach (KeyValuePair<string, JsonNode?> path in OpenApi.Value["paths"]!.AsObject())
        {
            foreach (KeyValuePair<string, JsonNode?> method in path.Value!.AsObject())
            {
                string? id = (string?)method.Value!["operationId"];
                JsonNode? content = method.Value["responses"]?["200"]?["content"]?["application/json"];
                string? reference = (string?)content?["schema"]?["$ref"];
                if (id is null || OutOfScope.Contains(id) || reference is null)
                {
                    continue;
                }

                string schema = reference.Split('/')[^1];
                if (content!["example"] is JsonNode single)
                {
                    yield return (id, "example", schema, single);
                }

                if (content["examples"] is JsonObject many)
                {
                    foreach (KeyValuePair<string, JsonNode?> e in many)
                    {
                        yield return (id, e.Key, schema, e.Value!["value"]!);
                    }
                }
            }
        }
    }

    /// <summary>Every field of the example that the model knows comes back with the same value.</summary>
    private static void AssertKnownFieldsSurvive(JsonNode? original, JsonNode? written, JsonTypeInfo info, string where)
    {
        if (original is null)
        {
            Assert.True(written is null || written.GetValueKind() == JsonValueKind.Null, $"{where}: expected null");
            return;
        }

        if (info.Kind == JsonTypeInfoKind.Object && original is JsonObject obj)
        {
            Assert.IsType<JsonObject>(written);
            foreach (KeyValuePair<string, JsonNode?> field in obj)
            {
                JsonPropertyInfo? property = info.Properties.FirstOrDefault(p => p.Name == field.Key);
                if (property is null)
                {
                    continue; // Unknown to the model (forward compatibility: ignored by design).
                }

                AssertKnownFieldsSurvive(
                    field.Value, written![field.Key], info.Options.GetTypeInfo(property.PropertyType), $"{where}.{field.Key}");
            }

            return;
        }

        if (info.Kind == JsonTypeInfoKind.Enumerable && original is JsonArray array)
        {
            JsonArray copy = Assert.IsType<JsonArray>(written);
            Assert.Equal(array.Count, copy.Count);
            Type elementType = info.Type.IsArray ? info.Type.GetElementType()! : info.Type.GetGenericArguments()[0];
            JsonTypeInfo element = info.Options.GetTypeInfo(elementType);
            for (int i = 0; i < array.Count; i++)
            {
                AssertKnownFieldsSurvive(array[i], copy[i], element, $"{where}[{i}]");
            }

            return;
        }

        Assert.NotNull(written);
        if (original.GetValueKind() == JsonValueKind.Number)
        {
            Assert.Equal(Number(original), Number(written!), 9);
        }
        else if (original.GetValueKind() == JsonValueKind.String
            && DateTimeOffset.TryParse((string?)original, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTimeOffset when)
            && ((string?)original)!.Contains('T', StringComparison.Ordinal))
        {
            Assert.Equal(when, DateTimeOffset.Parse((string)written!, CultureInfo.InvariantCulture));
        }
        else
        {
            Assert.Equal(original.ToJsonString(), written!.ToJsonString());
        }
    }

    private static double Number(JsonNode node) => double.Parse(node.ToJsonString(), CultureInfo.InvariantCulture);

    /// <summary>Converts YAML to JSON, typing plain scalars as YAML 1.2 core does (null, booleans, numbers).</summary>
    internal static JsonNode YamlToJson(string yaml)
    {
        var stream = new YamlStream();
        stream.Load(new StringReader(yaml));
        return Convert(stream.Documents[0].RootNode)!;
    }

    private static JsonNode? Convert(YamlNode node)
    {
        switch (node)
        {
            case YamlMappingNode map:
                var obj = new JsonObject();
                foreach (KeyValuePair<YamlNode, YamlNode> entry in map.Children)
                {
                    obj[((YamlScalarNode)entry.Key).Value!] = Convert(entry.Value);
                }

                return obj;
            case YamlSequenceNode seq:
                var array = new JsonArray();
                foreach (YamlNode item in seq.Children)
                {
                    array.Add(Convert(item));
                }

                return array;
            case YamlScalarNode scalar:
                string text = scalar.Value ?? string.Empty;
                if (scalar.Style is ScalarStyle.Plain)
                {
                    if (text is "null" or "~" or "")
                    {
                        return null;
                    }

                    if (text is "true" or "false")
                    {
                        return JsonValue.Create(text == "true");
                    }

                    if (long.TryParse(text, NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out long l))
                    {
                        return JsonValue.Create(l);
                    }

                    if (double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out double d))
                    {
                        return JsonValue.Create(d);
                    }
                }

                return JsonValue.Create(text);
            default:
                throw new InvalidOperationException($"Unsupported YAML node {node.NodeType}.");
        }
    }
}
