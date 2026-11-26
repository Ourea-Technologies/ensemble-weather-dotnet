// SPDX-License-Identifier: BSD-2-Clause

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace EnsembleWeather;

/// <summary>
/// The snow on one slope at a spot, described as the spot's flat ground is: the model run on a slope of
/// <c>slope_angle_deg</c> facing <c>aspect</c>, with the flat ground's snowfall and the slope's own sun. Modelled
/// and never measured. Descriptive only.
/// </summary>
public sealed class SnowpackAspect
{
    /// <summary>
    /// The way the slope faces: today <c>north</c>, <c>east</c>, <c>south</c> or <c>west</c>. Tolerate values you
    /// do not know.
    /// </summary>
    [JsonPropertyName("aspect")]
    public string Aspect { get; init; } = string.Empty;

    /// <summary>
    /// How steep the slope is, degrees from horizontal.
    /// </summary>
    [JsonPropertyName("slope_angle_deg")]
    public double SlopeAngleDeg { get; init; }

    /// <summary>
    /// Hourly snow depth on the slope, measured vertically as the flat depth is. <c>basis</c> is always
    /// <c>open_loop</c>.
    /// </summary>
    [JsonPropertyName("depth")]
    public SnowpackDepth Depth { get; init; } = null!;

    /// <summary>
    /// As the spot's <c>depth_change_24h</c>, this slope's own (open loop on both sides), read from the same
    /// earlier run as the spot's and from no other. Absent where the spot's has no earlier run, that run kept no
    /// such slope, or a value is missing.
    /// </summary>
    [JsonPropertyName("depth_change_24h")]
    public double? DepthChange24Hours { get; init; }

    /// <summary>
    /// One entry every six hours, at the spot's own <c>steps[].valid_time</c>.
    /// </summary>
    [JsonPropertyName("steps")]
    public IReadOnlyList<SnowpackStep> Steps { get; init; } = [];

    /// <summary>
    /// Buried layers on the slope at <c>depth.start_time</c>, at most three. Descriptive only.
    /// </summary>
    [JsonPropertyName("layers")]
    public IReadOnlyList<SnowpackLayer> Layers { get; init; } = [];
}
