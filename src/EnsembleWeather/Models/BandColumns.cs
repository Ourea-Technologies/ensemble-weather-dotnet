// SPDX-License-Identifier: BSD-2-Clause

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace EnsembleWeather;

/// <summary>
/// One band's statistics (Hyndman-Fan type 7 over the served member values), each an array aligned to
/// <c>PlumeBands.valid_time</c>, <c>null</c> where the band is absent at that hour. <c>P50</c>, the members'
/// median, is served to Plus and Pro (<c>plume_detail</c>) and is ABSENT, the key itself, for Free and anonymous
/// callers (<c>members_view: pool</c>); every other column is served to every caller.
/// </summary>
public sealed class BandColumns
{
    /// <summary>
    /// Numbers aligned to the enclosing block's <c>valid_time</c>; <c>null</c> = no value there.
    /// </summary>
    [JsonPropertyName("mean")]
    public IReadOnlyList<double?> Mean { get; init; } = [];

    /// <summary>
    /// Numbers aligned to the enclosing block's <c>valid_time</c>; <c>null</c> = no value there.
    /// </summary>
    [JsonPropertyName("min")]
    public IReadOnlyList<double?> Min { get; init; } = [];

    /// <summary>
    /// Numbers aligned to the enclosing block's <c>valid_time</c>; <c>null</c> = no value there.
    /// </summary>
    [JsonPropertyName("p10")]
    public IReadOnlyList<double?> P10 { get; init; } = [];

    /// <summary>
    /// Numbers aligned to the enclosing block's <c>valid_time</c>; <c>null</c> = no value there.
    /// </summary>
    [JsonPropertyName("p25")]
    public IReadOnlyList<double?> P25 { get; init; } = [];

    /// <summary>
    /// Numbers aligned to the enclosing block's <c>valid_time</c>; <c>null</c> = no value there.
    /// </summary>
    [JsonPropertyName("p50")]
    public IReadOnlyList<double?> P50 { get; init; } = [];

    /// <summary>
    /// Numbers aligned to the enclosing block's <c>valid_time</c>; <c>null</c> = no value there.
    /// </summary>
    [JsonPropertyName("p75")]
    public IReadOnlyList<double?> P75 { get; init; } = [];

    /// <summary>
    /// Numbers aligned to the enclosing block's <c>valid_time</c>; <c>null</c> = no value there.
    /// </summary>
    [JsonPropertyName("p90")]
    public IReadOnlyList<double?> P90 { get; init; } = [];

    /// <summary>
    /// Numbers aligned to the enclosing block's <c>valid_time</c>; <c>null</c> = no value there.
    /// </summary>
    [JsonPropertyName("max")]
    public IReadOnlyList<double?> Max { get; init; } = [];
}
