// SPDX-License-Identifier: BSD-2-Clause

using System;
using System.Text.Json.Serialization;

namespace EnsembleWeather;

/// <summary>
/// One <c>?prob=</c> term, answered.
/// </summary>
public sealed class ProbabilityResult
{
    /// <summary>
    /// The term's canonical spelling, as the SDK sent it (<c>snowfall:ge:6.000:24h</c>): the key the SDK matches the
    /// caller's term by.
    /// </summary>
    [JsonPropertyName("term")]
    public string Term { get; init; } = string.Empty;

    /// <summary>
    /// The forecast variable.
    /// </summary>
    [JsonPropertyName("variable")]
    public ForecastVariable Variable { get; init; }

    /// <summary>
    /// The comparison: at least (<c>ge</c>) or at most (<c>le</c>).
    /// </summary>
    [JsonPropertyName("op")]
    public ProbabilityOperator Operator { get; init; }

    /// <summary>
    /// In the request's units, at the variable's served precision.
    /// </summary>
    [JsonPropertyName("threshold")]
    public double Threshold { get; init; }

    /// <summary>
    /// The window start, in hours from the first served hour.
    /// </summary>
    [JsonPropertyName("window_start_hours")]
    public int WindowStartHours { get; init; }

    /// <summary>
    /// The window end, in hours from the first served hour.
    /// </summary>
    [JsonPropertyName("window_end_hours")]
    public int WindowEndHours { get; init; }

    /// <summary>
    /// How the window is aggregated: <c>total</c>, <c>any</c> or <c>all</c>.
    /// </summary>
    [JsonPropertyName("aggregation")]
    public ProbabilityAggregation Aggregation { get; init; }

    /// <summary>
    /// The window's first instant.
    /// </summary>
    [JsonPropertyName("from")]
    public DateTimeOffset? From { get; init; }

    /// <summary>
    /// The window's end (exclusive).
    /// </summary>
    [JsonPropertyName("to")]
    public DateTimeOffset? To { get; init; }

    /// <summary>
    /// The share of members meeting the term, 0..1 (0.001); <see langword="null"/> (never 0) when it cannot be
    /// answered: <see cref="NullReason"/> says why.
    /// </summary>
    [JsonPropertyName("probability")]
    public double? Probability { get; init; }

    /// <summary>
    /// Members that voted: for <c>total</c>, the members whose values cover the whole window; for
    /// <c>any</c>/<c>all</c>, those with a value in it.
    /// </summary>
    [JsonPropertyName("members")]
    public int Members { get; init; }

    /// <summary>
    /// Why <see cref="Probability"/> is <see langword="null"/>; <see langword="null"/> when it is not.
    /// </summary>
    [JsonPropertyName("null_reason")]
    public NullReason? NullReason { get; init; }
}
