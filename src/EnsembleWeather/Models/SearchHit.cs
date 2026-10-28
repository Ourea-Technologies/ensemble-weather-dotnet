// SPDX-License-Identifier: BSD-2-Clause

using System.Text.Json.Serialization;

namespace EnsembleWeather;

/// <summary>
/// One location search result: the [<c>LocationRef</c>] fields, the county and <c>key</c>. <c>Types[]</c>
/// is gone (<c>type</c> is the primary type).
/// </summary>
public sealed class SearchHit
{
    /// <summary>
    /// State-qualified identity <c>"&lt;state&gt;-&lt;slug&gt;"</c> (lowercase state). Bare slugs are only unique per
    /// state.
    /// </summary>
    [JsonPropertyName("key")]
    public string Key { get; init; } = string.Empty;

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
    /// The county name.
    /// </summary>
    [JsonPropertyName("county")]
    public string County { get; init; } = string.Empty;
}
