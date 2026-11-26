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
    /// (JSON <c>station_id</c>) The station's id in its network, as <c>nearest_sensors[].station_id</c>.
    /// </summary>
    [JsonPropertyName("station_id")]
    public string Id { get; init; } = string.Empty;

    /// <summary>
    /// The station's registry name, else its id.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; init; } = string.Empty;

    /// <summary>
    /// When the observation was taken.
    /// </summary>
    [JsonPropertyName("observation_time")]
    public DateTimeOffset ObservationTime { get; init; }

    /// <summary>
    /// <c>True</c> when the station's reading was calm (0 wind speed; a METAR's <c>00000KT</c>). A calm reading has
    /// no bearing, so it does not turn the served direction: <c>wind_direction</c> is then the forecast's, not a
    /// reported direction, and should not be shown as the station's. <c>false</c> for a moving wind and for a
    /// station that reads no wind. Additive; Cap'n Proto <c>CurrentStation.calm</c>.
    /// </summary>
    [JsonPropertyName("calm")]
    public bool Calm { get; init; }
}
