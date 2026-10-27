// SPDX-License-Identifier: BSD-2-Clause

using System;
using System.Text.Json.Serialization;

namespace EnsembleWeather;

/// <summary>
/// The surface station whose reading anchored temperature, dewpoint and wind at this hour. Omitted when none did.
/// </summary>
public sealed class ObservingStation
{
    /// <summary>
    /// The name.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; init; } = string.Empty;

    /// <summary>
    /// When the observation was taken.
    /// </summary>
    [JsonPropertyName("observation_time")]
    public DateTimeOffset ObservationTime { get; init; }
}
