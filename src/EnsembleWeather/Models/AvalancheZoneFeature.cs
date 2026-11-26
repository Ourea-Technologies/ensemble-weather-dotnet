// SPDX-License-Identifier: BSD-2-Clause

using System.Text.Json.Serialization;

namespace EnsembleWeather;

/// <summary>
/// One forecast zone, a GeoJSON <c>Feature</c>. GeoJSON: every key is the wire's, unchanged (BEHAVIOR.md 3.0).
/// </summary>
public sealed class AvalancheZoneFeature
{
    /// <summary>
    /// One of <c>Feature</c>.
    /// </summary>
    [JsonPropertyName("type")]
    public string Type { get; init; } = string.Empty;

    /// <summary>
    /// The zone's id, as <c>properties.zone_id</c>.
    /// </summary>
    [JsonPropertyName("id")]
    public long Id { get; init; }

    /// <summary>
    /// A GeoJSON <c>MultiPolygon</c>: an array of polygons, each an array of rings (the outer one first, then any
    /// holes), each ring closed. A position is <c>[longitude, latitude]</c> in degrees (WGS84) with at most four
    /// decimals.
    /// </summary>
    [JsonPropertyName("geometry")]
    public AvalancheZoneGeometry Geometry { get; init; } = null!;

    /// <summary>
    /// The avalanche center's danger rating for the forecast zone the location's point is in, from avalanche.org's
    /// public map layer. Link-first: the danger level, the zone and center names, and a link to the center's full
    /// forecast, which is the thing to read -- Ourea never serves the forecast's text and never rates danger
    /// itself. Absent when the feature is off on this server or the point is in no avalanche center's zone. Fail
    /// closed: when the zone has not been seen in a successful poll recently, or its product has expired
    /// (<c>valid_until</c> passed), <c>status</c> is <c>unavailable</c> and <c>danger_level</c> is <c>null</c> --
    /// an old rating is never served. Show <c>attribution</c> with the block. Colorado (additive): where this
    /// server reads CAIC's own feed, a Colorado point gets its CAIC forecast area's rating instead of
    /// avalanche.org's single statewide CAIC zone. The level is the HIGHEST of CAIC's three elevation bands (above,
    /// near and below treeline) for the current day, the convention of the avalanche.org map. CAIC regroups its
    /// areas as conditions change, so the area's <c>zone_id</c> can change from day to day, and <c>forecast_url</c>
    /// opens CAIC's map at the point. If CAIC's feed is stale or down, a Colorado point gets <c>unavailable</c>,
    /// never the statewide rating. A point in an area where CAIC issues no forecast gets <c>no_rating</c>. CAIC's
    /// areas have no names, so <c>zone_name</c> is <c>CAIC forecast area</c>. The Cap'n Proto body carries the same
    /// as <c>ForecastResponse.avalanche</c> (set when <c>hasAvalanche</c>).
    /// </summary>
    [JsonPropertyName("properties")]
    public AvalancheZoneProperties Properties { get; init; } = null!;
}
