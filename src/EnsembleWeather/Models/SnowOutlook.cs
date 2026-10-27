// SPDX-License-Identifier: BSD-2-Clause

using System.Text.Json.Serialization;

namespace EnsembleWeather;

/// <summary>
/// Snow accumulation totals for common UI windows. Values follow the request's <c>units</c> param (mm metric,
/// inches imperial). Every total is the sum of the hourly snowfall MEANS over its window (changed 2026-09-24 from
/// summed medians): the mean is the only statistic that adds across hours, and it is what every client sums, so
/// <c>next_24h</c> equals the sum of <c>timeseries[0..24]</c> snowfall means. Pair with
/// <c>daily[].snow_probability</c> for occurrence. The windows open at the first served hour and are read from
/// the forecast whatever <c>max_hours</c> or <c>max_days</c> is. A window the forecast does not reach (the model
/// run ends before it closes) is <c>null</c> -- never the shorter sum of the hours there are, which was served as
/// the whole window.
/// </summary>
public sealed class SnowOutlook
{
    /// <summary>
    /// Sum of the hourly snowfall means over the 24 h from the first served hour; <c>null</c> when the forecast does
    /// not reach its end.
    /// </summary>
    [JsonPropertyName("next_24h")]
    public double? Next24Hours { get; init; }

    /// <summary>
    /// Sum of the hourly snowfall means over the first 48 h; <c>null</c> when not covered.
    /// </summary>
    [JsonPropertyName("next_48h")]
    public double? Next48Hours { get; init; }

    /// <summary>
    /// Sum of the hourly snowfall means over the first 168 h; <c>null</c> when not covered.
    /// </summary>
    [JsonPropertyName("next_7d")]
    public double? Next7Days { get; init; }

    /// <summary>
    /// Snow expected overnight, local 16:00 today → 09:00 tomorrow: the sum of the hourly snowfall means in the
    /// window, read from the forecast whatever <c>max_hours</c> is. Null when the window isn't covered by the model
    /// run, with <c>nowcast=false</c> (it is wall-clock-anchored), and null when any hour inside the window was
    /// refused — a fixed window summed across a gap under-counts by exactly the missing hours while still looking
    /// complete, so the headline is withheld instead.
    /// </summary>
    [JsonPropertyName("overnight_4pm_9am")]
    public double? Overnight4PmTo9Am { get; init; }

    /// <summary>
    /// The distribution of the window totals.
    /// </summary>
    [JsonPropertyName("quantiles")]
    public SnowOutlookQuantiles? Quantiles { get; init; }
}
