// SPDX-License-Identifier: BSD-2-Clause

using System;
using System.Text.Json.Serialization;

namespace EnsembleWeather;

/// <summary>
/// Chances at the pass over the next <c>window_hours</c> hours, each the share of the forecast's members meeting
/// the event (the <c>?prob=</c> machinery), except the gust chance (see its <c>method</c>). Thresholds are fixed
/// product constants named in imperial (1 in, 6 in, 40 mph): they do not change with <c>units</c>; <c>threshold</c>
/// states each in the response's units (25.4 mm, 152.4 mm, 17.9 m/s).
/// </summary>
public sealed class PassChances
{
    /// <summary>
    /// The window's length, 24.
    /// </summary>
    [JsonPropertyName("window_hours")]
    public int WindowHours { get; init; }

    /// <summary>
    /// The window's first instant.
    /// </summary>
    [JsonPropertyName("from")]
    public DateTimeOffset From { get; init; }

    /// <summary>
    /// The window's end (exclusive).
    /// </summary>
    [JsonPropertyName("to")]
    public DateTimeOffset To { get; init; }

    /// <summary>
    /// Snowfall total over the window of 1 in (25.4 mm) or more: <c>prob=snowfall:ge:&lt;1 in&gt;:24h</c>.
    /// Threshold fixed, does not change with units.
    /// </summary>
    [JsonPropertyName("p_snow_ge_1in")]
    public PassChance PSnowGe1In { get; init; } = null!;

    /// <summary>
    /// Snowfall total over the window of 6 in (152.4 mm) or more. Threshold fixed, does not change with units.
    /// </summary>
    [JsonPropertyName("p_snow_ge_6in")]
    public PassChance PSnowGe6In { get; init; } = null!;

    /// <summary>
    /// Some hour of the window in which the member is at or below freezing (<c>threshold</c>: 0 C, 32 F) with
    /// precipitation of at least <c>precipitation_threshold</c> (0.254 mm, 0.01 in) in the member's value ending at
    /// that hour: a wet road at or below freezing. Weather, not a measured pavement state.
    /// </summary>
    [JsonPropertyName("p_wet_below_freezing")]
    public PassChance PWetBelowFreezing { get; init; } = null!;

    /// <summary>
    /// A gust over 40 mph (17.9 m/s; threshold fixed, does not change with units) in the window. The members carry
    /// no gust, so this is the highest hourly chance in the window, read from each hour's served calibrated gust
    /// distribution (<c>method</c> <c>hourly_gust_distribution</c>): the chance of the gustiest hour, not of any
    /// hour.
    /// </summary>
    [JsonPropertyName("p_gust_gt_40mph")]
    public PassChance PGustGt40Mph { get; init; } = null!;
}
