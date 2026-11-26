// SPDX-License-Identifier: BSD-2-Clause

using System.Text.Json.Serialization;

namespace EnsembleWeather;

/// <summary>
/// A mountain pass chance: a probability with its threshold and the members that voted, never a bare number.
/// </summary>
public sealed class PassChance
{
    /// <summary>
    /// 0-1 (0.001); null when it cannot be answered (<c>null_reason</c>).
    /// </summary>
    [JsonPropertyName("probability")]
    public double? Probability { get; init; }

    /// <summary>
    /// The event's threshold, in the response's units.
    /// </summary>
    [JsonPropertyName("threshold")]
    public double Threshold { get; init; }

    /// <summary>
    /// For <c>p_wet_below_freezing</c>, the precipitation that makes an hour wet, in the response's units.
    /// </summary>
    [JsonPropertyName("precipitation_threshold")]
    public double? PrecipitationThreshold { get; init; }

    /// <summary>
    /// The members that voted (0 for <c>hourly_gust_distribution</c>).
    /// </summary>
    [JsonPropertyName("members")]
    public int Members { get; init; }

    /// <summary>
    /// One of <c>members</c>, <c>hourly_gust_distribution</c>.
    /// </summary>
    [JsonPropertyName("method")]
    public string Method { get; init; } = string.Empty;

    /// <summary>
    /// One of <c>window_not_covered</c>, <c>too_few_members</c>, <c>variable_unavailable</c>.
    /// </summary>
    [JsonPropertyName("null_reason")]
    public string? NullReason { get; init; }
}
