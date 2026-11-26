// SPDX-License-Identifier: BSD-2-Clause

using System;
using System.Text.Json.Serialization;

namespace EnsembleWeather;

/// <summary>
/// One hour of a mountain pass forecast, at the pass's point and elevation.
/// </summary>
public sealed class PassHour
{
    /// <summary>
    /// The hour's <c>valid_time</c>; amounts are over the hour ending then.
    /// </summary>
    [JsonPropertyName("time")]
    public DateTimeOffset Time { get; init; }

    /// <summary>
    /// <c>ensemble.temperature.mean</c>.
    /// </summary>
    [JsonPropertyName("temperature")]
    public double Temperature { get; init; }

    /// <summary>
    /// <c>ensemble.precipitation.mean</c> (mm, in).
    /// </summary>
    [JsonPropertyName("precipitation")]
    public double Precipitation { get; init; }

    /// <summary>
    /// <c>ensemble.precipitation.probability</c>, 0-1.
    /// </summary>
    [JsonPropertyName("precipitation_probability")]
    public double? PrecipitationProbability { get; init; }

    /// <summary>
    /// The hour's served <c>precipitation_type.type</c>; omitted on a dry hour. One of <c>rain</c>, <c>snow</c>,
    /// <c>rain_snow_mix</c>, <c>freezing_rain</c>.
    /// </summary>
    [JsonPropertyName("precipitation_type")]
    public string? PrecipitationType { get; init; }

    /// <summary>
    /// <c>ensemble.snowfall.mean</c> (mm, in); 0 without snow.
    /// </summary>
    [JsonPropertyName("snowfall")]
    public double Snowfall { get; init; }

    /// <summary>
    /// <c>ensemble.snow_level.mean</c> (m, ft); omitted where not served.
    /// </summary>
    [JsonPropertyName("snow_level")]
    public double? SnowLevel { get; init; }

    /// <summary>
    /// (JSON <c>snow_level_above_pass</c>) <c>snow_level</c> minus the pass's elevation (m, ft): above 0 the snow level is above the pass (rain at the
    /// pass when wet), below 0 it is under it.
    /// </summary>
    [JsonPropertyName("snow_level_above_pass")]
    public double? SnowLevelMargin { get; init; }

    /// <summary>
    /// The hour is at or below freezing (temperature mean at or below 0 C, 32 F) and wet: precipitation of at least
    /// 0.254 mm (0.01 in) in this hour or one of the two before it. Weather at the pass, not a measured road state.
    /// </summary>
    [JsonPropertyName("freezing_on_wet_road")]
    public bool FreezingOnWetRoad { get; init; }

    /// <summary>
    /// <c>ensemble.wind_speed.mean</c>.
    /// </summary>
    [JsonPropertyName("wind_speed")]
    public double WindSpeed { get; init; }

    /// <summary>
    /// <c>ensemble.wind_gust.mean</c>; omitted where not served.
    /// </summary>
    [JsonPropertyName("wind_gust")]
    public double? WindGust { get; init; }

    /// <summary>
    /// <c>wind_gust</c> is over 40 mph (17.9 m/s); threshold fixed, does not change with units.
    /// </summary>
    [JsonPropertyName("gust_over_40mph")]
    public bool GustOver40Mph { get; init; }

    /// <summary>
    /// Gusts of 25 mph (11.2 m/s) or more at or below freezing with loose snow to move: snow falling this hour, 1
    /// in (25.4 mm) or more fallen in the 24 served hours before it, or snow on the ground at the pass
    /// (<c>snow_on_ground</c>).
    /// </summary>
    [JsonPropertyName("blowing_snow")]
    public bool BlowingSnow { get; init; }
}
