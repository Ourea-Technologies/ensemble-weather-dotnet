// SPDX-License-Identifier: BSD-2-Clause

using System;
using System.Text.Json.Serialization;

namespace EnsembleWeather;

/// <summary>
/// A nearby observation sensor, ranked by elevation-weighted distance.
/// </summary>
public sealed class NearestSensor
{
    /// <summary>
    /// The station's id in its network.
    /// </summary>
    [JsonPropertyName("station_id")]
    public string StationId { get; init; } = string.Empty;

    /// <summary>
    /// The station's name from the stations registry, else its id (it was one <c>station_name</c> holding the id).
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; init; } = string.Empty;

    /// <summary>
    /// Observation network (snotel, nws, raws, rwis, cdec, tempest, etc.)
    /// </summary>
    [JsonPropertyName("source")]
    public string Source { get; init; } = string.Empty;

    /// <summary>
    /// Straight-line surface distance in km.
    /// </summary>
    [JsonPropertyName("distance_km")]
    public double DistanceKm { get; init; }

    /// <summary>
    /// Sensor elevation minus forecast-location elevation, in the caller's length units (m metric, ft imperial).
    /// Positive if the sensor sits above the forecast point.
    /// </summary>
    [JsonPropertyName("elevation_diff")]
    public double ElevationDifference { get; init; }

    /// <summary>
    /// <c>distance_km + |elevation_diff_m| / 100</c>. Lower ranks closer.
    /// </summary>
    [JsonPropertyName("effective_distance_km")]
    public double EffectiveDistanceKm { get; init; }

    /// <summary>
    /// When the observation was taken.
    /// </summary>
    [JsonPropertyName("observation_time")]
    public DateTimeOffset ObservationTime { get; init; }
}
