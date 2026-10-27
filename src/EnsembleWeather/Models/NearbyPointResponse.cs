// SPDX-License-Identifier: BSD-2-Clause

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace EnsembleWeather;

/// <summary>
/// The nearest public locations to a coordinate.
/// </summary>
public sealed class NearbyPointResponse : ApiResult
{
    /// <summary>
    /// Latitude, decimal degrees (WGS84).
    /// </summary>
    [JsonPropertyName("lat")]
    public double Latitude { get; init; }

    /// <summary>
    /// Longitude, decimal degrees (WGS84, west negative).
    /// </summary>
    [JsonPropertyName("lon")]
    public double Longitude { get; init; }

    /// <summary>
    /// The search radius, in kilometres.
    /// </summary>
    [JsonPropertyName("radius_km")]
    public double RadiusKm { get; init; }

    /// <summary>
    /// The number of items in this response.
    /// </summary>
    [JsonPropertyName("count")]
    public int Count { get; init; }

    /// <summary>
    /// The locations, nearest first where a distance applies.
    /// </summary>
    [JsonPropertyName("locations")]
    public IReadOnlyList<NearbyLocation> Locations { get; init; } = [];
}
