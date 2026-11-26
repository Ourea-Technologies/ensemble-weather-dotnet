// SPDX-License-Identifier: BSD-2-Clause

using System.Text.Json.Serialization;

namespace EnsembleWeather;

/// <summary>
/// Each number a summary sentence states, as served. A property is present only when the sentence states it.
/// </summary>
public sealed class SummaryValues
{
    /// <summary>
    /// The window's headline snowfall total (mm, in): for <c>snow_coming</c> the forecast's
    /// <c>snow_outlook.&lt;snowfall_window&gt;</c>; for <c>pass_snow_starts</c> the window's
    /// <c>pass.snow_windows[].total</c>.
    /// </summary>
    [JsonPropertyName("snowfall")]
    public double? Snowfall { get; init; }

    /// <summary>
    /// The window's served p10 (<c>snow_outlook.quantiles.&lt;window&gt;.p10</c>, or the pass window's <c>p10</c>).
    /// </summary>
    [JsonPropertyName("snowfall_p10")]
    public double? SnowfallP10 { get; init; }

    /// <summary>
    /// The window's served p90.
    /// </summary>
    [JsonPropertyName("snowfall_p90")]
    public double? SnowfallP90 { get; init; }

    /// <summary>
    /// Which window <c>snowfall</c> totals. One of <c>next_24h</c>, <c>next_48h</c>, <c>next_7d</c>,
    /// <c>pass_snow_window</c>.
    /// </summary>
    [JsonPropertyName("snowfall_window")]
    public string? SnowfallWindow { get; init; }

    /// <summary>
    /// A day's served high (<c>daily[].high.mean</c>) a <c>day_headline</c> or <c>temperature_change</c> states.
    /// </summary>
    [JsonPropertyName("temperature")]
    public double? Temperature { get; init; }

    /// <summary>
    /// The served hourly gust mean (<c>ensemble.wind_gust.mean</c>) a <c>wind</c> sentence states.
    /// </summary>
    [JsonPropertyName("wind_gust")]
    public double? WindGust { get; init; }

    /// <summary>
    /// The served chance (0-1) behind the sentence's likelihood word.
    /// </summary>
    [JsonPropertyName("precipitation_probability")]
    public double? PrecipitationProbability { get; init; }
}
