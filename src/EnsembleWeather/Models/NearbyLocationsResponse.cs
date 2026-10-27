// SPDX-License-Identifier: BSD-2-Clause

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace EnsembleWeather;

/// <summary>
/// A location and its nearest public neighbours.
/// </summary>
public sealed class NearbyLocationsResponse : ApiResult
{
    /// <summary>
    /// The location.
    /// </summary>
    [JsonPropertyName("location")]
    public LocationSummary Location { get; init; } = null!;

    /// <summary>
    /// The radius applied, after the cap.
    /// </summary>
    [JsonPropertyName("radius_km")]
    public double RadiusKm { get; init; }

    /// <summary>
    /// Neighbours in this response (the location itself is not counted).
    /// </summary>
    [JsonPropertyName("count")]
    public int Count { get; init; }

    /// <summary>
    /// Nearest first.
    /// </summary>
    [JsonPropertyName("locations")]
    public IReadOnlyList<NearbyLocation> Locations { get; init; } = [];
}
