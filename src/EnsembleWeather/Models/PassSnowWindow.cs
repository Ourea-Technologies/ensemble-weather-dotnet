// SPDX-License-Identifier: BSD-2-Clause

using System;
using System.Text.Json.Serialization;

namespace EnsembleWeather;

/// <summary>
/// A spell of snow at a mountain pass: when, how much, and the chance of at least an inch.
/// </summary>
public sealed class PassSnowWindow
{
    /// <summary>
    /// When the first snowing hour begins (its <c>valid_time</c> minus 1 h).
    /// </summary>
    [JsonPropertyName("start")]
    public DateTimeOffset Start { get; init; }

    /// <summary>
    /// When the last snowing hour ends (its <c>valid_time</c>).
    /// </summary>
    [JsonPropertyName("end")]
    public DateTimeOffset End { get; init; }

    /// <summary>
    /// The sum of the hourly snowfall means over the spell (mm, in), as <c>snow_outlook</c> sums its windows.
    /// </summary>
    [JsonPropertyName("total")]
    public double Total { get; init; }

    /// <summary>
    /// The p10 of the members' own totals over the spell (<c>?totals=</c> machinery); omitted without enough
    /// members.
    /// </summary>
    [JsonPropertyName("p10")]
    public double? P10 { get; init; }

    /// <summary>
    /// The p90 of the members' own totals over the spell.
    /// </summary>
    [JsonPropertyName("p90")]
    public double? P90 { get; init; }

    /// <summary>
    /// The share of members whose own total over the spell is 1 in (25.4 mm) or more; threshold fixed, does not
    /// change with units.
    /// </summary>
    [JsonPropertyName("p_ge_1in")]
    public double? PGe1In { get; init; }
}
