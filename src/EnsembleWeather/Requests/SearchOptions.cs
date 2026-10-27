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
}
