// SPDX-License-Identifier: BSD-2-Clause

using System.Text.Json.Serialization;

namespace EnsembleWeather;

/// <summary>
/// A named elevation of a location.
/// </summary>
public sealed class LocationElevation
{
    /// <summary>
    /// The name.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; init; } = string.Empty;

    /// <summary>
    /// In the caller's length units (m, ft), to 0.1.
    /// </summary>
    [JsonPropertyName("elevation")]
    public double Elevation { get; init; }

    /// <summary>
    /// Present (true) on the primary only.
    /// </summary>
    [JsonPropertyName("default")]
    public bool? IsDefault { get; init; }
}
