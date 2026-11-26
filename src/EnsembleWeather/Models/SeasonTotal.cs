// SPDX-License-Identifier: BSD-2-Clause

using System.Text.Json.Serialization;

namespace EnsembleWeather;

/// <summary>
/// A season-to-date total (precipitation or snowfall), with its coverage and its normal.
/// </summary>
public sealed class SeasonTotal
{
    /// <summary>
    /// (JSON <c>total</c>) Sum over the covered days, in <c>unit</c>.
    /// </summary>
    [JsonPropertyName("total")]
    public double Amount { get; init; }

    /// <summary>
    /// One of <c>mm</c>, <c>cm</c>, <c>in</c>.
    /// </summary>
    [JsonPropertyName("unit")]
    public string Unit { get; init; } = string.Empty;

    /// <summary>
    /// <c>mrms_multisensor</c>: the gridded analysis at the location; <c>station_gauge</c>: the location is a gauge
    /// site and this is the gauge's accumulator.
    /// </summary>
    [JsonPropertyName("source")]
    public string Source { get; init; } = string.Empty;

    /// <summary>
    /// The days the total covers.
    /// </summary>
    [JsonPropertyName("days_covered")]
    public int DaysCovered { get; init; }

    /// <summary>
    /// Days since <c>starts</c> with no complete observation; not counted as dry.
    /// </summary>
    [JsonPropertyName("days_missing")]
    public int DaysMissing { get; init; }

    /// <summary>
    /// Covered days still inside the 7-day restate window.
    /// </summary>
    [JsonPropertyName("provisional_days")]
    public int ProvisionalDays { get; init; }

    /// <summary>
    /// PRISM 1991-2020 over the covered days, in <c>unit</c>; null where no normal is loaded.
    /// </summary>
    [JsonPropertyName("normal_to_date")]
    public double? NormalToDate { get; init; }

    /// <summary>
    /// The total as a percent of normal to date.
    /// </summary>
    [JsonPropertyName("percent_of_normal")]
    public int? PercentOfNormal { get; init; }

    /// <summary>
    /// One of <c>prism_1991_2020</c>, <c>None</c>.
    /// </summary>
    [JsonPropertyName("normal_source")]
    public string? NormalSource { get; init; }
}
