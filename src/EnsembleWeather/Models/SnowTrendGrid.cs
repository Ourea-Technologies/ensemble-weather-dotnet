// SPDX-License-Identifier: BSD-2-Clause

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace EnsembleWeather;

/// <summary>
/// One source's grid: a row per recent run, a column per day.
/// </summary>
public sealed class SnowTrendGrid
{
    /// <summary>
    /// <c>ourea</c>, or a model id.
    /// </summary>
    [JsonPropertyName("source")]
    public string Source { get; init; } = string.Empty;

    /// <summary>
    /// Oldest first; the last is the newest. Empty when none is recorded yet.
    /// </summary>
    [JsonPropertyName("runs")]
    public IReadOnlyList<SnowTrendRun> Runs { get; init; } = [];
}
