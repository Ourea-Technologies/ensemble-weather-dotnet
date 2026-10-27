// SPDX-License-Identifier: BSD-2-Clause

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace EnsembleWeather;

/// <summary>
/// One page of the location list.
/// </summary>
public sealed class LocationsResponse : ApiResult
{
    /// <summary>
    /// Display label for the scope: <c>Oregon</c>, <c>Washington</c>, <c>California</c> for a single-state query, or
    /// <c>all</c> when hitting <c>/api/v1/locations</c>.
    /// </summary>
    [JsonPropertyName("state")]
    public string State { get; init; } = string.Empty;

    /// <summary>
    /// The number of items in this response.
    /// </summary>
    [JsonPropertyName("count")]
    public int Count { get; init; }

    /// <summary>
    /// The locations, nearest first where a distance applies.
    /// </summary>
    [JsonPropertyName("locations")]
    public IReadOnlyList<LocationSummary> Locations { get; init; } = [];

    /// <summary>
    /// Present only on a paged response (<c>limit</c> given) that has more rows after it. Pass it back as
    /// <c>cursor</c>.
    /// </summary>
    [JsonPropertyName("next_cursor")]
    public string? NextCursor { get; init; }
}
