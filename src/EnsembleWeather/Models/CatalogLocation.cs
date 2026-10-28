// SPDX-License-Identifier: BSD-2-Clause

using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EnsembleWeather;

/// <summary>
/// A catalog location's PUBLIC projection -- the one [<c>LocationRef</c>] plus the catalog's public content, by
/// an allowlist. The CMS internals the public catalog used to serve (row ids, <c>created_by</c>,
/// <c>updated_by</c>, <c>created_at</c>, <c>updated_at</c> -- which moved on every pageview -- <c>deleted_at</c>,
/// <c>is_published</c>, <c>enrichment_tier</c>, <c>view_count_*</c>, <c>template_variant</c>) are not served.
/// </summary>
public class CatalogLocation
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
    /// Notes on the local microclimate.
    /// </summary>
    [JsonPropertyName("microclimate_notes")]
    public string? MicroclimateNotes { get; init; }

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
    /// The best seasons to visit.
    /// </summary>
    [JsonPropertyName("best_seasons")]
    public IReadOnlyList<string>? BestSeasons { get; init; }

    /// <summary>
    /// Key weather factors.
    /// </summary>
    [JsonPropertyName("key_factors")]
    public IReadOnlyList<string>? KeyFactors { get; init; }

    /// <summary>
    /// Tags.
    /// </summary>
    [JsonPropertyName("tags")]
    public IReadOnlyList<string>? Tags { get; init; }

    /// <summary>
    /// Free-form JSON of labelled links.
    /// </summary>
    [JsonPropertyName("external_links")]
    public JsonElement? ExternalLinks { get; init; }

    /// <summary>
    /// Whether the entry is featured.
    /// </summary>
    [JsonPropertyName("is_featured")]
    public bool? IsFeatured { get; init; }
}
