// SPDX-License-Identifier: BSD-2-Clause

using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace EnsembleWeather;

/// <summary>
/// A spot of a location's snowpack forecast: its elevation, depth, layers and steps.
/// </summary>
public sealed class SnowpackSpot
{
    /// <summary>
    /// The named elevation (<c>location.elevations[].name</c>); <c>null</c> is the location's own point.
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; init; }

    /// <summary>
    /// What the named elevation is called, as <c>ElevationSummary.display_name</c>; absent for the location's own
    /// point and where the catalog has no name for the elevation.
    /// </summary>
    [JsonPropertyName("display_name")]
    public string? DisplayName { get; init; }

    /// <summary>
    /// In the response's units (m or ft).
    /// </summary>
    [JsonPropertyName("elevation")]
    public double Elevation { get; init; }

    /// <summary>
    /// The elevation, metres.
    /// </summary>
    [JsonPropertyName("elevation_m")]
    public double ElevationMeters { get; init; }

    /// <summary>
    /// The blend cycle this spot's forecast was driven with; older than <c>model_run.initialized_at</c> when the
    /// newest run did not reach the spot.
    /// </summary>
    [JsonPropertyName("initialized_at")]
    public DateTimeOffset InitializedAt { get; init; }

    /// <summary>
    /// The snow-depth sensor at the spot. Absent when there is none.
    /// </summary>
    [JsonPropertyName("anchor")]
    public SnowpackAnchor? Anchor { get; init; }

    /// <summary>
    /// Hourly snow depth from <c>start_time</c> (cm or in).
    /// </summary>
    [JsonPropertyName("depth")]
    public SnowpackDepth Depth { get; init; } = null!;

    /// <summary>
    /// The depth at <c>depth.start_time</c> less this spot's modelled depth 24 h before, in the depth's units and
    /// rounding (cm or in; a loss is negative): the model's own change, snowfall, settlement and melt together.
    /// Both sides are taken as <c>depth.p50</c> is for its <c>basis</c>. The earlier depth is read from the newest
    /// earlier run whose start was at most 12 h before that hour and whose depth had the same <c>basis</c>; it is
    /// never a longer-lead value. Absent where there is no such run or either value is missing; never 0 in place of
    /// a value, and a real zero change is <c>0</c>.
    /// </summary>
    [JsonPropertyName("depth_change_24h")]
    public double? DepthChange24Hours { get; init; }

    /// <summary>
    /// Hourly snow water equivalent (mm or in), parallel to <c>depth.p50</c>.
    /// </summary>
    [JsonPropertyName("swe")]
    public IReadOnlyList<double> SnowWaterEquivalent { get => field ?? []; init; } = [];

    /// <summary>
    /// One entry every six hours.
    /// </summary>
    [JsonPropertyName("steps")]
    public IReadOnlyList<SnowpackStep> Steps { get => field ?? []; init; } = [];

    /// <summary>
    /// Buried layers at <c>depth.start_time</c>, at most three. Descriptive only.
    /// </summary>
    [JsonPropertyName("layers")]
    public IReadOnlyList<SnowpackLayer> Layers { get => field ?? []; init; } = [];

    /// <summary>
    /// The snow on a 38° slope at the spot facing each way, in the order north, east, south, west. Present only
    /// where the spot's slopes were run: every named elevation of a ski area, and the area's own unnamed spot at
    /// the 150 beta resorts. The fields above are always flat ground. Descriptive only.
    /// </summary>
    [JsonPropertyName("aspects")]
    public IReadOnlyList<SnowpackAspect> Aspects { get => field ?? []; init; } = [];
}
