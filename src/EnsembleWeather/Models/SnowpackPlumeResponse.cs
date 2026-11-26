// SPDX-License-Identifier: BSD-2-Clause

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace EnsembleWeather;

/// <summary>
/// A location's snow-depth plume.
/// </summary>
public sealed class SnowpackPlumeResponse : ApiResult
{
    /// <summary>
    /// Always <c>true</c> while the product is a beta; fields may be added.
    /// </summary>
    [JsonPropertyName("beta")]
    public bool Beta { get; init; }

    /// <summary>
    /// ONE location reference, the same wherever a place is named -- the forecast's and <c>/current</c>'s
    /// <c>location</c>, every list and <c>/nearby</c> row, search hits and favorites.
    /// </summary>
    [JsonPropertyName("location")]
    public LocationReference Location { get; init; } = null!;

    /// <summary>
    /// Unit system for response values. <c>kelvin</c> returns metric units with Kelvin temperatures.
    /// </summary>
    [JsonPropertyName("units")]
    public UnitSystem Units { get; init; }

    /// <summary>
    /// The run the plume is from: the 12Z blend cycle the members were read from. Absent when no member stage has
    /// reached the location.
    /// </summary>
    [JsonPropertyName("model_run")]
    public SnowpackRun? ModelRun { get; init; }

    /// <summary>
    /// The plume's blend cycle is more than 24 h old.
    /// </summary>
    [JsonPropertyName("stale")]
    public bool Stale { get; init; }

    /// <summary>
    /// The members' depth at each step. Absent when there is no plume.
    /// </summary>
    [JsonPropertyName("depth")]
    public SnowpackPlumeDepth? Depth { get; init; }

    /// <summary>
    /// Every member's own series, by model, each model to its own horizon. Plus and Pro (<c>plume_detail</c>);
    /// absent for Free and anonymous callers, who are served <c>member_depths</c>.
    /// </summary>
    [JsonPropertyName("models")]
    public IReadOnlyList<SnowpackPlumeModel> Models { get => field ?? []; init; } = [];

    /// <summary>
    /// Every member's depth as ONE unlabelled pool, for Free and anonymous callers (absent for Plus and Pro, who
    /// are served <c>models</c>). One series per member, from <c>depth.start_time</c> every
    /// <c>depth.interval_hours</c>, each exactly the length of <c>depth.mean</c> (the body ends at the last step
    /// every member reaches, so no series is shorter than another). There is no member id and no model; the order
    /// is fixed for the run and the location and does not follow the models. Units as <c>depth</c>.
    /// <c>depth.members</c> is the number of series.
    /// </summary>
    [JsonPropertyName("member_depths")]
    public IReadOnlyList<IReadOnlyList<double>> MemberDepths { get => field ?? []; init; } = [];

    /// <summary>
    /// What the bands are, in a sentence a client may show beside them.
    /// </summary>
    [JsonPropertyName("spread")]
    public string Spread { get; init; } = string.Empty;
}
