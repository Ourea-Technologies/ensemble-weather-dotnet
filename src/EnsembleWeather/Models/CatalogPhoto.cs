// SPDX-License-Identifier: BSD-2-Clause

using System.Text.Json.Serialization;

namespace EnsembleWeather;

/// <summary>
/// Photo with the storage backend already resolved to a single public <c>url</c> (GCS object or external link).
/// </summary>
public sealed class CatalogPhoto
{
    /// <summary>
    /// The URL.
    /// </summary>
    [JsonPropertyName("url")]
    public string Url { get; init; } = string.Empty;

    /// <summary>
    /// The caption.
    /// </summary>
    [JsonPropertyName("caption")]
    public string? Caption { get; init; }

    /// <summary>
    /// The credit line to carry with any redistributed output.
    /// </summary>
    [JsonPropertyName("credit")]
    public string? Credit { get; init; }

    /// <summary>
    /// Alternative text.
    /// </summary>
    [JsonPropertyName("alt_text")]
    public string? AltText { get; init; }

    /// <summary>
    /// The licence.
    /// </summary>
    [JsonPropertyName("license")]
    public string? License { get; init; }

    /// <summary>
    /// The licence URL.
    /// </summary>
    [JsonPropertyName("license_url")]
    public string? LicenseUrl { get; init; }
}
