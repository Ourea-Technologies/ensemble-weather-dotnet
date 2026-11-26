// SPDX-License-Identifier: BSD-2-Clause

using System.Text.Json.Serialization;

namespace EnsembleWeather;

/// <summary>
/// The point the grids are for.
/// </summary>
public sealed class SnowTrendElevation
{
    /// <summary>
    /// The named elevation; <c>null</c> for a location with one point.
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; init; }

    /// <summary>
    /// What that elevation is called, as <c>ElevationSummary.display_name</c>; absent where it has no name.
    /// </summary>
    [JsonPropertyName("display_name")]
    public string? DisplayName { get; init; }

    /// <summary>
    /// (JSON <c>elevation</c>) Its elevation, in m (metric) or ft (imperial).
    /// </summary>
    [JsonPropertyName("elevation")]
    public double Height { get; init; }
}
