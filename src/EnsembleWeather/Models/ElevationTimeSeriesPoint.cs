// SPDX-License-Identifier: BSD-2-Clause

using System;
using System.Text.Json.Serialization;

namespace EnsembleWeather;

/// <summary>
/// One hour of an elevation forecast.
/// </summary>
public sealed class ElevationTimeSeriesPoint
{
    /// <summary>
    /// The instant this point is valid for.
    /// </summary>
    [JsonPropertyName("valid_time")]
    public DateTimeOffset ValidTime { get; init; }

    /// <summary>
    /// Hours since the model run's init.
    /// </summary>
    [JsonPropertyName("lead_time_hours")]
    public int LeadTimeHours { get; init; }

    /// <summary>
    /// Temperature.
    /// </summary>
    [JsonPropertyName("temperature")]
    public Quantiles Temperature { get; init; } = null!;

    /// <summary>
    /// Elevation minus snow level (positive = above the snow level = snow expected; negative = below it = rain)
    /// </summary>
    [JsonPropertyName("snow_level_delta")]
    public Quantiles? SnowLevelDelta { get; init; }

    /// <summary>
    /// The variant's served wind speed (<c>hourly[].ensemble.wind_speed</c>)
    /// </summary>
    [JsonPropertyName("wind_speed")]
    public Quantiles? WindSpeed { get; init; }

    /// <summary>
    /// The variant's served gust (<c>hourly[].ensemble.wind_gust</c>); absent at lead 0, as on the primary.
    /// </summary>
    [JsonPropertyName("wind_gust")]
    public Quantiles? WindGust { get; init; }

    /// <summary>
    /// The variant's served hourly precipitation (<c>hourly[].ensemble.precipitation</c>), the same shape as the
    /// primary's, including the occurrence <c>probability</c>. Prefer <c>p50</c> (or <c>probability</c>) over
    /// <c>mean</c> for display.
    /// </summary>
    [JsonPropertyName("precipitation")]
    public Quantiles Precipitation { get; init; } = null!;

    /// <summary>
    /// The machine code, as <c>TimeSeriesPoint.condition</c>.
    /// </summary>
    [JsonPropertyName("condition")]
    public string Condition { get; init; } = string.Empty;

    /// <summary>
    /// The variant's served condition (<c>hourly[].condition</c>), derived as for the primary's hour (see
    /// <c>TimeSeriesPoint.condition</c>): a dry hour above the snow level is a sky condition, not snow.
    /// </summary>
    [JsonPropertyName("condition_text")]
    public string ConditionText { get; init; } = string.Empty;

    /// <summary>
    /// The variant's served icon hint (<c>hourly[].condition_icon</c>; see <c>TimeSeriesPoint.condition_icon</c>).
    /// </summary>
    [JsonPropertyName("condition_icon")]
    public string ConditionIcon { get; init; } = string.Empty;
}
