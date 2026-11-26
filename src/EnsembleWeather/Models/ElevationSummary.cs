// SPDX-License-Identifier: BSD-2-Clause

using System.Text.Json.Serialization;

namespace EnsembleWeather;

/// <summary>
/// One of a location's elevation points, as <c>/nearby</c>, the list rows and the forecast's
/// <c>location.elevations</c> serve it.
/// </summary>
public sealed class ElevationSummary
{
    /// <summary>
    /// The elevation's id: the name to pass in <c>?elevations=</c> (case, <c>-</c>, <c>_</c> and spaces ignored).
    /// Stable; not a display name (<c>base</c>, <c>low_cascade_village_lift</c>).
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; init; } = string.Empty;

    /// <summary>
    /// What the place is called, for display ("K-1 Lodge", "FourRunner Quad top"). Absent where the catalog has no
    /// name for the point: show <c>name</c> as before. Never accepted by <c>?elevations=</c>.
    /// </summary>
    [JsonPropertyName("display_name")]
    public string? DisplayName { get; init; }

    /// <summary>
    /// (JSON <c>elevation</c>) In the caller's length units (m, ft), to 0.1.
    /// </summary>
    [JsonPropertyName("elevation")]
    public double Height { get; init; }

    /// <summary>
    /// Present (true) on the primary only.
    /// </summary>
    [JsonPropertyName("default")]
    public bool? IsDefault { get; init; }
}
