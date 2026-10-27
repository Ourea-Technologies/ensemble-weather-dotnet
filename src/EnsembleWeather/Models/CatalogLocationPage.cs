// SPDX-License-Identifier: BSD-2-Clause

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace EnsembleWeather;

/// <summary>
/// One cursor page of catalog locations.
/// </summary>
public sealed class CatalogLocationPage : ApiResult
{
    /// <summary>
    /// The items on this page.
    /// </summary>
    [JsonPropertyName("items")]
    public IReadOnlyList<CatalogLocation> Items { get; init; } = [];

    /// <summary>
    /// Pass as <c>?cursor=</c> for the next page; null at the end.
    /// </summary>
    [JsonPropertyName("next_cursor")]
    public string? NextCursor { get; init; }
}
