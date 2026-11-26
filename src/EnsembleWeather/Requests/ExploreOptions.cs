// SPDX-License-Identifier: BSD-2-Clause

namespace EnsembleWeather;

/// <summary>
/// Filters and paging for <see cref="OureaClient.ExplorePageAsync"/>. Every option left unset is omitted, as is one
/// set to the API's default, so equal requests share one edge-cache entry.
/// </summary>
public sealed class ExploreOptions
{
    /// <summary>Gets or sets the order. The API's default is <see cref="SortOrder.Descending"/> (the most first).</summary>
    public SortOrder? Order { get; set; }

    /// <summary>Gets or sets a location type to filter to.</summary>
    public LocationType? Type { get; set; }

    /// <summary>Gets or sets a state to filter to: two-letter code (sent in lowercase) or name.</summary>
    public string? State { get; set; }

    /// <summary>Gets or sets a point: only places within <see cref="RadiusKm"/> of it.</summary>
    public GeoPoint? Near { get; set; }

    /// <summary>Gets or sets the radius around <see cref="Near"/>, km (up to 1000). The API's default is 100.</summary>
    public double? RadiusKm { get; set; }

    /// <summary>Gets or sets the lowest point elevation, in the response's height unit (m or ft).</summary>
    public double? ElevationMin { get; set; }

    /// <summary>Gets or sets the highest point elevation, in the response's height unit (m or ft).</summary>
    public double? ElevationMax { get; set; }

    /// <summary>Gets or sets which points rank. The API's default is <see cref="ExplorePoints.Primary"/>.</summary>
    public ExplorePoints? Points { get; set; }

    /// <summary>Gets or sets the rows per page (1..100). The API's default is 25.</summary>
    public int? Limit { get; set; }

    /// <summary>
    /// Gets or sets a previous page's <see cref="ExploreResponse.NextCursor"/>; or use
    /// <see cref="OureaClient.ExploreAsync"/>, which follows the cursor itself.
    /// </summary>
    public string? Cursor { get; set; }

    /// <summary>Gets or sets the unit system; <see langword="null"/> for the client's.</summary>
    public UnitSystem? Units { get; set; }
}
