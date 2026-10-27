// SPDX-License-Identifier: BSD-2-Clause

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace EnsembleWeather;

/// <summary>
/// Ranked location search hits, best first.
/// </summary>
public sealed class SearchLocationsResponse : ApiResult
{
    /// <summary>
    /// The items on this page.
    /// </summary>
    [JsonPropertyName("items")]
    public IReadOnlyList<SearchHit> Items { get; init; } = [];

    /// <summary>
    /// The number of items in this response.
    /// </summary>
    [JsonPropertyName("count")]
    public int Count { get; init; }
}
