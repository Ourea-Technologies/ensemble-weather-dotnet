// SPDX-License-Identifier: BSD-2-Clause

using System;
using System.Text.Json.Serialization;

namespace EnsembleWeather;

/// <summary>
/// The newest snowpack run among the spots served. Absent when <c>spots</c> is empty.
/// </summary>
public sealed class SnowpackRun
{
    /// <summary>
    /// The blend cycle the snowpack model was driven with.
    /// </summary>
    [JsonPropertyName("initialized_at")]
    public DateTimeOffset InitializedAt { get; init; }

    /// <summary>
    /// When the server made this.
    /// </summary>
    [JsonPropertyName("generated_at")]
    public DateTimeOffset GeneratedAt { get; init; }

    /// <summary>
    /// The snowpack model that made the run.
    /// </summary>
    [JsonPropertyName("model")]
    public SnowpackModel Model { get; init; } = null!;
}
