// SPDX-License-Identifier: BSD-2-Clause

using System;
using System.Text.Json.Serialization;

namespace EnsembleWeather;

/// <summary>
/// One <c>/current</c> sensor -- its latest readings, in the request's <c>units</c> (the Cap'n Proto
/// <c>SensorObservation</c> carries the same values). A reading the sensor does not take is omitted.
/// </summary>
public sealed class SensorObservation
{
    /// <summary>
    /// The station's id in its network.
    /// </summary>
    [JsonPropertyName("station_id")]
    public string StationId { get; init; } = string.Empty;

    /// <summary>
    /// The station's registry name, else its id.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; init; } = string.Empty;

    /// <summary>
    /// The observation source network.
    /// </summary>
    [JsonPropertyName("source")]
    public string Source { get; init; } = string.Empty;

    /// <summary>
    /// When the observation was taken.
    /// </summary>
    [JsonPropertyName("observation_time")]
    public DateTimeOffset ObservationTime { get; init; }

    /// <summary>
    /// °C, °F (imperial) or K (kelvin).
    /// </summary>
    [JsonPropertyName("temperature")]
    public double? Temperature { get; init; }

    /// <summary>
    /// As <c>temperature</c>.
    /// </summary>
    [JsonPropertyName("dewpoint")]
    public double? Dewpoint { get; init; }

    /// <summary>
    /// Percent.
    /// </summary>
    [JsonPropertyName("relative_humidity")]
    public double? RelativeHumidity { get; init; }

    /// <summary>
    /// M/s or mph (imperial).
    /// </summary>
    [JsonPropertyName("wind_speed")]
    public double? WindSpeed { get; init; }

    /// <summary>
    /// As <c>wind_speed</c>.
    /// </summary>
    [JsonPropertyName("wind_gust")]
    public double? WindGust { get; init; }

    /// <summary>
    /// Degrees clockwise from north.
    /// </summary>
    [JsonPropertyName("wind_direction")]
    public double? WindDirection { get; init; }

    /// <summary>
    /// Station pressure: hPa, or inHg (imperial).
    /// </summary>
    [JsonPropertyName("pressure")]
    public double? Pressure { get; init; }

    /// <summary>
    /// The last reporting period's precipitation: mm, or in (imperial).
    /// </summary>
    [JsonPropertyName("precipitation")]
    public double? Precipitation { get; init; }

    /// <summary>
    /// Radar precipitation rate: mm/h, or in/h (imperial).
    /// </summary>
    [JsonPropertyName("precip_rate")]
    public double? PrecipitationRate { get; init; }

    /// <summary>
    /// Radar reflectivity, dBZ.
    /// </summary>
    [JsonPropertyName("reflectivity")]
    public double? Reflectivity { get; init; }

    /// <summary>
    /// As <c>temperature</c>.
    /// </summary>
    [JsonPropertyName("pavement_temp")]
    public double? PavementTemp { get; init; }

    /// <summary>
    /// As <c>temperature</c>.
    /// </summary>
    [JsonPropertyName("subsurface_temp")]
    public double? SubsurfaceTemp { get; init; }

    /// <summary>
    /// M, or ft (imperial).
    /// </summary>
    [JsonPropertyName("visibility")]
    public double? Visibility { get; init; }

    /// <summary>
    /// Percent.
    /// </summary>
    [JsonPropertyName("cloud_cover")]
    public double? CloudCover { get; init; }

    /// <summary>
    /// M, or ft (imperial).
    /// </summary>
    [JsonPropertyName("ceiling_height")]
    public double? CeilingHeight { get; init; }

    /// <summary>
    /// Cm, or in (imperial).
    /// </summary>
    [JsonPropertyName("snow_depth")]
    public double? SnowDepth { get; init; }

    /// <summary>
    /// Snow water equivalent: mm, or in (imperial).
    /// </summary>
    [JsonPropertyName("swe")]
    public double? SnowWaterEquivalent { get; init; }

    /// <summary>
    /// SWE as a percent of the station's median for the date.
    /// </summary>
    [JsonPropertyName("swe_percent_of_median")]
    public double? SnowWaterEquivalentPercentOfMedian { get; init; }

    /// <summary>
    /// Mm, or in (imperial).
    /// </summary>
    [JsonPropertyName("snowfall")]
    public double? Snowfall { get; init; }

    /// <summary>
    /// W/m².
    /// </summary>
    [JsonPropertyName("solar_radiation")]
    public double? SolarRadiation { get; init; }

    /// <summary>
    /// Road-weather pavement state as the network reports it.
    /// </summary>
    [JsonPropertyName("pavement_condition")]
    public string? PavementCondition { get; init; }
}
