// SPDX-License-Identifier: BSD-2-Clause

using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace EnsembleWeather;

/// <summary>
/// What a forecast request serves now; <c>null</c> when no cycle is servable.
/// </summary>
public sealed class ServedRun
{
    /// <summary>
    /// The reference init: the newest contributing cycle, a forecast's <c>model_run.initialized_at</c>.
    /// </summary>
    [JsonPropertyName("initialized_at")]
    public DateTimeOffset InitializedAt { get; init; }

    /// <summary>
    /// Each contributing model and its own cycle (<c>{id, cycle}</c>), as <c>model_run.models</c>.
    /// </summary>
    [JsonPropertyName("models")]
    public IReadOnlyList<ModelEntry> Models { get; init; } = [];
}
