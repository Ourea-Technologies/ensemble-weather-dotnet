// SPDX-License-Identifier: BSD-2-Clause

using System.Text.Json;
using System.Text.Json.Serialization;

namespace EnsembleWeather;

/// <summary>
/// A GeoJSON <c>MultiPolygon</c>: an array of polygons, each an array of rings (the outer one first, then any
/// holes), each ring closed. A position is <c>[longitude, latitude]</c> in degrees (WGS84) with at most four
/// decimals. GeoJSON: every key is the wire's, unchanged (BEHAVIOR.md 3.0).
/// </summary>
public sealed class AvalancheZoneGeometry
{
    /// <summary>
    /// One of <c>MultiPolygon</c>.
    /// </summary>
    [JsonPropertyName("type")]
    public string Type { get; init; } = string.Empty;

    /// <summary>
    /// The positions as sent: nested arrays of <c>[longitude, latitude]</c>, never reprojected or simplified.
    /// </summary>
    [JsonPropertyName("coordinates")]
    public JsonElement Coordinates { get; init; }
}
