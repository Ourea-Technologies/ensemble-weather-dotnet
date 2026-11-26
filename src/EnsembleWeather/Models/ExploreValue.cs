// SPDX-License-Identifier: BSD-2-Clause

using System;
using System.Text.Json.Serialization;

namespace EnsembleWeather;

/// <summary>
/// A search hit's Explore value; the fields mean what an <c>ExploreItem</c>'s do.
/// </summary>
public sealed class ExploreValue
{
    /// <summary>
    /// One of <c>base_depth</c>, <c>new_snow_24h</c>, <c>snow_today</c>, <c>snow_5d</c>, <c>snow_10d</c>,
    /// <c>snow_14d</c>, <c>precip_today</c>, <c>precip_5d</c>, <c>precip_10d</c>, <c>precip_14d</c>,
    /// <c>season_precip</c>, <c>season_precip_pct_normal</c>, <c>season_snow</c>.
    /// </summary>
    [JsonPropertyName("metric")]
    public string Metric { get; init; } = string.Empty;

    /// <summary>
    /// The value, in the response's units.
    /// </summary>
    [JsonPropertyName("value")]
    public double Value { get; init; }

    /// <summary>
    /// One of <c>mm</c>, <c>cm</c>, <c>in</c>, <c>percent</c>.
    /// </summary>
    [JsonPropertyName("unit")]
    public string Unit { get; init; } = string.Empty;

    /// <summary>
    /// The 10th percentile.
    /// </summary>
    [JsonPropertyName("p10")]
    public double? P10 { get; init; }

    /// <summary>
    /// The 90th percentile.
    /// </summary>
    [JsonPropertyName("p90")]
    public double? P90 { get; init; }

    /// <summary>
    /// When the value was computed.
    /// </summary>
    [JsonPropertyName("as_of")]
    public DateTimeOffset AsOf { get; init; }

    /// <summary>
    /// The model run's initialisation time.
    /// </summary>
    [JsonPropertyName("initialized_at")]
    public DateTimeOffset? InitializedAt { get; init; }

    /// <summary>
    /// The data source.
    /// </summary>
    [JsonPropertyName("source")]
    public string Source { get; init; } = string.Empty;
}
