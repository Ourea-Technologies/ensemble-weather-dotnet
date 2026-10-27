// SPDX-License-Identifier: BSD-2-Clause

using System.Text.Json.Serialization;

namespace EnsembleWeather;

/// <summary>
/// A neighbouring location and its distance.
/// </summary>
public sealed class NearbyLocation : LocationSummary
{
    /// <summary>
    /// Great-circle distance from the location, km.
    /// </summary>
    [JsonPropertyName("distance_km")]
    public double DistanceKm { get; init; }
}
