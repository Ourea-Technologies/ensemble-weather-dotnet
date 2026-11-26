// SPDX-License-Identifier: BSD-2-Clause

using System;
using System.Text.Json.Serialization;

namespace EnsembleWeather;

/// <summary>
/// The avalanche center's danger rating for the forecast zone the location's point is in, from avalanche.org's
/// public map layer. Link-first: the danger level, the zone and center names, and a link to the center's full
/// forecast, which is the thing to read -- Ourea never serves the forecast's text and never rates danger itself.
/// Absent when the feature is off on this server or the point is in no avalanche center's zone. Fail closed: when
/// the zone has not been seen in a successful poll recently, or its product has expired (<c>valid_until</c>
/// passed), <c>status</c> is <c>unavailable</c> and <c>danger_level</c> is <c>null</c> -- an old rating is never
/// served. Show <c>attribution</c> with the block. Colorado (additive): where this server reads CAIC's own feed, a
/// Colorado point gets its CAIC forecast area's rating instead of avalanche.org's single statewide CAIC zone. The
/// level is the HIGHEST of CAIC's three elevation bands (above, near and below treeline) for the current day, the
/// convention of the avalanche.org map. CAIC regroups its areas as conditions change, so the area's <c>zone_id</c>
/// can change from day to day, and <c>forecast_url</c> opens CAIC's map at the point. If CAIC's feed is stale or
/// down, a Colorado point gets <c>unavailable</c>, never the statewide rating. A point in an area where CAIC issues
/// no forecast gets <c>no_rating</c>. CAIC's areas have no names, so <c>zone_name</c> is <c>CAIC forecast area</c>.
/// The Cap'n Proto body carries the same as <c>ForecastResponse.avalanche</c> (set when <c>hasAvalanche</c>).
/// </summary>
public sealed class AvalancheForecast
{
    /// <summary>
    /// The avalanche center's short id.
    /// </summary>
    [JsonPropertyName("center_id")]
    public string CenterId { get; init; } = string.Empty;

    /// <summary>
    /// The avalanche center's name.
    /// </summary>
    [JsonPropertyName("center_name")]
    public string CenterName { get; init; } = string.Empty;

    /// <summary>
    /// avalanche.org's id for the forecast zone; for a CAIC forecast area, an id at or above 2^52 derived from
    /// CAIC's area id (stable for that grouping of CAIC's polygons).
    /// </summary>
    [JsonPropertyName("zone_id")]
    public long ZoneId { get; init; }

    /// <summary>
    /// The avalanche zone's name.
    /// </summary>
    [JsonPropertyName("zone_name")]
    public string ZoneName { get; init; } = string.Empty;

    /// <summary>
    /// The North American Public Avalanche Danger Scale level, 1-5; present only when <c>status</c> is
    /// <c>rated</c>.
    /// </summary>
    [JsonPropertyName("danger_level")]
    public int? DangerLevel { get; init; }

    /// <summary>
    /// Display text: the scale's English name for <c>danger_level</c> (Low, Moderate, Considerable, High, Extreme),
    /// derived from the level, never the center's own wording; <c>null</c> exactly when <c>danger_level</c> is.
    /// Switch on <c>danger_level</c> and <c>status</c>, show this -- like <c>condition_text</c>, it is not an enum,
    /// so it can be localised later without breaking a client.
    /// </summary>
    [JsonPropertyName("danger_label")]
    public string? DangerLabel { get; init; }

    /// <summary>
    /// <c>rated</c>: a current rating. <c>no_rating</c>: the center issued none for the zone. <c>off_season</c>:
    /// the center is out of season. <c>unavailable</c>: no rating can be vouched for (the data is stale, the
    /// product expired, or the level is not one the scale has).
    /// </summary>
    [JsonPropertyName("status")]
    public string Status { get; init; } = string.Empty;

    /// <summary>
    /// When the center's current product took effect (UTC), when stated.
    /// </summary>
    [JsonPropertyName("valid_from")]
    public DateTimeOffset? ValidFrom { get; init; }

    /// <summary>
    /// When the center's current product expires (UTC), when stated.
    /// </summary>
    [JsonPropertyName("valid_until")]
    public DateTimeOffset? ValidUntil { get; init; }

    /// <summary>
    /// (JSON <c>forecast_url</c>) The center's full forecast for the zone. Link it prominently. For a CAIC area, CAIC's forecast map at the
    /// point (<c>https://avalanche.state.co.us/?lat=..&amp;lng=..</c>).
    /// </summary>
    [JsonPropertyName("forecast_url")]
    public string Url { get; init; } = string.Empty;

    /// <summary>
    /// The avalanche center's home page.
    /// </summary>
    [JsonPropertyName("center_url")]
    public string CenterUrl { get; init; } = string.Empty;

    /// <summary>
    /// The credit to show with the block: "Avalanche forecast by &lt;center&gt; via avalanche.org", or "Avalanche
    /// forecast by Colorado Avalanche Information Center via avalanche.state.co.us" for a CAIC area.
    /// </summary>
    [JsonPropertyName("attribution")]
    public string Attribution { get; init; } = string.Empty;
}
