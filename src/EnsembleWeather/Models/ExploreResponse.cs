// SPDX-License-Identifier: BSD-2-Clause

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace EnsembleWeather;

/// <summary>
/// One page of a ranking.
/// </summary>
public sealed class ExploreResponse : ApiResult
{
    /// <summary>
    /// One of <c>base_depth</c>, <c>new_snow_24h</c>, <c>snow_today</c>, <c>snow_5d</c>, <c>snow_10d</c>,
    /// <c>snow_14d</c>, <c>precip_today</c>, <c>precip_5d</c>, <c>precip_10d</c>, <c>precip_14d</c>,
    /// <c>season_precip</c>, <c>season_precip_pct_normal</c>, <c>season_snow</c>.
    /// </summary>
    [JsonPropertyName("metric")]
    public string Metric { get; init; } = string.Empty;

    /// <summary>
    /// One of <c>desc</c>, <c>asc</c>.
    /// </summary>
    [JsonPropertyName("order")]
    public string Order { get; init; } = string.Empty;

    /// <summary>
    /// Unit system for response values. <c>kelvin</c> returns metric units with Kelvin temperatures.
    /// </summary>
    [JsonPropertyName("units")]
    public UnitSystem Units { get; init; }

    /// <summary>
    /// The unit of every <c>value</c>, <c>p10</c> and <c>p90</c>. One of <c>mm</c>, <c>cm</c>, <c>in</c>,
    /// <c>percent</c>.
    /// </summary>
    [JsonPropertyName("unit")]
    public string Unit { get; init; } = string.Empty;

    /// <summary>
    /// The rows.
    /// </summary>
    [JsonPropertyName("items")]
    public IReadOnlyList<ExploreItem> Items { get => field ?? []; init; } = [];

    /// <summary>
    /// Items on this page.
    /// </summary>
    [JsonPropertyName("count")]
    public int Count { get; init; }

    /// <summary>
    /// Pass as <c>?cursor=</c> (same other parameters) for the next page; null at the end.
    /// </summary>
    [JsonPropertyName("next_cursor")]
    public string? NextCursor { get; init; }
}
