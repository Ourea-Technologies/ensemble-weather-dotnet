// SPDX-License-Identifier: BSD-2-Clause

using System.Text.Json.Serialization;

namespace EnsembleWeather;

/// <summary>
/// Closed key set — unknown keys are a <c>400</c>. The photo is referenced **by id**, never a literal URL.
/// </summary>
public sealed class CatalogAdCreative
{
    /// <summary>
    /// The headline text.
    /// </summary>
    [JsonPropertyName("headline")]
    public string? Headline { get; init; }

    /// <summary>
    /// The body.
    /// </summary>
    [JsonPropertyName("body")]
    public string? Body { get; init; }

    /// <summary>
    /// Call-to-action label.
    /// </summary>
    [JsonPropertyName("cta")]
    public string? CallToAction { get; init; }

    /// <summary>
    /// Must be http(s).
    /// </summary>
    [JsonPropertyName("click_url")]
    public string? ClickUrl { get; init; }

    /// <summary>
    /// Must reference an existing photo row.
    /// </summary>
    [JsonPropertyName("photo_id")]
    public string? PhotoId { get; init; }
}
