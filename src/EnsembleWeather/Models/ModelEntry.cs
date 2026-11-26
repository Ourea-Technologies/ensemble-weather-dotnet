// SPDX-License-Identifier: BSD-2-Clause

using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace EnsembleWeather;

/// <summary>
/// One contributing model.
/// </summary>
public sealed class ModelEntry
{
    /// <summary>
    /// The model id (<c>aifs</c>, <c>aigefs</c>, <c>gefs</c>, <c>ifs_ens</c>, <c>rrfs</c>; treat others as additive).
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; init; } = string.Empty;

    /// <summary>
    /// The model's own cycle init (per-model-latest). Omitted when it was not read.
    /// </summary>
    [JsonPropertyName("cycle")]
    public DateTimeOffset? Cycle { get; init; }

    /// <summary>
    /// How many of its members the forecast carries (in <c>model_run</c>); omitted in <c>/runs</c> <c>served</c>.
    /// </summary>
    [JsonPropertyName("members")]
    public int? Members { get; init; }

    /// <summary>
    /// The <c>member_id</c>s of this model's members served for display only, on a response that asks for members:
    /// <c>rrfs</c> 1..5 are the RRFS ensemble's m001..m005, beside the deterministic run (0). They ride in
    /// <c>timeseries[].members</c> and <c>forecast.plume</c> like any member, and are in nothing else: not in
    /// <c>members</c> or <c>total_members</c>, the blend, any statistic, band or <c>precipitation_by_type</c>, any
    /// <c>prob=</c> or <c>totals=</c> answer, or verification. Absent when there are none (a cycle stored without
    /// them, or a point with no value of them). A member that a cycle did not finish is left out, so this can be
    /// fewer than five.
    /// </summary>
    [JsonPropertyName("display_members")]
    public IReadOnlyList<int> DisplayMembers { get => field ?? []; init; } = [];

    /// <summary>
    /// How the <c>display_members</c>' <c>snowfall</c> and <c>cumulative_snowfall</c> are made, present exactly
    /// when <c>display_members</c> is. <c>borrowed_from_deterministic</c>: the members store no pressure column, so
    /// each member's snowfall is its own precipitation x the deterministic run's snow-to-liquid ratio at that hour,
    /// gated by the deterministic run's snow level, through the same snowfall rule as every member. Their snowfall
    /// spread is the precipitation spread alone, not each member's own temperature profile; label it as borrowed
    /// where it is drawn.
    /// </summary>
    [JsonPropertyName("display_members_snowfall")]
    public string? DisplayMembersSnowfall { get; init; }
}
