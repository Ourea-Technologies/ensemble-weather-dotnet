// SPDX-License-Identifier: BSD-2-Clause

using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EnsembleWeather;

/// <summary>
/// A catalog station's PUBLIC projection -- the [<c>LocationRef</c>] fields (<c>type</c> is
/// <c>observation_station</c>) plus the station's public facts, by an allowlist; the CMS internals are not
/// served.
/// </summary>
public sealed class CatalogStationDetail : ApiResult
{
    /// <summary>
    /// The location's key: its catalog slug. With <c>state</c>, the URL <c>/api/v1/locations/{state}/{slug}</c>; what
    /// a favorite stores.
    /// </summary>
    [JsonPropertyName("slug")]
    public string Slug { get; init; } = string.Empty;

    /// <summary>
    /// The lowercase USPS state code.
    /// </summary>
    [JsonPropertyName("state")]
    public string State { get; init; } = string.Empty;

    /// <summary>
    /// The display name.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; init; } = string.Empty;

    /// <summary>
    /// Latitude, decimal degrees (WGS84).
    /// </summary>
    [JsonPropertyName("lat")]
    public double Latitude { get; init; }

    /// <summary>
    /// Longitude, decimal degrees (WGS84, west negative).
    /// </summary>
    [JsonPropertyName("lon")]
    public double Longitude { get; init; }

    /// <summary>
    /// Elevation in metres, whatever <c>units</c> is (0.1 m).
    /// </summary>
    [JsonPropertyName("elevation_m")]
    public double ElevationMeters { get; init; }

    /// <summary>
    /// The primary location type, snake_case (D-5).
    /// </summary>
    [JsonPropertyName("type")]
    public LocationType Type { get; init; }

    /// <summary>
    /// The IANA zone the location's local days and hours are kept in -- the zone <c>daily</c> dates, sunrise and
    /// sunset are computed in. Label the hourly series in it (/1080).
    /// </summary>
    [JsonPropertyName("timezone")]
    public string Timezone { get; init; } = string.Empty;

    /// <summary>
    /// The county name.
    /// </summary>
    [JsonPropertyName("county")]
    public string? County { get; init; }

    /// <summary>
    /// The ISO 3166-1 alpha-2 country code.
    /// </summary>
    [JsonPropertyName("country")]
    public string? Country { get; init; }

    /// <summary>
    /// Other names this location answers to.
    /// </summary>
    [JsonPropertyName("aliases")]
    public IReadOnlyList<string>? Aliases { get; init; }

    /// <summary>
    /// The observation network.
    /// </summary>
    [JsonPropertyName("network")]
    public string? Network { get; init; }

    /// <summary>
    /// The station id in its source network.
    /// </summary>
    [JsonPropertyName("source_station_id")]
    public string? SourceStationId { get; init; }

    /// <summary>
    /// Elevation above mean sea level, metres.
    /// </summary>
    [JsonPropertyName("elevation_msl_m")]
    public double? ElevationMslMeters { get; init; }

    /// <summary>
    /// Sensor height above ground, metres.
    /// </summary>
    [JsonPropertyName("sensor_height_agl_m")]
    public double? SensorHeightAglMeters { get; init; }

    /// <summary>
    /// What the station measures.
    /// </summary>
    [JsonPropertyName("sensor_capabilities")]
    public IReadOnlyList<string>? SensorCapabilities { get; init; }

    /// <summary>
    /// The station status.
    /// </summary>
    [JsonPropertyName("station_status")]
    public string? StationStatus { get; init; }

    /// <summary>
    /// When the station last reported.
    /// </summary>
    [JsonPropertyName("last_report_at")]
    public DateTimeOffset? LastReportAt { get; init; }

    /// <summary>
    /// Free-form JSON of the last report.
    /// </summary>
    [JsonPropertyName("last_report_summary")]
    public JsonElement? LastReportSummary { get; init; }

    /// <summary>
    /// When the station was installed.
    /// </summary>
    [JsonPropertyName("install_date")]
    [JsonConverter(typeof(Serialization.CalendarDateConverter))]
    public DateTime? InstallDate { get; init; }

    /// <summary>
    /// When the station was decommissioned.
    /// </summary>
    [JsonPropertyName("decommission_date")]
    [JsonConverter(typeof(Serialization.CalendarDateConverter))]
    public DateTime? DecommissionDate { get; init; }

    /// <summary>
    /// How often the station reports, minutes.
    /// </summary>
    [JsonPropertyName("update_frequency_minutes")]
    public int? UpdateFrequencyMinutes { get; init; }

    /// <summary>
    /// The station's page.
    /// </summary>
    [JsonPropertyName("station_url")]
    public string? StationUrl { get; init; }

    /// <summary>
    /// The station's data page.
    /// </summary>
    [JsonPropertyName("data_url")]
    public string? DataUrl { get; init; }

    /// <summary>
    /// A short description.
    /// </summary>
    [JsonPropertyName("description_short")]
    public string? DescriptionShort { get; init; }

    /// <summary>
    /// A long description.
    /// </summary>
    [JsonPropertyName("description_long")]
    public string? DescriptionLong { get; init; }

    /// <summary>
    /// The page title.
    /// </summary>
    [JsonPropertyName("meta_title")]
    public string? MetaTitle { get; init; }

    /// <summary>
    /// The page meta description.
    /// </summary>
    [JsonPropertyName("meta_description")]
    public string? MetaDescription { get; init; }

    /// <summary>
    /// Tags.
    /// </summary>
    [JsonPropertyName("tags")]
    public IReadOnlyList<string>? Tags { get; init; }

    /// <summary>
    /// Whether the entry is featured.
    /// </summary>
    [JsonPropertyName("is_featured")]
    public bool? IsFeatured { get; init; }

    /// <summary>
    /// Photos.
    /// </summary>
    [JsonPropertyName("photos")]
    public IReadOnlyList<CatalogPhoto> Photos { get; init; } = [];

    /// <summary>
    /// Webcams.
    /// </summary>
    [JsonPropertyName("webcams")]
    public IReadOnlyList<CatalogWebcam> Webcams { get; init; } = [];

    /// <summary>
    /// The linked primary location, when any.
    /// </summary>
    [JsonPropertyName("primary_location")]
    public CatalogPrimaryLocation? PrimaryLocation { get; init; }

    /// <summary>
    /// The resolved page template (<c>include=template</c>).
    /// </summary>
    [JsonPropertyName("template")]
    public CatalogTemplateSummary? Template { get; init; }

    /// <summary>
    /// Published content entries (<c>include=content</c>).
    /// </summary>
    [JsonPropertyName("content")]
    public IReadOnlyList<CatalogResolvedEntry> Content { get; init; } = [];

    /// <summary>
    /// Resolved ad placements (<c>include=ads</c>).
    /// </summary>
    [JsonPropertyName("ads")]
    public IReadOnlyList<CatalogResolvedAdSlot> Ads { get; init; } = [];
}
