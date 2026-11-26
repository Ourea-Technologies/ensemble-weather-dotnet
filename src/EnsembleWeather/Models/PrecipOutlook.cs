// SPDX-License-Identifier: BSD-2-Clause

using System.Text.Json.Serialization;

namespace EnsembleWeather;

/// <summary>
/// Rolling liquid-equivalent totals over the same windows as <c>snow_outlook</c>, for places where rain is the
/// story (the favorites cards' "0.6 in rain" with its range). Values follow <c>units</c> (mm metric, inches
/// imperial). Each total is the sum of the hourly precipitation means over the window (means add across hours),
/// read from the forecast whatever <c>max_hours</c> is; a window the forecast does not reach is <c>null</c>.
/// <c>quantiles</c> holds each window's range: the percentiles of every member's own precipitation total over the
/// same hours, and those members' own mean -- one distribution, never a sum of hourly percentiles. The members
/// carry the calibrated water (ECC through the headline's fit), so that mean is close to the total but not equal to
/// it; show the range with the total.
/// </summary>
public sealed class PrecipOutlook
{
    /// <summary>
    /// Sum of the hourly precipitation means over the 24 h from the first served hour; <c>null</c> when not
    /// covered.
    /// </summary>
    [JsonPropertyName("next_24h")]
    public double? Next24Hours { get; init; }

    /// <summary>
    /// The same over 48 h.
    /// </summary>
    [JsonPropertyName("next_48h")]
    public double? Next48Hours { get; init; }

    /// <summary>
    /// The same over 72 h.
    /// </summary>
    [JsonPropertyName("next_72h")]
    public double? Next72Hours { get; init; }

    /// <summary>
    /// The same over 168 h.
    /// </summary>
    [JsonPropertyName("next_7d")]
    public double? Next7Days { get; init; }

    /// <summary>
    /// The RANGE of each <c>snow_outlook</c> window's snowfall total, on the default response (no <c>members</c>
    /// needed) and on every <c>elevation_forecasts[].snow_outlook</c>. The percentiles are those of every member's
    /// OWN total over the window -- the same hours the headline sums (valid times in [first served hour, + N h),
    /// each covering the hour ending at it; a 6-hourly bucket straddling an end counts its share inside) -- never a
    /// sum of hourly percentiles (quantiles are not additive). Members are pooled across models, one vote each, and
    /// a member counts only when its values cover the WHOLE window (a model whose run ends inside it is left out).
    /// <c>Mean</c> is the mean of those same member totals, so the mean and the percentiles are one distribution
    /// (as <c>window_totals</c> from <c>?totals=snowfall:&lt;window&gt;</c> serves it). Until 2026-11-20 it was the
    /// window's headline total (<c>snow_outlook.next_24h</c>...) written beside them, which could lie under what
    /// the percentiles allow or above <c>max</c>; the headline total is unchanged and is no longer repeated here.
    /// Show it as "<c>mean</c>, at least <c>p10</c>, could reach <c>p90</c>". A window is null when the headline
    /// total is, or when fewer than two members cover it; the object is omitted when every window is null.
    /// </summary>
    [JsonPropertyName("quantiles")]
    public SnowOutlookQuantiles? Quantiles { get; init; }
}
