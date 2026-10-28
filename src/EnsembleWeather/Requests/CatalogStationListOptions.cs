// SPDX-License-Identifier: BSD-2-Clause

using System.Collections.Generic;

namespace EnsembleWeather;

/// <summary>Filters and paging for the catalog station list.</summary>
public sealed class CatalogStationListOptions
{
    /// <summary>Gets or sets an ISO 3166-2 admin1 code to filter to, for example <c>US-OR</c>.</summary>
    public string? Admin1 { get; set; }

    /// <summary>Gets or sets an ISO 3166-1 alpha-2 country code to filter to.</summary>
    public string? Country { get; set; }

    /// <summary>Gets the networks to filter to, snake_case, for example <c>snotel</c> or <c>rwis_wsdot</c>.</summary>
    public IList<string> Networks { get; } = new List<string>();

    /// <summary>Gets the capabilities every station must have, for example <c>swe</c>.</summary>
    public IList<string> Capabilities { get; } = new List<string>();

    /// <summary>Gets or sets a station status to filter to, snake_case, for example <c>active</c>.</summary>
    public string? Status { get; set; }

    /// <summary>Gets or sets the page size (1..100). The API's default is 20.</summary>
    public int? Limit { get; set; }

    /// <summary>Gets or sets the cursor to start from: a previous page's <c>next_cursor</c>.</summary>
    public string? Cursor { get; set; }
}
