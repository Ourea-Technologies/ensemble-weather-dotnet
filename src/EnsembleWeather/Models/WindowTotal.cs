// SPDX-License-Identifier: BSD-2-Clause

using System;
using System.Text.Json.Serialization;

namespace EnsembleWeather;

/// <summary>
/// One <c>?totals=</c> window, across the members.
/// </summary>
public sealed class WindowTotal
{
    /// <summary>
    /// The term's canonical spelling, as the SDK sent it (<c>snowfall:72h</c>): the key the SDK matches the caller's
    /// term by.
    /// </summary>
    [JsonPropertyName("term")]
    public string Term { get; init; } = string.Empty;

    /// <summary>
    /// The forecast variable.
    /// </summary>
    [JsonPropertyName("variable")]
    public ForecastVariable Variable { get; init; }

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
    /// The members' window totals, counted as the headline counts its windows (a straddling bucket by its share
    /// inside; only members covering the whole window); <c>null</c> when the window is not covered or fewer than two
    /// members cover it.
    /// </summary>
    [JsonPropertyName("quantiles")]
    public Quantiles? Quantiles { get; init; }

    /// <summary>
    /// The number of ensemble members that contributed.
    /// </summary>
    [JsonPropertyName("members")]
    public int Members { get; init; }

    /// <summary>
    /// What each member contributes: <see cref="WindowTotalAggregation.Total"/> for precipitation and snowfall.
    /// </summary>
    [JsonPropertyName("aggregation")]
    public WindowTotalAggregation Aggregation { get; init; }

    /// <summary>
    /// Why <see cref="Quantiles"/> is <see langword="null"/>; <see langword="null"/> when it is not.
    /// </summary>
    [JsonPropertyName("null_reason")]
    public NullReason? NullReason { get; init; }
}
