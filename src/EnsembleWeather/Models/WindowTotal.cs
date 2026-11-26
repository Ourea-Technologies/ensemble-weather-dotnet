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
    /// (JSON <c>window_start_hours</c>) The window start, in hours from the first served hour.
    /// </summary>
    [JsonPropertyName("window_start_hours")]
    public int StartHours { get; init; }

    /// <summary>
    /// (JSON <c>window_end_hours</c>) The window end, in hours from the first served hour.
    /// </summary>
    [JsonPropertyName("window_end_hours")]
    public int EndHours { get; init; }

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

    /// <summary>
    /// -- how <c>quantiles</c> was computed; <c>null</c> exactly when <c>quantiles</c> is. <c>members</c>: each
    /// member's own aggregate over the window, then quantiles across the members. <c>ensemble_moments</c>: a
    /// <c>wind_speed</c> <c>max</c> that fewer than two hourly members cover (today every window), ESTIMATED from
    /// NOAA REFS's hourly 10 m wind mean and spread (it ships no members), assuming each member keeps its rank
    /// through the window: each hour's mean and spread are first calibrated against station winds (a shift, and a
    /// scale from a floor, REFS's spread and the speed itself, per lead range 1-24 h / 25-60 h), then each quantile
    /// is the largest <c>location + z * scale</c> over the window's hours, the mean that rule integrated. A
    /// statistical estimate from ensemble summaries, not a maximum over member traces. Held-out METAR verification
    /// (24 h windows, leads 1-48): p10-p90 coverage 0.81 against the observed hourly sustained wind's maximum
    /// (0.75-0.77 raw), median p50 within 0.02 m/s. REFS reaches 60 h from its cycle: a window past it is
    /// <c>null</c> with <c>too_few_members</c>.
    /// </summary>
    [JsonPropertyName("method")]
    public string? Method { get; init; }
}
