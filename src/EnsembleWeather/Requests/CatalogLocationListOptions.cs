// SPDX-License-Identifier: BSD-2-Clause

namespace EnsembleWeather;

/// <summary>Filters and paging for the catalog location list.</summary>
public sealed class CatalogLocationListOptions
{
    /// <summary>Gets or sets an ISO 3166-2 admin1 code to filter to, for example <c>US-OR</c>.</summary>
    public string? Admin1 { get; set; }

    /// <summary>Gets or sets an ISO 3166-1 alpha-2 country code to filter to, for example <c>US</c>.</summary>
    public string? Country { get; set; }

    /// <summary>Gets or sets a catalog location type to filter to, snake_case, for example <c>ski_area</c>.</summary>
    public string? Type { get; set; }

    /// <summary>Gets or sets a page template variant to filter to, snake_case, for example <c>mountain</c>.</summary>
    public string? Template { get; set; }

    /// <summary>Gets or sets the page size (1..100). The API's default is 20.</summary>
    public int? Limit { get; set; }

    /// <summary>Gets or sets the cursor to start from: a previous page's <c>next_cursor</c>.</summary>
    public string? Cursor { get; set; }
}
