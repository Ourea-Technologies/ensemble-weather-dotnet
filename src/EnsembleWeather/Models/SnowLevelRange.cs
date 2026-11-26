// SPDX-License-Identifier: BSD-2-Clause

using System.Text.Json.Serialization;

namespace EnsembleWeather;

/// <summary>
/// A day's snow level over its wet hours (m, or ft with <c>units=imperial</c>): the lowest and highest hourly
/// ensemble-mean snow level among them, and their precipitation-weighted mean. The level is the rain line (the
/// bottom of the melting layer), as <c>EnsembleStats.snow_level</c> is; the day on the NWS scale is
/// <c>DailyForecast.snow_level_wet_bulb</c>. <c>DailyForecast.snow_level_wet_bulb_range</c> is this same object
/// over the same hours on the NWS scale (the height of the 0.5 C wet-bulb).
/// </summary>
public sealed class SnowLevelRange
{
    /// <summary>
    /// The lowest hourly snow level among the wet hours.
    /// </summary>
    [JsonPropertyName("low")]
    public double Low { get; init; }

    /// <summary>
    /// The highest hourly snow level among the wet hours.
    /// </summary>
    [JsonPropertyName("high")]
    public double High { get; init; }

    /// <summary>
    /// The wet hours' snow level, weighted by their precipitation.
    /// </summary>
    [JsonPropertyName("mean")]
    public double Mean { get; init; }
}
