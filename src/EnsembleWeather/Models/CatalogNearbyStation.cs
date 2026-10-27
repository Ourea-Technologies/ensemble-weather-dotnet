// SPDX-License-Identifier: BSD-2-Clause

using System.Text.Json.Serialization;

namespace EnsembleWeather;

/// <summary>
/// Inlined summary of a station near a catalog location — enough to render a list; full detail requires a follow-
/// up GET against <c>/api/v1/catalog/stations/{slug}</c>.
/// </summary>
public sealed class CatalogNearbyStation
{
    /// <summary>
    /// The catalog slug.
    /// </summary>
    [JsonPropertyName("slug")]
    public string Slug { get; init; } = string.Empty;

    /// <summary>
    /// The name.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; init; } = string.Empty;

    /// <summary>
    /// The observation network.
    /// </summary>
    [JsonPropertyName("network")]
    public string Network { get; init; } = string.Empty;

    /// <summary>
    /// The station status.
    /// </summary>
    [JsonPropertyName("station_status")]
    public string StationStatus { get; init; } = string.Empty;

    /// <summary>
    /// The distance, kilometres.
    /// </summary>
    [JsonPropertyName("distance_km")]
    public double? DistanceKm { get; init; }

    /// <summary>
    /// The elevation difference, metres.
    /// </summary>
    [JsonPropertyName("elevation_diff_m")]
    public double? ElevationDifferenceMeters { get; init; }

    /// <summary>
    /// Whether this is the primary station.
    /// </summary>
    [JsonPropertyName("is_primary")]
    public bool IsPrimary { get; init; }
}
