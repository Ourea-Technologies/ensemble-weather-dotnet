// SPDX-License-Identifier: BSD-2-Clause

using System.Text.Json.Serialization;

namespace EnsembleWeather;

/// <summary>
/// A named elevation of a catalog location.
/// </summary>
public sealed class CatalogElevation
{
    /// <summary>
    /// The label.
    /// </summary>
    [JsonPropertyName("label")]
    public string Label { get; init; } = string.Empty;

    /// <summary>
    /// Elevation, metres.
    /// </summary>
    [JsonPropertyName("elevation_m")]
    public double ElevationMeters { get; init; }

    /// <summary>
    /// A latitude override for this elevation.
    /// </summary>
    [JsonPropertyName("lat_override")]
    public double? LatitudeOverride { get; init; }

    /// <summary>
    /// A longitude override for this elevation.
    /// </summary>
    [JsonPropertyName("lon_override")]
    public double? LongitudeOverride { get; init; }

    /// <summary>
    /// Whether this is the default elevation to show in the UI.
    /// </summary>
    [JsonPropertyName("is_default")]
    public bool IsDefault { get; init; }
}
