// SPDX-License-Identifier: BSD-2-Clause

using System.Text.Json.Serialization;

namespace EnsembleWeather;

/// <summary>
/// Columbia Gorge gap flow status at the first served hour, from the forecast mean-sea-level-pressure difference
/// across the Gorge (The Dalles minus Portland, each model differenced on its own, then averaged). Only present
/// for locations within the Gorge corridor and when the forecast carries MSLP at both ends. Omitted for
/// nowcast=false.
/// </summary>
public sealed class GapFlow
{
    /// <summary>
    /// The status.
    /// </summary>
    [JsonPropertyName("status")]
    public GapFlowStatus? Status { get; init; }

    /// <summary>
    /// The pressure difference, hPa.
    /// </summary>
    [JsonPropertyName("pressure_diff_hpa")]
    public double? PressureDifferenceHpa { get; init; }

    /// <summary>
    /// The wind multiplier.
    /// </summary>
    [JsonPropertyName("wind_multiplier")]
    public double? WindMultiplier { get; init; }

    /// <summary>
    /// The flow direction.
    /// </summary>
    [JsonPropertyName("direction")]
    public string? Direction { get; init; }
}
