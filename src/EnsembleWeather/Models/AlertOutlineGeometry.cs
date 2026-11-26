// SPDX-License-Identifier: BSD-2-Clause

using System.Text.Json;
using System.Text.Json.Serialization;

namespace EnsembleWeather;

/// <summary>
/// A GeoJSON <c>Polygon</c> or <c>MultiPolygon</c>. A position is <c>[longitude, latitude]</c> in degrees (WGS84)
/// with at most three decimals; every ring is closed (its first position repeated last). A <c>Polygon</c>'s
/// <c>coordinates</c> is an array of rings (the outer one first, then any holes); a <c>MultiPolygon</c>'s is an
/// array of such polygons. GeoJSON: every key is the wire's, unchanged (BEHAVIOR.md 3.0).
/// </summary>
public sealed class AlertOutlineGeometry
{
    /// <summary>
    /// One of <c>Polygon</c>, <c>MultiPolygon</c>.
    /// </summary>
    [JsonPropertyName("type")]
    public string Type { get; init; } = string.Empty;

    /// <summary>
    /// The positions as sent: nested arrays of <c>[longitude, latitude]</c>, never reprojected or simplified.
    /// </summary>
    [JsonPropertyName("coordinates")]
    public JsonElement Coordinates { get; init; }
}
