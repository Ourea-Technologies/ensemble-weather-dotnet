// SPDX-License-Identifier: BSD-2-Clause

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace EnsembleWeather;

/// <summary>
/// The forecast options the server applied, after the tier clamps and bounds. <c>max_hours</c> and
/// <c>max_days</c> are clamped, never refused (Free 240 h / 10 days, Pro 360 h / 15 days; <c>max_hours</c> has a
/// 6 h floor), so the body says what it served. Present on every REST forecast body.
/// </summary>
public sealed class AppliedOptions
{
    /// <summary>
    /// Hours past the first served hour; <c>null</c> with <c>hourly=false</c>.
    /// </summary>
    [JsonPropertyName("max_hours")]
    public int? MaxHours { get; init; }

    /// <summary>
    /// Daily rows served for; <c>null</c> with <c>daily=false</c>.
    /// </summary>
    [JsonPropertyName("max_days")]
    public int? MaxDays { get; init; }

    /// <summary>
    /// The requested options the server changed, in this order. Empty when it changed none.
    /// </summary>
    [JsonPropertyName("clamped")]
    public IReadOnlyList<string> Clamped { get; init; } = [];
}
