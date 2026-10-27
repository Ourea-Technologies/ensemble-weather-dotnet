// SPDX-License-Identifier: BSD-2-Clause

using System.Text.Json.Serialization;

namespace EnsembleWeather;

/// <summary>
/// Surface-based cold pool or elevated inversion detected from the model's vertical temperature profile. Clients
/// can use this to badge hours where "warmer aloft than at the surface" conditions are in effect.
/// </summary>
public sealed class InversionInfo
{
    /// <summary>
    /// <c>surface_shallow</c>: shallow (&lt;300 m) surface-based inversion, common overnight in calm/clear
    /// conditions. <c>cold_pool</c>: deep (≥300 m) surface-based cold pool, can persist for days under a ridge.
    /// <c>elevated</c>: warm layer aloft above a normal surface layer (subsidence, capped boundary layer).
    /// </summary>
    [JsonPropertyName("kind")]
    public InversionKind Kind { get; init; }

    /// <summary>
    /// Depth of the inversion layer in the caller's length units (meters for metric, feet for imperial). For
    /// <c>elevated</c>, this is the thickness of the warm layer (top − base).
    /// </summary>
    [JsonPropertyName("depth")]
    public double Depth { get; init; }

    /// <summary>
    /// Temperature difference driving the inversion in the caller's temperature units. Always positive: the warm part
    /// minus the cold part.
    /// </summary>
    [JsonPropertyName("strength")]
    public double Strength { get; init; }
}
