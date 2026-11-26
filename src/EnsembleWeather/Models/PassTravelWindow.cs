// SPDX-License-Identifier: BSD-2-Clause

using System;
using System.Text.Json.Serialization;

namespace EnsembleWeather;

/// <summary>
/// A window to cross a mountain pass in, and the weather in it.
/// </summary>
public sealed class PassTravelWindow
{
    /// <summary>
    /// When the run's first hour begins.
    /// </summary>
    [JsonPropertyName("start")]
    public DateTimeOffset Start { get; init; }

    /// <summary>
    /// When its last hour ends.
    /// </summary>
    [JsonPropertyName("end")]
    public DateTimeOffset End { get; init; }

    /// <summary>
    /// The sum of the hourly snowfall means over the run (mm, in).
    /// </summary>
    [JsonPropertyName("snowfall")]
    public double Snowfall { get; init; }

    /// <summary>
    /// The hours with a wet road below freezing.
    /// </summary>
    [JsonPropertyName("freezing_on_wet_road_hours")]
    public int FreezingOnWetRoadHours { get; init; }

    /// <summary>
    /// The highest hourly gust mean in the run; omitted where no gust is served.
    /// </summary>
    [JsonPropertyName("max_wind_gust")]
    public double? MaxWindGust { get; init; }
}
