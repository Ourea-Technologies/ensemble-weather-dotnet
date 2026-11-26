// SPDX-License-Identifier: BSD-2-Clause

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace EnsembleWeather;

/// <summary>
/// One model's members in the snow-depth plume.
/// </summary>
public sealed class SnowpackPlumeModel
{
    /// <summary>
    /// <c>aifs</c>, <c>aigefs</c>, <c>gefs</c> or <c>ifs_ens</c>; a client must tolerate one it does not know.
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; init; } = string.Empty;

    /// <summary>
    /// The model's own member numbers, in the order of <c>depth</c>.
    /// </summary>
    [JsonPropertyName("member_ids")]
    public IReadOnlyList<int> MemberIds { get => field ?? []; init; } = [];

    /// <summary>
    /// One series per member, from <c>depth.start_time</c> every <c>depth.interval_hours</c>.
    /// </summary>
    [JsonPropertyName("depth")]
    public IReadOnlyList<IReadOnlyList<double>> Depth { get => field ?? []; init; } = [];

    /// <summary>
    /// The equal-weight mean of this model's members' <c>depth</c> at each of the model's own steps (from
    /// <c>depth.start_time</c> every <c>depth.interval_hours</c>, to the model's horizon: the length of its
    /// members' series), in the response's units. Never null.
    /// </summary>
    [JsonPropertyName("depth_mean")]
    public IReadOnlyList<double> DepthMean { get => field ?? []; init; } = [];

    /// <summary>
    /// Members not run because the ground starts bare and they bring no snow; their series is 0 throughout.
    /// </summary>
    [JsonPropertyName("members_skipped")]
    public int MembersSkipped { get; init; }
}
