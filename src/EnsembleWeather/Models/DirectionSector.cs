// SPDX-License-Identifier: BSD-2-Clause

using System.Text.Json.Serialization;

namespace EnsembleWeather;

/// <summary>
/// A clockwise arc on the compass, degrees. <c>to</c> may be numerically smaller than <c>from</c> because the arc
/// crosses north; the width is <c>(to - from) mod 360</c>, never <c>to - from</c>, and a bearing <c>b</c> lies
/// inside it when <c>(b - from) mod 360 &lt;= (to - from) mod 360</c>.
/// </summary>
public sealed class DirectionSector
{
    /// <summary>
    /// The window's first instant.
    /// </summary>
    [JsonPropertyName("from")]
    public double From { get; init; }

    /// <summary>
    /// The window's end (exclusive).
    /// </summary>
    [JsonPropertyName("to")]
    public double To { get; init; }
}
