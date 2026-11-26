// SPDX-License-Identifier: BSD-2-Clause

using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace EnsembleWeather;

/// <summary>
/// The members' depth at each step. Absent when there is no plume.
/// </summary>
public sealed class SnowpackPlumeDepth
{
    /// <summary>
    /// The time of the first value of every series in the body.
    /// </summary>
    [JsonPropertyName("start_time")]
    public DateTimeOffset StartTime { get; init; }

    /// <summary>
    /// Hours between values (3).
    /// </summary>
    [JsonPropertyName("interval_hours")]
    public int IntervalHours { get; init; }

    /// <summary>
    /// Members that reach each step; it falls where a model's horizon ends.
    /// </summary>
    [JsonPropertyName("members")]
    public IReadOnlyList<int> Members { get => field ?? []; init; } = [];

    /// <summary>
    /// The members' median depth, in the response's units. Plus and Pro (<c>plume_detail</c>); ABSENT for Free and
    /// anonymous callers.
    /// </summary>
    [JsonPropertyName("p50")]
    public IReadOnlyList<double> P50 { get => field ?? []; init; } = [];

    /// <summary>
    /// Served to every caller, as the bands are. The equal-weight mean depth of the members that reach each step
    /// (<c>members</c>), in the response's units (cm, or inches with <c>units=imperial</c>) at the depths'
    /// precision; one value per step from <c>start_time</c> every <c>interval_hours</c>, the length of <c>p50</c>.
    /// Never null. It steps where a model's horizon ends and its members leave the mean, as the bands do.
    /// </summary>
    [JsonPropertyName("mean")]
    public IReadOnlyList<double> Mean { get => field ?? []; init; } = [];

    /// <summary>
    /// Every caller (Plus and Pro only before). A tenth of the members are at or below it.
    /// </summary>
    [JsonPropertyName("p10")]
    public IReadOnlyList<double> P10 { get => field ?? []; init; } = [];

    /// <summary>
    /// Every caller (Plus and Pro only before).
    /// </summary>
    [JsonPropertyName("p25")]
    public IReadOnlyList<double> P25 { get => field ?? []; init; } = [];

    /// <summary>
    /// Every caller (Plus and Pro only before).
    /// </summary>
    [JsonPropertyName("p75")]
    public IReadOnlyList<double> P75 { get => field ?? []; init; } = [];

    /// <summary>
    /// Every caller (Plus and Pro only before). A tenth of the members are at or above it.
    /// </summary>
    [JsonPropertyName("p90")]
    public IReadOnlyList<double> P90 { get => field ?? []; init; } = [];
}
