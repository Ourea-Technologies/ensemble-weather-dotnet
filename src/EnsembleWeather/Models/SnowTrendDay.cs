// SPDX-License-Identifier: BSD-2-Clause

using System;
using System.Text.Json.Serialization;

namespace EnsembleWeather;

/// <summary>
/// One day of one run in a snow-trend grid.
/// </summary>
public sealed class SnowTrendDay
{
    /// <summary>
    /// The <c>dates</c> entry this day is for.
    /// </summary>
    [JsonPropertyName("date")]
    [JsonConverter(typeof(Serialization.CalendarDateConverter))]
    public DateTime Date { get; init; }

    /// <summary>
    /// (JSON <c>snow</c>) The day's snowfall in mm (metric) or inches (imperial), at the served amount precision. <c>ourea</c>: the
    /// day card's <c>precipitation_display.snow_amount</c>, the day's mean snowfall, <c>null</c> where the card has
    /// none. A model: the median of its members' day totals; <c>null</c> only where no member has a value.
    /// </summary>
    [JsonPropertyName("snow")]
    public double? Amount { get; init; }

    /// <summary>
    /// The run covers only part of this local day.
    /// </summary>
    [JsonPropertyName("partial")]
    public bool Partial { get; init; }

    /// <summary>
    /// The range's low end. <c>ourea</c>: <c>total_snowfall.p10</c>, the day's own 10th percentile (not given a wet
    /// day), present exactly when <c>snow</c> is. A model: its members' 10th percentile, absent with fewer than two
    /// members.
    /// </summary>
    [JsonPropertyName("p10")]
    public double? P10 { get; init; }

    /// <summary>
    /// The range's high end, as <c>p10</c>.
    /// </summary>
    [JsonPropertyName("p90")]
    public double? P90 { get; init; }

    /// <summary>
    /// <c>ourea</c> only: the day's <c>precipitation_display.kind</c>. One of <c>none</c>, <c>rain</c>,
    /// <c>snow</c>, <c>rain_snow_mix</c>, <c>freezing_rain</c>.
    /// </summary>
    [JsonPropertyName("kind")]
    public string? Kind { get; init; }
}
