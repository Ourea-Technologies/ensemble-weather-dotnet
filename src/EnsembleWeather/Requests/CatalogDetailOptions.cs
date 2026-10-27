// SPDX-License-Identifier: BSD-2-Clause

using System.Collections.Generic;

namespace EnsembleWeather;

/// <summary>Opt-in extras for a catalog location or station detail.</summary>
public sealed class CatalogDetailOptions
{
    /// <summary>Gets the extras to include: <c>content</c>, <c>template</c>, <c>ads</c>.</summary>
    public IList<string> Include { get; } = new List<string>();

    /// <summary>Gets or sets the BCP-47 locale for <c>content</c>. The API's default is <c>en</c>.</summary>
    public string? Locale { get; set; }
}
