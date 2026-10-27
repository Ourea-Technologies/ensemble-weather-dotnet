// SPDX-License-Identifier: BSD-2-Clause

using System.Text.Json.Serialization;

namespace EnsembleWeather;

/// <summary>
/// A webcam at a catalog location.
/// </summary>
public sealed class CatalogWebcam
{
    /// <summary>
    /// The name.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; init; } = string.Empty;

    /// <summary>
    /// The URL.
    /// </summary>
    [JsonPropertyName("url")]
    public string Url { get; init; } = string.Empty;

    /// <summary>
    /// How the webcam embeds.
    /// </summary>
    [JsonPropertyName("embed_type")]
    public string? EmbedType { get; init; }

    /// <summary>
    /// The provider id.
    /// </summary>
    [JsonPropertyName("provider")]
    public string? Provider { get; init; }

    /// <summary>
    /// How often the image refreshes, seconds.
    /// </summary>
    [JsonPropertyName("refresh_interval_seconds")]
    public int? RefreshIntervalSeconds { get; init; }
}
