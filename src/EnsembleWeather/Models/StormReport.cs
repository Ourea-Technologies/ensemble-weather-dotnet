// SPDX-License-Identifier: BSD-2-Clause

using System;
using System.Text.Json.Serialization;

namespace EnsembleWeather;

/// <summary>
/// A verified storm event summary published to the public <c>/api/v1/storm-reports</c> surface. Pairs forecast vs
/// observed totals with a composite skill score and headline, ready for display.
/// </summary>
public sealed class StormReport : ApiResult
{
    /// <summary>
    /// The identifier.
    /// </summary>
    [JsonPropertyName("id")]
    public long Id { get; init; }

    /// <summary>
    /// The location name.
    /// </summary>
    [JsonPropertyName("location_name")]
    public string LocationName { get; init; } = string.Empty;

    /// <summary>
    /// Precipitation regime (e.g., <c>snowfall</c>, <c>rainfall</c>, <c>atmospheric_river</c>).
    /// </summary>
    [JsonPropertyName("event_type")]
    public string EventType { get; init; } = string.Empty;

    /// <summary>
    /// When the event began.
    /// </summary>
    [JsonPropertyName("onset_time")]
    public DateTimeOffset OnsetTime { get; init; }

    /// <summary>
    /// When the event ended.
    /// </summary>
    [JsonPropertyName("end_time")]
    public DateTimeOffset EndTime { get; init; }

    /// <summary>
    /// The forecast precipitation total, mm.
    /// </summary>
    [JsonPropertyName("forecast_total_mm")]
    public double ForecastTotalMm { get; init; }

    /// <summary>
    /// The observed precipitation total, mm.
    /// </summary>
    [JsonPropertyName("observed_total_mm")]
    public double ObservedTotalMm { get; init; }

    /// <summary>
    /// The forecast snowfall total, mm.
    /// </summary>
    [JsonPropertyName("forecast_snowfall_mm")]
    public double? ForecastSnowfallMm { get; init; }

    /// <summary>
    /// The observed snowfall total, mm.
    /// </summary>
    [JsonPropertyName("observed_snowfall_mm")]
    public double? ObservedSnowfallMm { get; init; }

    /// <summary>
    /// Model with the highest composite score for this event.
    /// </summary>
    [JsonPropertyName("best_model")]
    public string? BestModel { get; init; }

    /// <summary>
    /// The best model's lead time, hours.
    /// </summary>
    [JsonPropertyName("best_lead_time_hours")]
    public int? BestLeadTimeHours { get; init; }

    /// <summary>
    /// 0-1 skill score (higher = better).
    /// </summary>
    [JsonPropertyName("composite_score")]
    public double? CompositeScore { get; init; }

    /// <summary>
    /// The headline text.
    /// </summary>
    [JsonPropertyName("headline")]
    public string? Headline { get; init; }
}
