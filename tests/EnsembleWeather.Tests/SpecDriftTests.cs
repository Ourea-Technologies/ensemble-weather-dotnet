// SPDX-License-Identifier: BSD-2-Clause

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using System.Text.RegularExpressions;
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
        ["getCurrentByCoordinates"] = ["GetPointCurrentAsync"],
        ["getLocationAlerts"] = ["GetLocationAlertsAsync"],
        ["getAlertsByCoordinates"] = ["GetPointAlertsAsync"],
        ["getNearbyLocations"] = ["GetNearbyLocationsAsync"],
        ["getNearbyByCoordinates"] = ["GetPointNearbyAsync"],
        ["searchLocations"] = ["SearchLocationsAsync"],
        ["getAllLocations"] = ["GetAllLocationsPageAsync", "GetAllLocationsAsync"],
        ["getLocations"] = ["GetLocationsPageAsync", "GetLocationsAsync"],
        ["listCatalogLocations"] = ["GetCatalogLocationsPageAsync", "GetCatalogLocationsAsync"],
        ["getCatalogLocation"] = ["GetCatalogLocationAsync"],
        ["listCatalogStations"] = ["GetCatalogStationsPageAsync", "GetCatalogStationsAsync"],
        ["getCatalogStation"] = ["GetCatalogStationAsync"],
        ["getAvailableRuns"] = ["GetAvailableRunsAsync"],
        ["getAttribution"] = ["GetAttributionAsync"],
        ["explore"] = ["ExplorePageAsync", "ExploreAsync"],
        ["getComposition"] = ["GetCompositionAsync"],
        ["getLocationSeason"] = ["GetSeasonAsync"],
        ["getLocationSnowTrend"] = ["GetSnowTrendAsync"],
        ["getLocationSnowpack"] = ["GetSnowpackAsync"],
        ["getLocationSnowpackPlume"] = ["GetSnowpackPlumeAsync"],
        ["getAlertOutlines"] = ["GetAlertOutlinesAsync"],
        ["getAvalancheZones"] = ["GetAvalancheZonesAsync"],
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
        ["AttributionRegistry"] = typeof(AttributionRegistry),
        ["ExploreResponse"] = typeof(ExploreResponse),
        ["CompositionResponse"] = typeof(CompositionResponse),
        ["SeasonResponse"] = typeof(SeasonResponse),
        ["SnowTrendResponse"] = typeof(SnowTrendResponse),
        ["SnowpackResponse"] = typeof(SnowpackResponse),
        ["SnowpackPlumeResponse"] = typeof(SnowpackPlumeResponse),
        ["AlertOutlineCollection"] = typeof(AlertOutlineCollection),
        ["AvalancheZoneCollection"] = typeof(AvalancheZoneCollection),
    };

    /// <summary>The GeoJSON bodies: passed through untouched, so no rename applies inside them (BEHAVIOR.md 3.0).</summary>
    private static readonly HashSet<string> GeoJson = ["AlertOutlineCollection", "AvalancheZoneCollection"];

    /// <summary>
    /// Inline objects the rename table names by their SDK type (BEHAVIOR.md 3.0 lists them as "Schema.field").
    /// </summary>
    private static readonly Dictionary<string, string> InlineHolders = new()
    {
        ["CurrentConditions.station"] = "CurrentStation",
        ["TimeSeriesPoint.members"] = "MemberGroup",
        ["SnowTrendResponse.elevation"] = "SnowTrendResponseElevation",
    };

    /// <summary>
    /// Fields whose .NET name spells out the wire name (an idiom kept from before the rename table, never a rename
    /// for taste): the wire name -> the property name.
    /// </summary>
    private static readonly Dictionary<string, string> SpelledOut = new()
    {
        ["lat"] = "Latitude",
        ["lon"] = "Longitude",
        ["requested_lat"] = "RequestedLatitude",
        ["requested_lon"] = "RequestedLongitude",
        ["lat_override"] = "LatitudeOverride",
        ["lon_override"] = "LongitudeOverride",
        ["default"] = "IsDefault",
        ["astro"] = "Astronomy",
        ["spec"] = "Sections",
        ["std"] = "StandardDeviation",
        ["stddev"] = "StandardDeviation",
        ["op"] = "Operator",
        ["mslp"] = "MeanSeaLevelPressure",
        ["pressure_diff_hpa"] = "PressureDifferenceHpa",
        ["distance_km"] = "DistanceKilometers",
        ["elevation_diff"] = "ElevationDifference",
        ["elevation_diff_m"] = "ElevationDifferenceMeters",
        ["overnight_4pm_9am"] = "Overnight4PmTo9Am",
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

        var methods = typeof(OureaClient).GetMethods(BindingFlags.Public | BindingFlags.Instance)
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

                string? reference = (string?)Content(method.Value)?["schema"]?["$ref"];
                Assert.NotNull(reference);
                Assert.True(Models.ContainsKey(reference!.Split('/')[^1]), $"{id} returns {reference}, which has no model.");
            }
        }
    }

    /// <summary>
    /// Every field of every schema an operation returns has a model property, reached through the models
    /// themselves; renamed fields carry the SDK name of the BEHAVIOR.md 3.0 table (read from the pinned spec, so a
    /// new row is enforced here), every other field its wire name in .NET case, and every row of the table is
    /// applied. GeoJSON bodies keep every wire name.
    /// </summary>
    [Fact]
    public void EveryFieldIsModelledUnderItsSdkName()
    {
        var rows = RenameTable();
        Assert.True(rows.Count >= 40, $"The BEHAVIOR.md 3.0 table has {rows.Count} rows; the parser found too few.");
        var byHolder = rows.ToLookup(r => r.Holder, StringComparer.Ordinal);
        var applied = new HashSet<(string, string)>();
        var failures = new List<string>();
        var visited = new HashSet<string>(StringComparer.Ordinal);
        foreach ((string schema, Type type) in Models)
        {
            JsonTypeInfo info = EnsembleWeatherJsonContext.Default.GetTypeInfo(type)!;
            WalkFields(Schemas[schema]!, schema, info, GeoJson.Contains(schema), byHolder, applied, failures, visited);
        }

        foreach ((string holder, string wire, _) in rows.Where(r => r.Holder != "ErrorResponse"))
        {
            if (!applied.Contains((holder, wire)))
            {
                failures.Add($"rename {holder}.{wire} is never applied (no model reaches it)");
            }
        }

        Assert.True(failures.Count == 0, string.Join("\n", failures));
    }

    private static void WalkFields(
        JsonNode schema,
        string holder,
        JsonTypeInfo info,
        bool geo,
        ILookup<string, (string Holder, string Wire, string SdkName)> renames,
        HashSet<(string, string)> applied,
        List<string> failures,
        HashSet<string> visited)
    {
        if (!visited.Add(holder + "|" + info.Type.FullName + "|" + geo))
        {
            return;
        }

        string[] holders = holder == "PublicCatalogStationDetail" ? [holder, "PublicCatalogStation"] : [holder];
        foreach ((string wire, JsonNode? property) in Properties(schema))
        {
            JsonPropertyInfo? field = info.Properties.FirstOrDefault(p => p.Name == wire);
            if (field is null)
            {
                failures.Add($"{holder}.{wire} has no field on {info.Type.Name}");
                continue;
            }

            string clrName = (field.AttributeProvider as MemberInfo)?.Name
                ?? info.Type.GetProperties().First(p => p.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name == wire).Name;
            var row = geo ? default : holders.SelectMany(h => renames[h]).FirstOrDefault(r => r.Wire == wire);
            if (row.SdkName is not null)
            {
                applied.Add((row.Holder, wire));
                if (clrName != DotNetName(row.SdkName))
                {
                    failures.Add($"{holder}.{wire} is {info.Type.Name}.{clrName}; the rename table says {DotNetName(row.SdkName)}");
                }
            }
            else if (clrName != DotNetName(wire) && !(SpelledOut.TryGetValue(wire, out string? spelled) && spelled == clrName))
            {
                failures.Add($"{holder}.{wire} is {info.Type.Name}.{clrName}, a rename the table does not list");
            }

            (JsonNode? target, string? name) = Target(property!);
            if (target is null)
            {
                continue;
            }

            Type element = ElementType(field.PropertyType);
            if (element == typeof(JsonElement))
            {
                continue;
            }

            string next = name ?? (InlineHolders.TryGetValue(holder + "." + wire, out string? inline) ? inline : holder + "." + wire);
            bool nextGeo = geo || GeoJson.Contains(next);
            WalkFields(target, next, info.Options.GetTypeInfo(element), nextGeo, renames, applied, failures, visited);
        }
    }

    /// <summary>The object schema a property holds (through null, allOf and arrays), and its name if it is a component.</summary>
    private static (JsonNode? Schema, string? Name) Target(JsonNode property)
    {
        JsonNode node = property;
        string? name = null;
        for (int guard = 0; guard < 16; guard++)
        {
            if ((string?)node["$ref"] is string reference)
            {
                name = reference.Split('/')[^1];
                node = Schemas[name]!;
                continue;
            }

            JsonArray? alternatives = (node["allOf"] ?? node["oneOf"] ?? node["anyOf"]) as JsonArray;
            if (node["properties"] is null && alternatives is not null)
            {
                var concrete = alternatives.Where(a => (string?)a!["type"] != "null").ToList();
                if (concrete.Count == 1)
                {
                    node = concrete[0]!;
                    continue;
                }
            }

            if (node["items"] is JsonNode items)
            {
                node = items;
                continue;
            }

            if (node["additionalProperties"] is JsonObject values)
            {
                node = values;
                continue;
            }

            break;
        }

        return Properties(node).Any() ? (node, name) : (null, null);
    }

    /// <summary>An object schema's properties, its <c>allOf</c> parts flattened.</summary>
    private static IEnumerable<(string Name, JsonNode? Schema)> Properties(JsonNode schema)
    {
        if (schema["allOf"] is JsonArray parts)
        {
            foreach (JsonNode? part in parts)
            {
                JsonNode resolved = (string?)part!["$ref"] is string r ? Schemas[r.Split('/')[^1]]! : part;
                foreach (var p in Properties(resolved))
                {
                    yield return p;
                }
            }
        }

        if (schema["properties"] is JsonObject properties)
        {
            foreach (KeyValuePair<string, JsonNode?> p in properties)
            {
                yield return (p.Key, p.Value);
            }
        }
    }

    private static Type ElementType(Type type)
    {
        while (true)
        {
            Type? nullable = Nullable.GetUnderlyingType(type);
            if (nullable is not null)
            {
                type = nullable;
                continue;
            }

            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IReadOnlyList<>))
            {
                type = type.GetGenericArguments()[0];
                continue;
            }

            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IReadOnlyDictionary<,>))
            {
                type = type.GetGenericArguments()[1];
                continue;
            }

            return type;
        }
    }

    /// <summary>The .NET property name for an SDK (or wire) name: Pascal case, units spelled out.</summary>
    internal static string DotNetName(string sdkName)
    {
        var words = new List<string>();
        string[] parts = sdkName.Split('_');
        for (int i = 0; i < parts.Length; i++)
        {
            string w = parts[i];
            if (i == parts.Length - 1 && w == "m" && parts.Length > 1)
            {
                words.Add("Meters");
                continue;
            }

            Match unit = Regex.Match(w, "^([0-9]+)(h|d)$");
            Match count = Regex.Match(w, "^([0-9]+)(in|mph|to)([0-9]*)$");
            words.Add(w switch
            {
                "slr" => "SnowToLiquidRatio",
                "swe" => "SnowWaterEquivalent",
                _ when unit.Success => unit.Groups[1].Value + (unit.Groups[2].Value == "h" ? "Hours" : "Days"),
                _ when count.Success => count.Groups[1].Value + char.ToUpperInvariant(count.Groups[2].Value[0]) + count.Groups[2].Value[1..] + count.Groups[3].Value,
                _ => w.Length == 0 ? w : char.ToUpperInvariant(w[0]) + w[1..],
            });
        }

        return string.Concat(words);
    }

    /// <summary>The BEHAVIOR.md 3.0 rename table, one row per field: (holder schema, wire name, SDK name).</summary>
    internal static List<(string Holder, string Wire, string SdkName)> RenameTable()
    {
        string text = File.ReadAllText(Path.Combine(RepoPaths.Spec, "BEHAVIOR.md"));
        int start = text.IndexOf("### 3.0", StringComparison.Ordinal);
        int end = text.IndexOf("### 3.1", start, StringComparison.Ordinal);
        var rows = new List<(string, string, string)>();
        foreach (string line in text[start..end].Split('\n'))
        {
            string[] cells = line.Split('|');
            if (cells.Length < 5 || !line.StartsWith("| `", StringComparison.Ordinal))
            {
                continue;
            }

            string[] wires = Ticks(cells[2]);
            string[] sdks = Ticks(cells[3]);
            foreach (string holderCell in Ticks(cells[1]))
            {
                string holder = holderCell.Replace("[]", string.Empty, StringComparison.Ordinal);
                holder = holder switch
                {
                    "CurrentConditions.station" => "CurrentStation",
                    "TimeSeriesPoint.members" => "MemberGroup",
                    "SnowTrendResponse.elevation" => "SnowTrendResponseElevation",
                    "PublicCatalogStationDetail" => "PublicCatalogStation",
                    _ => holder,
                };
                for (int i = 0; i < wires.Length; i++)
                {
                    rows.Add((holder, wires[i], sdks[i]));
                }
            }
        }

        return rows.Distinct().ToList();
    }

    private static string[] Ticks(string cell)
        => Regex.Matches(cell, "`([^`]+)`").Select(m => m.Groups[1].Value).ToArray();

    private static JsonObject Schemas => OpenApi.Value["components"]!["schemas"]!.AsObject();

    /// <summary>The 200 response's body (JSON, or GeoJSON for the two map-layer routes).</summary>
    private static JsonNode? Content(JsonNode operation)
    {
        JsonNode? content = operation["responses"]?["200"]?["content"];
        return content?["application/json"] ?? content?["application/geo+json"];
    }

    private static readonly Lazy<JsonNode> OpenApi = new(() => YamlToJson(File.ReadAllText(RepoPaths.OpenApi)));

    private static IEnumerable<(string Operation, string Example, string Schema, JsonNode Value)> ResponseExamples()
    {
        foreach (KeyValuePair<string, JsonNode?> path in OpenApi.Value["paths"]!.AsObject())
        {
            foreach (KeyValuePair<string, JsonNode?> method in path.Value!.AsObject())
            {
                string? id = (string?)method.Value!["operationId"];
                JsonNode? content = Content(method.Value);
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
