// SPDX-License-Identifier: BSD-2-Clause

using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace EnsembleWeather;

/// <summary>
/// The composition the forecast was blended from. The blend is per-model-latest: every model contributes its
/// freshest complete cycle within its staleness cap (aifs/aigefs/gefs 30 h, ifs_ens 36 h, rrfs 14 h), aligned by
/// VALID time, so the models are generally at DIFFERENT inits — <c>models[].cycle</c> names each one's.
/// </summary>
public sealed class ModelRunInfo
{
    /// <summary>
    /// The reference init: the NEWEST contributing model's cycle. Every <c>lead_time_hours</c> in the response counts
    /// from it; a model whose cycle is older contributes its own lead <c>lead_time_hours + (initialized_at −
    /// models[].cycle)</c>. For <c>?run=</c> (a pinned historical cycle) every model is at this init.
    /// </summary>
    [JsonPropertyName("initialized_at")]
    public DateTimeOffset InitializedAt { get; init; }

    /// <summary>
    /// The total number of ensemble members.
    /// </summary>
    [JsonPropertyName("total_members")]
    public int TotalMembers { get; init; }

    /// <summary>
    /// One entry per contributing model, in the order <c>aifs</c>, <c>aigefs</c>, <c>gefs</c>, <c>ifs_ens</c>,
    /// <c>rrfs</c> (a model added later follows, by id). It replaces <c>aifs_members</c>..<c>rrfs_members</c>, the
    /// <c>source_models</c> CSV and the <c>model_cycles</c> map: a new model is a new element, never a new key. A
    /// model that contributed nothing is absent.
    /// </summary>
    [JsonPropertyName("models")]
    public IReadOnlyList<ModelEntry> Models { get; init; } = [];
}
