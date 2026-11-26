// SPDX-License-Identifier: BSD-2-Clause

using System;
using System.Text.Json.Serialization;

namespace EnsembleWeather;

/// <summary>
/// The station a season's totals are measured at, and its own percent-of-median readings.
/// </summary>
public sealed class SeasonStation
{
    /// <summary>
    /// The NRCS station triplet.
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; init; } = string.Empty;

    /// <summary>
    /// One of <c>snotel</c>, <c>scan</c>.
    /// </summary>
    [JsonPropertyName("network")]
    public string Network { get; init; } = string.Empty;

    /// <summary>
    /// The name.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; init; } = string.Empty;

    /// <summary>
    /// The distance, km.
    /// </summary>
    [JsonPropertyName("distance_km")]
    public double DistanceKm { get; init; }

    /// <summary>
    /// The elevation, metres.
    /// </summary>
    [JsonPropertyName("elevation_m")]
    public double ElevationMeters { get; init; }

    /// <summary>
    /// The gauge's water-year accumulator on <c>through</c> (mm or in).
    /// </summary>
    [JsonPropertyName("precipitation_total")]
    public double? PrecipitationTotal { get; init; }

    /// <summary>
    /// The water year's precipitation as a percent of the median to date.
    /// </summary>
    [JsonPropertyName("precipitation_percent_of_median")]
    public int? PrecipitationPercentOfMedian { get; init; }

    /// <summary>
    /// Snow water equivalent on <c>through</c> (mm or in).
    /// </summary>
    [JsonPropertyName("swe")]
    public double? SnowWaterEquivalent { get; init; }

    /// <summary>
    /// The snow water equivalent as a percent of the median for the date.
    /// </summary>
    [JsonPropertyName("swe_percent_of_median")]
    public int? SnowWaterEquivalentPercentOfMedian { get; init; }

    /// <summary>
    /// Snow depth on <c>through</c> (cm or in).
    /// </summary>
    [JsonPropertyName("snow_depth")]
    public double? SnowDepth { get; init; }

    /// <summary>
    /// The gauge's newest day.
    /// </summary>
    [JsonPropertyName("through")]
    [JsonConverter(typeof(Serialization.CalendarDateConverter))]
    public DateTime Through { get; init; }
}
