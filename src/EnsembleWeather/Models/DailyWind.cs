// SPDX-License-Identifier: BSD-2-Clause

using System.Text.Json.Serialization;

namespace EnsembleWeather;

/// <summary>
/// Daily wind summary. Uses gust data when available, otherwise sustained wind.
/// </summary>
public sealed class DailyWind
{
    /// <summary>
    /// Wind severity classification based on peak gust/speed. calm: &lt;11 mph, light: 11-22, moderate: 22-34,
    /// strong: 34-49 (lifts affected), very_strong: 49-67 (most lifts closed), extreme: 67+ (all lifts closed).
    /// </summary>
    [JsonPropertyName("status")]
    public WindStatus? Status { get; init; }

    /// <summary>
    /// Peak wind for the day (mph imperial, m/s metric): the expected peak — the maximum over the day's hours of the
    /// hourly gust ensemble mean when gust data is available, otherwise of the sustained wind mean. Comparable to a
    /// deterministic provider's daily gust (NWS).
    /// </summary>
    [JsonPropertyName("peak_speed")]
    public double? PeakSpeed { get; init; }

    /// <summary>
    /// The 90th-percentile counterpart of peak_speed (the maximum over the day's hours of the hourly gust p90), a
    /// "gusts could reach" upper band. Omitted when is_gust is false.
    /// </summary>
    [JsonPropertyName("peak_speed_p90")]
    public double? PeakSpeedP90 { get; init; }

    /// <summary>
    /// Whether peak_speed is from gust data (true) or sustained wind (false)
    /// </summary>
    [JsonPropertyName("is_gust")]
    public bool? IsGust { get; init; }

    /// <summary>
    /// Peak sustained wind for the day (mph imperial, m/s metric), independent of gust. Equals peak_speed when
    /// is_gust is false. Use this for sustained-vs-sustained comparison against providers whose daily wind is
    /// sustained (e.g. NWS). Omitted when no sustained-wind ensemble was available.
    /// </summary>
    [JsonPropertyName("sustained_speed")]
    public double? SustainedSpeed { get; init; }
}
