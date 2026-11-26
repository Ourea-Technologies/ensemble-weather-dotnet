// SPDX-License-Identifier: BSD-2-Clause

using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace EnsembleWeather;

/// <summary>
/// Hourly snow depth from <c>start_time</c> (cm or in).
/// </summary>
public sealed class SnowpackDepth
{
    /// <summary>
    /// The first column's time.
    /// </summary>
    [JsonPropertyName("start_time")]
    public DateTimeOffset StartTime { get; init; }

    /// <summary>
    /// The hours between columns.
    /// </summary>
    [JsonPropertyName("interval_hours")]
    public int IntervalHours { get; init; }

    /// <summary>
    /// <c>measured</c>: started from a sensor reading at the spot. <c>open_loop</c>: the model's own state, served
    /// with <c>p10</c> and <c>p90</c> where the caller's plan includes the range. Tolerate values you do not know.
    /// </summary>
    [JsonPropertyName("basis")]
    public string Basis { get; init; } = string.Empty;

    /// <summary>
    /// The 10th percentile.
    /// </summary>
    [JsonPropertyName("p10")]
    public IReadOnlyList<double> P10 { get => field ?? []; init; } = [];

    /// <summary>
    /// The 50th percentile (the median).
    /// </summary>
    [JsonPropertyName("p50")]
    public IReadOnlyList<double> P50 { get => field ?? []; init; } = [];

    /// <summary>
    /// The 90th percentile.
    /// </summary>
    [JsonPropertyName("p90")]
    public IReadOnlyList<double> P90 { get => field ?? []; init; } = [];
}
