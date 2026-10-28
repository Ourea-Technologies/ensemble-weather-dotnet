// SPDX-License-Identifier: BSD-2-Clause

using System.Text.Json.Serialization;

namespace EnsembleWeather;

/// <summary>
/// The event behind each served probability, in the request's units, so no client hard-codes them: P(amount &gt;=
/// the threshold).
/// </summary>
public sealed class ProbabilityThresholds
{
    /// <summary>
    /// <c>ensemble.precipitation.probability</c>'s threshold (0.254 mm).
    /// </summary>
    [JsonPropertyName("hourly_precipitation")]
    public double HourlyPrecipitation { get; init; }

    /// <summary>
    /// <c>ensemble.snowfall.probability</c>'s threshold (0.254 mm).
    /// </summary>
    [JsonPropertyName("hourly_snowfall")]
    public double HourlySnowfall { get; init; }

    /// <summary>
    /// <c>daily[].precipitation_probability</c>'s day-total threshold (2.5 mm).
    /// </summary>
    [JsonPropertyName("daily_precipitation_probability")]
    public double DailyPrecipitationProbability { get; init; }

    /// <summary>
    /// <c>daily[].precipitation_occurrence_probability</c>'s threshold (1.0 mm), also the "if wet" condition.
    /// </summary>
    [JsonPropertyName("daily_precipitation_occurrence_probability")]
    public double DailyPrecipitationOccurrenceProbability { get; init; }

    /// <summary>
    /// <c>daily[].snow_probability</c>'s day-snowfall threshold (2.5 mm).
    /// </summary>
    [JsonPropertyName("daily_snow_probability")]
    public double DailySnowProbability { get; init; }
}
