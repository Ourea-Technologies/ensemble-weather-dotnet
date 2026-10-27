// SPDX-License-Identifier: BSD-2-Clause

namespace EnsembleWeather;

/// <summary>Filters and paging for the location lists.</summary>
public sealed class LocationListOptions
{
    /// <summary>Gets or sets a location type to filter to.</summary>
    public LocationType? Type { get; set; }

    /// <summary>Gets or sets the unit system; <see langword="null"/> for the client's.</summary>
    public UnitSystem? Units { get; set; }

    /// <summary>Gets or sets the page size (1..5000; capped lower for anonymous and Free callers).</summary>
    public int? Limit { get; set; }

    /// <summary>Gets or sets the cursor to start from: a previous page's <c>next_cursor</c>.</summary>
    public string? Cursor { get; set; }
}
