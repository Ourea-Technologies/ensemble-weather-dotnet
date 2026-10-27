// SPDX-License-Identifier: BSD-2-Clause

using System;
using System.Text.Json.Serialization;

namespace EnsembleWeather;

/// <summary>
/// A run a pinned <c>run</c> can name.
/// </summary>
public sealed class AvailableRun
{
    /// <summary>
    /// The run's init time.
    /// </summary>
    [JsonPropertyName("initialized_at")]
    public DateTimeOffset InitializedAt { get; init; }
}
