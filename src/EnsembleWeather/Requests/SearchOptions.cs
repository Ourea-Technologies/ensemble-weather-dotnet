// SPDX-License-Identifier: BSD-2-Clause

namespace EnsembleWeather;

/// <summary>Hard filters and paging for a location search.</summary>
public sealed class SearchOptions
{
    /// <summary>Gets or sets a state to filter to: two-letter code or name.</summary>
    public string? State { get; set; }

    /// <summary>Gets or sets a county to filter to.</summary>
    public string? County { get; set; }

    /// <summary>Gets or sets a location type to filter to (required to find observation stations).</summary>
    public LocationType? Type { get; set; }

    /// <summary>Gets or sets the most hits wanted (1..100). The API's default is 20.</summary>
    public int? Limit { get; set; }

    /// <summary>
    /// Gets or sets a metric to sort by; <see langword="null"/> for relevance (the API's default). With a metric,
    /// each hit carries <see cref="SearchHit.Explore"/>.
    /// </summary>
    public ExploreMetric? Sort { get; set; }

    /// <summary>Gets or sets the order of a metric sort. The API's default is <see cref="SortOrder.Descending"/>.</summary>
    public SortOrder? Order { get; set; }

    /// <summary>Gets or sets a point: only hits within <see cref="RadiusKm"/> of it.</summary>
    public GeoPoint? Near { get; set; }

    /// <summary>Gets or sets the radius around <see cref="Near"/>, km (up to 1000). The API's default is 100.</summary>
    public double? RadiusKm { get; set; }

    /// <summary>Gets or sets the unit system of <see cref="SearchHit.Explore"/> values; <see langword="null"/> for the client's.</summary>
    public UnitSystem? Units { get; set; }
}
