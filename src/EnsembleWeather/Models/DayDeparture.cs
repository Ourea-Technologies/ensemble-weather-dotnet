// SPDX-License-Identifier: BSD-2-Clause

using System.Text.Json.Serialization;

namespace EnsembleWeather;

/// <summary>
/// The served headline minus the normal, in the request's units: <c>high.p50 - normal.high</c> and <c>low.p50 -
/// normal.low</c>. Positive is warmer than normal. Present only with <c>normal</c>, and absent on a date the
/// forecast covers only from partway through (the first date of a run that starts after local midnight), whose high
/// and low are over part of the day while the normal is over all of it.
/// </summary>
public sealed class DayDeparture
{
    /// <summary>
    /// The day's high.
    /// </summary>
    [JsonPropertyName("high")]
    public double? High { get; init; }

    /// <summary>
    /// The day's low.
    /// </summary>
    [JsonPropertyName("low")]
    public double? Low { get; init; }
}
