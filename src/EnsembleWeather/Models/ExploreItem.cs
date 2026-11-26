// SPDX-License-Identifier: BSD-2-Clause

using System;
using System.Text.Json.Serialization;

namespace EnsembleWeather;

/// <summary>
/// One ranked point of a place.
/// </summary>
public sealed class ExploreItem
{
    /// <summary>
    /// 1-based, across pages.
    /// </summary>
    [JsonPropertyName("rank")]
    public int Rank { get; init; }

    /// <summary>
    /// ONE location reference, the same wherever a place is named -- the forecast's and <c>/current</c>'s
    /// <c>location</c>, every list and <c>/nearby</c> row, search hits and favorites.
    /// </summary>
    [JsonPropertyName("location")]
    public LocationReference Location { get; init; } = null!;

    /// <summary>
    /// The named elevation; <c>null</c> = the place's own point.
    /// </summary>
    [JsonPropertyName("elevation_name")]
    public string? ElevationName { get; init; }

    /// <summary>
    /// What the elevation <c>elevation_name</c> is called, as <c>ElevationSummary.display_name</c>; absent where it
    /// has no name.
    /// </summary>
    [JsonPropertyName("elevation_display_name")]
    public string? ElevationDisplayName { get; init; }

    /// <summary>
    /// The point's elevation, m or ft by <c>units</c>.
    /// </summary>
    [JsonPropertyName("elevation")]
    public double Elevation { get; init; }

    /// <summary>
    /// The metric's value as the place's page serves it, in <c>unit</c>.
    /// </summary>
    [JsonPropertyName("value")]
    public double Value { get; init; }

    /// <summary>
    /// Present only on <c>snow_today</c> (the day's served <c>total_snowfall.p10</c>, where it has a
    /// <c>snow_amount</c>) and <c>precip_today</c> (<c>total_precipitation_if_wet.p10</c>, where the day is not
    /// <c>none</c>). No range on multi-day sums.
    /// </summary>
    [JsonPropertyName("p10")]
    public double? P10 { get; init; }

    /// <summary>
    /// As <c>p10</c>.
    /// </summary>
    [JsonPropertyName("p90")]
    public double? P90 { get; init; }

    /// <summary>
    /// When the value was taken: the forecast windows when recorded from the served run, <c>base_depth</c> its
    /// hour, <c>new_snow_24h</c> the snowpack run's first hour, the season its newest computed day.
    /// </summary>
    [JsonPropertyName("as_of")]
    public DateTimeOffset AsOf { get; init; }

    /// <summary>
    /// Forecast windows only, the served run's <c>model_run.initialized_at</c>.
    /// </summary>
    [JsonPropertyName("initialized_at")]
    public DateTimeOffset? InitializedAt { get; init; }

    /// <summary>
    /// One of <c>ourea</c>, <c>snowpack_measured</c>, <c>snowpack_open_loop</c>, <c>mrms_multisensor</c>,
    /// <c>station_gauge</c>.
    /// </summary>
    [JsonPropertyName("source")]
    public string Source { get; init; } = string.Empty;

    /// <summary>
    /// With <c>near</c> only, from it (0.1 km).
    /// </summary>
    [JsonPropertyName("distance_km")]
    public double? DistanceKm { get; init; }

    /// <summary>
    /// The place's forecast route; add <c>?elevations=&lt;elevation_name&gt;</c> for a named point.
    /// </summary>
    [JsonPropertyName("path")]
    public string Path { get; init; } = string.Empty;
}
