// SPDX-License-Identifier: BSD-2-Clause

using System.Text.Json.Serialization;

namespace EnsembleWeather;

/// <summary>
/// The snow-depth sensor at the spot. Absent when there is none.
/// </summary>
public sealed class SnowpackAnchor
{
    /// <summary>
    /// The station's catalog slug.
    /// </summary>
    [JsonPropertyName("station")]
    public string Station { get; init; } = string.Empty;

    /// <summary>
    /// The name.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; init; } = string.Empty;

    /// <summary>
    /// The station's network.
    /// </summary>
    [JsonPropertyName("network")]
    public string Network { get; init; } = string.Empty;
}
