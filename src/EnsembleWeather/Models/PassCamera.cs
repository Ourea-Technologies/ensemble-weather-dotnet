// SPDX-License-Identifier: BSD-2-Clause

using System;
using System.Text.Json.Serialization;

namespace EnsembleWeather;

/// <summary>
/// One DOT camera, a still image the DOT refreshes about once a minute. Hotlinked: the client loads
/// <c>image_url</c> from the DOT and shows <c>attribution</c>.
/// </summary>
public sealed class PassCamera
{
    /// <summary>
    /// The id.
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; init; } = string.Empty;

    /// <summary>
    /// The DOT's name for the camera.
    /// </summary>
    [JsonPropertyName("title")]
    public string Title { get; init; } = string.Empty;

    /// <summary>
    /// The DOT's current still image.
    /// </summary>
    [JsonPropertyName("image_url")]
    public string ImageUrl { get; init; } = string.Empty;

    /// <summary>
    /// The DOT's own page for the camera, to link out to.
    /// </summary>
    [JsonPropertyName("url")]
    public string Url { get; init; } = string.Empty;

    /// <summary>
    /// The image's time (its Last-Modified) when Ourea last checked it, every 15 minutes; the image at
    /// <c>image_url</c> may be newer. Omitted until it has been checked.
    /// </summary>
    [JsonPropertyName("captured_at")]
    public DateTimeOffset? CapturedAt { get; init; }

    /// <summary>
    /// <c>captured_at</c> is unknown or more than 60 minutes before <c>as_of</c>: the camera may be frozen.
    /// </summary>
    [JsonPropertyName("stale")]
    public bool Stale { get; init; }

    /// <summary>
    /// One of <c>caltrans</c>, <c>wsdot</c>, <c>cdot</c>, <c>odot</c>.
    /// </summary>
    [JsonPropertyName("source")]
    public string Source { get; init; } = string.Empty;

    /// <summary>
    /// The credit the source requires.
    /// </summary>
    [JsonPropertyName("attribution")]
    public string Attribution { get; init; } = string.Empty;
}
