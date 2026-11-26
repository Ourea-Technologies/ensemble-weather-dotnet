// SPDX-License-Identifier: BSD-2-Clause

using System;
using System.Text.Json.Serialization;

namespace EnsembleWeather;

/// <summary>
/// One day of a season's record.
/// </summary>
public sealed class SeasonDay
{
    /// <summary>
    /// The local calendar date.
    /// </summary>
    [JsonPropertyName("date")]
    [JsonConverter(typeof(Serialization.CalendarDateConverter))]
    public DateTime Date { get; init; }

    /// <summary>
    /// The day's total; null when an hour was not observed.
    /// </summary>
    [JsonPropertyName("precipitation")]
    public double? Precipitation { get; init; }

    /// <summary>
    /// Always null for now.
    /// </summary>
    [JsonPropertyName("snowfall")]
    public double? Snowfall { get; init; }

    /// <summary>
    /// Whether the day's data is provisional (may still be revised).
    /// </summary>
    [JsonPropertyName("provisional")]
    public bool Provisional { get; init; }
}
