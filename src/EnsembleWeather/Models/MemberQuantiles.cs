// SPDX-License-Identifier: BSD-2-Clause

using System.Text.Json.Serialization;

namespace EnsembleWeather;

/// <summary>
/// A distribution over the ensemble MEMBERS (<c>members_cumulative</c>, <c>members_spread</c>, <c>members_6h</c>):
/// the <c>Quantiles</c> shape. <c>P50</c>, the members' median, is the plume's paid detail (the <c>plume_detail</c>
/// entitlement: Plus and Pro) and is ABSENT for Free and anonymous callers (<c>members_view: pool</c>); the mean,
/// <c>p10</c>, <c>p25</c>, <c>p75</c>, <c>p90</c>, <c>min</c> and <c>max</c> are served to every caller. This does
/// not touch the forecast's own calibrated quantiles (<c>ensemble.&lt;variable&gt;.p50</c>, the daily statistics).
/// </summary>
public sealed class MemberQuantiles
{
    /// <summary>
    /// The mean.
    /// </summary>
    [JsonPropertyName("mean")]
    public double Mean { get; init; }

    /// <summary>
    /// The 10th percentile.
    /// </summary>
    [JsonPropertyName("p10")]
    public double P10 { get; init; }

    /// <summary>
    /// The 25th percentile.
    /// </summary>
    [JsonPropertyName("p25")]
    public double P25 { get; init; }

    /// <summary>
    /// The members' median. Plus and Pro; absent for Free and anonymous.
    /// </summary>
    [JsonPropertyName("p50")]
    public double? P50 { get; init; }

    /// <summary>
    /// The 75th percentile.
    /// </summary>
    [JsonPropertyName("p75")]
    public double P75 { get; init; }

    /// <summary>
    /// The 90th percentile.
    /// </summary>
    [JsonPropertyName("p90")]
    public double P90 { get; init; }

    /// <summary>
    /// The minimum.
    /// </summary>
    [JsonPropertyName("min")]
    public double Min { get; init; }

    /// <summary>
    /// The maximum.
    /// </summary>
    [JsonPropertyName("max")]
    public double Max { get; init; }

    /// <summary>
    /// As <c>Quantiles.probability</c>, where the use says.
    /// </summary>
    [JsonPropertyName("probability")]
    public double? Probability { get; init; }
}
