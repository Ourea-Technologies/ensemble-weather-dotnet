// SPDX-License-Identifier: BSD-2-Clause

using System;
using System.Text.Json.Serialization;

namespace EnsembleWeather;

/// <summary>
/// The next 24 h of snow at the pass: exactly the default forecast's own <c>snow_outlook.next_24h</c> and
/// <c>snow_outlook.quantiles.next_24h</c> (unpinned, the nowcast on, the same cycle), so the pass page, the passes
/// board and search state one number.
/// </summary>
public sealed class PassSnow24h
{
    /// <summary>
    /// <c>snow_outlook.next_24h</c> (mm, in); null where the forecast does not reach 24 h.
    /// </summary>
    [JsonPropertyName("total")]
    public double? Total { get; init; }

    /// <summary>
    /// <c>snow_outlook.quantiles.next_24h.p10</c>; null without members covering the window.
    /// </summary>
    [JsonPropertyName("p10")]
    public double? P10 { get; init; }

    /// <summary>
    /// <c>snow_outlook.quantiles.next_24h.p90</c>.
    /// </summary>
    [JsonPropertyName("p90")]
    public double? P90 { get; init; }

    /// <summary>
    /// The start of the first snow spell (<c>snow_windows[0].start</c>) when it begins inside the 24 h; null
    /// otherwise.
    /// </summary>
    [JsonPropertyName("starts_at")]
    public DateTimeOffset? StartsAt { get; init; }
}
