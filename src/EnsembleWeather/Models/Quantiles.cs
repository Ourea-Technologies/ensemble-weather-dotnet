// SPDX-License-Identifier: BSD-2-Clause

using System;
using System.Text.Json.Serialization;

namespace EnsembleWeather;

/// <summary>
/// The one distribution shape: every ensemble statistic the API serves (hourly <c>ensemble.*</c>, the daily
/// <c>high</c>/<c>low</c>/<c>total_*</c>, window totals, the snow outlook ranges, ...) is a <see cref="Quantiles"/>,
/// in the request's units.
/// </summary>
/// <remarks>
/// <para>
/// Every field is optional: a percentile the server did not send is <see langword="null"/>, never 0. The SDK
/// exposes exactly what the server computed from the full ensemble and never sums, averages or interpolates
/// quantiles (summing hourly percentiles does not give a window's percentiles; ask for a window total instead).
/// </para>
/// <para>
/// <c>wind_direction</c> quantiles are circular: <see cref="Max"/> may be numerically less than <see cref="Min"/>
/// (the arc crosses north). The SDK does not "fix" this.
/// </para>
/// </remarks>
public sealed class Quantiles
{
    /// <summary>
    /// Gets the ensemble mean. For skewed variables (precipitation, snowfall) the mean is pulled up by upper-tail
    /// members; prefer <see cref="P50"/> for a displayed amount.
    /// </summary>
    [JsonPropertyName("mean")]
    public double? Mean { get; init; }

    /// <summary>Gets the 10th percentile.</summary>
    [JsonPropertyName("p10")]
    public double? P10 { get; init; }

    /// <summary>Gets the 25th percentile.</summary>
    [JsonPropertyName("p25")]
    public double? P25 { get; init; }

    /// <summary>Gets the 50th percentile (the median).</summary>
    [JsonPropertyName("p50")]
    public double? P50 { get; init; }

    /// <summary>Gets the 75th percentile.</summary>
    [JsonPropertyName("p75")]
    public double? P75 { get; init; }

    /// <summary>Gets the 90th percentile.</summary>
    [JsonPropertyName("p90")]
    public double? P90 { get; init; }

    /// <summary>
    /// Gets the ensemble minimum, where the statistic has one. For circular <c>wind_direction</c> this is the
    /// anticlockwise end of an arc.
    /// </summary>
    [JsonPropertyName("min")]
    public double? Min { get; init; }

    /// <summary>
    /// Gets the ensemble maximum, where the statistic has one. For circular <c>wind_direction</c> this is the
    /// clockwise end of an arc and may be less than <see cref="Min"/>.
    /// </summary>
    [JsonPropertyName("max")]
    public double? Max { get; init; }

    /// <summary>
    /// Gets the probability of occurrence (an amount of at least a trace), on the hourly <c>precipitation</c> and
    /// <c>snowfall</c> only.
    /// </summary>
    [JsonPropertyName("probability")]
    public double? Probability { get; init; }

    /// <summary>
    /// Gets the median (<see cref="P50"/>). It is the median of the ensemble, not "the forecast": the forecast is
    /// the whole distribution.
    /// </summary>
    [JsonIgnore]
    public double? Median => P50;

    /// <summary>
    /// Gets a range the server serves: 10th to 90th (<c>GetRange(10, 90)</c>) or 25th to 75th percentile
    /// (<c>GetRange(25, 75)</c>).
    /// </summary>
    /// <param name="lowPercentile">The low percentile: 10 or 25.</param>
    /// <param name="highPercentile">The matching high percentile: 90 or 75.</param>
    /// <returns>The range, or <see langword="null"/> when the server did not send both ends.</returns>
    /// <exception cref="ArgumentException">
    /// The pair is not one the server serves. The SDK never interpolates another percentile.
    /// </exception>
    public QuantileRange? GetRange(int lowPercentile, int highPercentile)
    {
        (double? low, double? high) = (lowPercentile, highPercentile) switch
        {
            (10, 90) => (P10, P90),
            (25, 75) => (P25, P75),
            _ => throw new ArgumentException(
                $"The API serves the p10-p90 and p25-p75 ranges only, not p{lowPercentile}-p{highPercentile}; the SDK never interpolates quantiles.",
                nameof(lowPercentile)),
        };
        return low is double l && high is double h ? new QuantileRange(lowPercentile, l, highPercentile, h) : null;
    }
}
