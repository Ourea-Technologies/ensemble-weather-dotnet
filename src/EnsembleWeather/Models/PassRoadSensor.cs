// SPDX-License-Identifier: BSD-2-Clause

using System;
using System.Text.Json.Serialization;

namespace EnsembleWeather;

/// <summary>
/// One state DOT road-weather station's newest reading, in the response's units. What the station measured, never a
/// forecast. A reading the station does not take, or that the observation QC flagged, is omitted.
/// </summary>
public sealed class PassRoadSensor
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
    /// The feed: <c>rwis</c> (Caltrans CWWP2 RWIS) or <c>wsdot</c>.
    /// </summary>
    [JsonPropertyName("source")]
    public string Source { get; init; } = string.Empty;

    /// <summary>
    /// Who measured it, as the client credits it.
    /// </summary>
    [JsonPropertyName("attribution")]
    public string Attribution { get; init; } = string.Empty;

    /// <summary>
    /// Straight-line distance from the pass, km.
    /// </summary>
    [JsonPropertyName("distance_km")]
    public double DistanceKm { get; init; }

    /// <summary>
    /// The station's registry elevation in the response's length units (m, ft). Omitted when unknown.
    /// </summary>
    [JsonPropertyName("elevation")]
    public double? Elevation { get; init; }

    /// <summary>
    /// The station's registry elevation in metres. Omitted when unknown.
    /// </summary>
    [JsonPropertyName("elevation_m")]
    public double? ElevationMeters { get; init; }

    /// <summary>
    /// When the reading was taken.
    /// </summary>
    [JsonPropertyName("observed_at")]
    public DateTimeOffset ObservedAt { get; init; }

    /// <summary>
    /// <c>observed_at</c> is more than 60 minutes before <c>as_of</c>.
    /// </summary>
    [JsonPropertyName("stale")]
    public bool Stale { get; init; }

    /// <summary>
    /// The pavement state the sensor reports (NTCIP 1204 essSurfaceStatus as <c>/current</c> sensors serve it: dry,
    /// trace_moisture, wet, chemically_wet, ice_warning, ice_watch, snow_warning, snow_watch, absorption, dew,
    /// frost, absorption_at_dewpoint).
    /// </summary>
    [JsonPropertyName("pavement_condition")]
    public string? PavementCondition { get; init; }

    /// <summary>
    /// Pavement surface temperature: °C, °F (imperial) or K (kelvin).
    /// </summary>
    [JsonPropertyName("pavement_temp")]
    public double? PavementTemp { get; init; }

    /// <summary>
    /// As <c>pavement_temp</c>.
    /// </summary>
    [JsonPropertyName("subsurface_temp")]
    public double? SubsurfaceTemp { get; init; }

    /// <summary>
    /// Air temperature, as <c>pavement_temp</c>.
    /// </summary>
    [JsonPropertyName("temperature")]
    public double? Temperature { get; init; }

    /// <summary>
    /// m/s or mph (imperial).
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
    /// m, or statute miles (imperial).
    /// </summary>
    [JsonPropertyName("visibility")]
    public double? Visibility { get; init; }
}
