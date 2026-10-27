// SPDX-License-Identifier: BSD-2-Clause

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace EnsembleWeather;

/// <summary>
/// Published storm report cards, most recent onset first.
/// </summary>
public sealed class StormReportsResponse : ApiResult
{
    /// <summary>
    /// The storm report cards, most recent onset first.
    /// </summary>
    [JsonPropertyName("reports")]
    public IReadOnlyList<StormReport> Reports { get; init; } = [];

    /// <summary>
    /// The number of items in this response.
    /// </summary>
    [JsonPropertyName("count")]
    public int Count { get; init; }
}
