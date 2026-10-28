// SPDX-License-Identifier: BSD-2-Clause

using System.Text.Json.Serialization;

namespace EnsembleWeather;

/// <summary>
/// On a <c>PRO_REQUIRED</c> refusal of a coordinate request: the nearest named location whose forecast answers without
/// a key (within 10 km of the point and 300 m of its elevation). Call its <see cref="Url"/> anonymously.
/// </summary>
public sealed class NearestLocation
{
    /// <summary>
    /// The location's slug.
    /// </summary>
    [JsonPropertyName("slug")]
    public string Slug { get; init; } = string.Empty;

    /// <summary>
    /// The location route's path on this API, with the refused route's suffix (<c>/current</c>, <c>/alerts</c>).
    /// </summary>
    [JsonPropertyName("url")]
    public string Url { get; init; } = string.Empty;

    /// <summary>
    /// The distance from the point, kilometres.
    /// </summary>
    [JsonPropertyName("distance_km")]
    public double DistanceKilometers { get; init; }

    /// <summary>
    /// The location's elevation, metres.
    /// </summary>
    [JsonPropertyName("elevation_m")]
    public double ElevationMeters { get; init; }
}
