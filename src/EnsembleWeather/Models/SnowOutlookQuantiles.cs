// SPDX-License-Identifier: BSD-2-Clause

using System.Text.Json.Serialization;

namespace EnsembleWeather;

/// <summary>
/// The RANGE of each <c>snow_outlook</c> window's snowfall total, on the default response (no <c>members</c>
/// needed) and on every <c>elevation_forecasts[].snow_outlook</c>. The percentiles are those of every member's
/// OWN total over the window -- the same hours the headline sums (valid times in [first served hour, + N h), each
/// covering the hour ending at it; a 6-hourly bucket straddling an end counts its share inside) -- never a sum of
/// hourly percentiles (quantiles are not additive). Members are pooled across models, one vote each, and a member
/// counts only when its values cover the WHOLE window (a model whose run ends inside it is left out). <c>mean</c>
/// is the window's headline total (<c>next_24h</c>...). Show it as "most likely <c>p50</c>, at least <c>p10</c>,
/// could reach <c>p90</c>". A window is null when fewer than two members cover it; the object is omitted when
/// every window is null.
/// </summary>
public sealed class SnowOutlookQuantiles
{
    /// <summary>
    /// The next 24 hours.
    /// </summary>
    [JsonPropertyName("next_24h")]
    public Quantiles? Next24Hours { get; init; }

    /// <summary>
    /// The next 48 hours.
    /// </summary>
    [JsonPropertyName("next_48h")]
    public Quantiles? Next48Hours { get; init; }

    /// <summary>
    /// The next 7 days.
    /// </summary>
    [JsonPropertyName("next_7d")]
    public Quantiles? Next7Days { get; init; }
}
