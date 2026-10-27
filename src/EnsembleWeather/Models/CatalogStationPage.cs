// SPDX-License-Identifier: BSD-2-Clause

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace EnsembleWeather;

/// <summary>
/// One cursor page of catalog observation stations.
/// </summary>
public sealed class CatalogStationPage : ApiResult
{
    /// <summary>
    /// The items on this page.
    /// </summary>
    [JsonPropertyName("items")]
    public IReadOnlyList<CatalogStation> Items { get; init; } = [];

    /// <summary>
    /// The cursor for the next page, or <see langword="null"/> on the last page.
    /// </summary>
    [JsonPropertyName("next_cursor")]
    public string? NextCursor { get; init; }
}
