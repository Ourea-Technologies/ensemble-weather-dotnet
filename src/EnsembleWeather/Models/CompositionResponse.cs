// SPDX-License-Identifier: BSD-2-Clause

using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace EnsembleWeather;

/// <summary>
/// <c>GET /api/v1/composition</c>.
/// </summary>
public sealed class CompositionResponse : ApiResult
{
    /// <summary>
    /// The reference init: the newest contributing cycle, a forecast's <c>model_run.initialized_at</c>.
    /// </summary>
    [JsonPropertyName("initialized_at")]
    public DateTimeOffset InitializedAt { get; init; }

    /// <summary>
    /// Each contributing model, its own cycle and its members (<c>members</c> absent only when the cycle's member
    /// count is not known).
    /// </summary>
    [JsonPropertyName("models")]
    public IReadOnlyList<ModelEntry> Models { get => field ?? []; init; } = [];

    /// <summary>
    /// Entries in <c>models</c>.
    /// </summary>
    [JsonPropertyName("model_count")]
    public int ModelCount { get; init; }

    /// <summary>
    /// The sum of <c>models[].members</c>: the members the blend draws on. Not counting the RRFS display-only
    /// members.
    /// </summary>
    [JsonPropertyName("total_members")]
    public int TotalMembers { get; init; }
}
