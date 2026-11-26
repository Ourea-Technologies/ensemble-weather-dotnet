// SPDX-License-Identifier: BSD-2-Clause

using System;
using System.Text.Json.Serialization;

namespace EnsembleWeather;

/// <summary>
/// One step of a snowpack spot's forecast: the surface, the new snow and the settlement.
/// </summary>
public sealed class SnowpackStep
{
    /// <summary>
    /// The valid times, one per column.
    /// </summary>
    [JsonPropertyName("valid_time")]
    public DateTimeOffset ValidTime { get; init; }

    /// <summary>
    /// A key of <c>definitions.surface</c>: today <c>none</c>, <c>powder</c>, <c>wind_affected</c>, <c>settled</c>,
    /// <c>wet</c>, <c>crust</c> or <c>ice_crust</c>. Tolerate values you do not know.
    /// </summary>
    [JsonPropertyName("surface")]
    public string Surface { get; init; } = string.Empty;

    /// <summary>
    /// New snow of the 24 h to <c>valid_time</c>, as it lies (cm or in).
    /// </summary>
    [JsonPropertyName("new_snow_24h")]
    public double NewSnow24Hours { get; init; }

    /// <summary>
    /// Its density. Absent under a trace of new snow.
    /// </summary>
    [JsonPropertyName("new_snow_density_kg_m3")]
    public double? NewSnowDensityKgM3 { get; init; }

    /// <summary>
    /// How far the snow on the ground 24 h earlier settled (cm or in). Absent when not stated.
    /// </summary>
    [JsonPropertyName("settlement_24h")]
    public double? Settlement24Hours { get; init; }
}
