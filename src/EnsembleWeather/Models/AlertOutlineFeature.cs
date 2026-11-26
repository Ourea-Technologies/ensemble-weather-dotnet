// SPDX-License-Identifier: BSD-2-Clause

using System.Text.Json.Serialization;

namespace EnsembleWeather;

/// <summary>
/// One alert's outline, a GeoJSON <c>Feature</c>. GeoJSON: every key is the wire's, unchanged (BEHAVIOR.md 3.0).
/// </summary>
public sealed class AlertOutlineFeature
{
    /// <summary>
    /// One of <c>Feature</c>.
    /// </summary>
    [JsonPropertyName("type")]
    public string Type { get; init; } = string.Empty;

    /// <summary>
    /// A GeoJSON <c>Polygon</c> or <c>MultiPolygon</c>. A position is <c>[longitude, latitude]</c> in degrees
    /// (WGS84) with at most three decimals; every ring is closed (its first position repeated last). A
    /// <c>Polygon</c>'s <c>coordinates</c> is an array of rings (the outer one first, then any holes); a
    /// <c>MultiPolygon</c>'s is an array of such polygons.
    /// </summary>
    [JsonPropertyName("geometry")]
    public AlertOutlineGeometry Geometry { get; init; } = null!;

    /// <summary>
    /// The alert, under the names <c>ActiveAlert</c> uses for the same fields, and its <c>class</c>. The full text
    /// is on the alerts routes.
    /// </summary>
    [JsonPropertyName("properties")]
    public AlertOutlineProperties Properties { get; init; } = null!;
}
