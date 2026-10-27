// SPDX-License-Identifier: BSD-2-Clause

using System.Text.Json.Serialization;

namespace EnsembleWeather;

/// <summary>
/// ONE location reference, the same wherever a place is named -- the forecast's and <c>/current</c>'s
/// <c>location</c>, every list and <c>/nearby</c> row, search hits and favorites. It replaced the forecast's
/// <c>LocationInfo</c> (<c>name</c> = display name, <c>elevation</c> in the caller's units), the list row's
/// <c>name</c> = slug / <c>display_name</c>, search's <c>types[]</c>, and the <c>elevation</c>/<c>elevation_m</c>
/// spellings.
/// </summary>
public sealed class LocationReference
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
}
