// SPDX-License-Identifier: BSD-2-Clause

using System.Text.Json;
using System.Text.Json.Serialization;

namespace EnsembleWeather;

/// <summary>
/// One element of a page template's <c>spec</c>. Duplicate <c>(kind, key)</c> pairs within a spec are a
/// <c>400</c> (the same key under different kinds is fine).
/// </summary>
public sealed class CatalogTemplateSection
{
    /// <summary>
    /// Closed vocabulary — unknown kinds fail deserialization (<c>400</c>). <c>weather_panel</c> = a forecast/nowcast
    /// panel the client already knows how to draw; <c>content_slot</c> = a named hole filled by published content
    /// entries whose attachment <c>slot</c> matches <c>key</c>; <c>ad_slot</c> = a named hole resolved by the ad
    /// engine against <c>ad_slots.key</c> (the key must exist); <c>media</c> = photo/webcam strip.
    /// </summary>
    [JsonPropertyName("kind")]
    public string Kind { get; init; } = string.Empty;

    /// <summary>
    /// Kebab-case section key.
    /// </summary>
    [JsonPropertyName("key")]
    public string Key { get; init; } = string.Empty;

    /// <summary>
    /// Client-defined render hints (<c>hours</c>, <c>by_elevation_band</c>, media <c>source</c>/<c>limit</c>, …) —
    /// not backend contract.
    /// </summary>
    [JsonPropertyName("config")]
    public JsonElement? Config { get; init; }
}
