// SPDX-License-Identifier: BSD-2-Clause

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace EnsembleWeather;

/// <summary>
/// The model runs a pinned <c>run</c> can name, and what a forecast serves now.
/// </summary>
public sealed class AvailableRunsResponse : ApiResult
{
    /// <summary>
    /// What a forecast request serves now; <c>null</c> when no cycle is servable.
    /// </summary>
    [JsonPropertyName("served")]
    public ServedRun? Served { get; init; }

    /// <summary>
    /// Init times at least two models completed, newest first (it was <c>available_runs</c>, bare strings).
    /// </summary>
    [JsonPropertyName("runs")]
    public IReadOnlyList<AvailableRun> Runs { get; init; } = [];

    /// <summary>
    /// The number of items in this response.
    /// </summary>
    [JsonPropertyName("count")]
    public int Count { get; init; }
}
