// SPDX-License-Identifier: BSD-2-Clause

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace EnsembleWeather;

/// <summary>
/// A list, <c>/nearby</c> or favorite row: the [<c>LocationRef</c>] fields plus the county, the catalog
/// blurb and the location's named elevations.
/// </summary>
public class LocationSummary
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
    /// The primary location type, snake_case.
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
    /// Administrative county.
    /// </summary>
    [JsonPropertyName("county")]
    public string County { get; init; } = string.Empty;

    /// <summary>
    /// Free-text blurb from the catalog (when present).
    /// </summary>
    [JsonPropertyName("description")]
    public string? Description { get; init; }

    /// <summary>
    /// The location's elevation points (e.g., ski area base / mid / summit), the primary first. Exactly one entry is
    /// <c>default: true</c> -- the primary, the elevation the forecast is served at -- and it is listed once: when a
    /// named point sits at the primary's elevation the primary carries that name (<c>main_lodge</c>) instead of
    /// <c>default</c>. Every other entry omits <c>default</c>. Omitted when empty.
    /// </summary>
    [JsonPropertyName("elevations")]
    public IReadOnlyList<LocationElevation> Elevations { get; init; } = [];
}
