// SPDX-License-Identifier: BSD-2-Clause

using System.Text.Json.Serialization;

namespace EnsembleWeather;

/// <summary>
/// How unusual the day is: where the served headline (<c>p50</c>) falls in the day's climatological distribution,
/// as a percentile 0-100, <c>Pr{climate &lt;= forecast}</c> -- 95 for <c>precipitation</c> reads "wetter than 95%
/// of this date's days". <c>high</c> and <c>low</c> read the normal's Gaussian (mean and standard deviation of the
/// calendar-day maximum / minimum); <c>precipitation</c> reads its zero-inflated gamma (a dry atom plus a gamma
/// fitted to the wet days), so a dry forecast sits AT the atom and reads the climatological share of dry days. A
/// member is absent where its normal is, or where that normal has no distribution to place a value in (no fitted
/// mixture, zero spread). Absent as <c>departure</c> is.
/// </summary>
public sealed class DayPercentile
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

    /// <summary>
    /// Precipitation.
    /// </summary>
    [JsonPropertyName("precipitation")]
    public double? Precipitation { get; init; }
}
