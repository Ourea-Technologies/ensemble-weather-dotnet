// SPDX-License-Identifier: BSD-2-Clause

using System.Text.Json.Serialization;

namespace EnsembleWeather;

/// <summary>
/// A bearing's distribution stated on the circle. Weather APIs serve a direction as one number; the statistical
/// convention for a direction DISTRIBUTION is a mean direction with a dispersion beside it and intervals as
/// clockwise arcs. Degrees in both unit systems.
/// </summary>
public sealed class CircularStatistics
{
    /// <summary>
    /// The resultant-vector mean direction, degrees clockwise from north, in [0, 360). Equals
    /// <c>wind_direction.mean</c>.
    /// </summary>
    [JsonPropertyName("mean")]
    public double Mean { get; init; }

    /// <summary>
    /// Standard deviation of the members' signed angular offsets from <c>mean</c>, degrees. Exact for a concentrated
    /// distribution and never a difference of two bearings. Absent on a single-member hour.
    /// </summary>
    [JsonPropertyName("stddev")]
    public double? StandardDeviation { get; init; }

    /// <summary>
    /// The central 50 % of the members — the clockwise arc from the 25th to the 75th circular percentile.
    /// </summary>
    [JsonPropertyName("sector_50")]
    public DirectionSector Sector50 { get; init; } = null!;

    /// <summary>
    /// The central 90 % of the members — the clockwise arc from the 5th to the 95th circular percentile.
    /// </summary>
    [JsonPropertyName("sector_90")]
    public DirectionSector Sector90 { get; init; } = null!;
}
