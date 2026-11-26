// SPDX-License-Identifier: BSD-2-Clause

using System;
using System.Text.Json.Serialization;

namespace EnsembleWeather;

/// <summary>
/// What the snowpack model states is on the ground at a spot at one hour. <c>depth</c> is the same number as
/// <c>depth.p50</c> of that hour on <c>GET /locations/{state}/{slug}/snowpack</c>, which also serves the depth's
/// range, its hourly series, the six-hourly steps and the layer notes. This is a description of the snow, not an
/// avalanche forecast: nothing here is a stability or hazard statement.
/// </summary>
public sealed class SnowOnGround
{
    /// <summary>
    /// Always <c>true</c> while the product is a beta; fields may be added.
    /// </summary>
    [JsonPropertyName("beta")]
    public bool Beta { get; init; }

    /// <summary>
    /// Snow depth: cm, or in (imperial).
    /// </summary>
    [JsonPropertyName("depth")]
    public double Depth { get; init; }

    /// <summary>
    /// <c>measured</c>: the run started from a snow-depth sensor at the spot. <c>open_loop</c>: the model's own
    /// state, with the measured median correction of such depths. An open set: treat an unknown value as
    /// <c>open_loop</c>.
    /// </summary>
    [JsonPropertyName("basis")]
    public string Basis { get; init; } = string.Empty;

    /// <summary>
    /// The surface class at the hour; <c>/snowpack</c>'s <c>definitions.surface</c> carries the text of each value
    /// (an open set). Omitted before the run's first six-hourly step.
    /// </summary>
    [JsonPropertyName("surface")]
    public string? Surface { get; init; }

    /// <summary>
    /// The hour the depth is for.
    /// </summary>
    [JsonPropertyName("valid_time")]
    public DateTimeOffset ValidTime { get; init; }

    /// <summary>
    /// When the snowpack run that produced it finished.
    /// </summary>
    [JsonPropertyName("generated_at")]
    public DateTimeOffset GeneratedAt { get; init; }
}
