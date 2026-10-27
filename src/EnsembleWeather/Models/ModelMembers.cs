// SPDX-License-Identifier: BSD-2-Clause

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace EnsembleWeather;

/// <summary>
/// One model's members at one hour.
/// </summary>
public sealed class ModelMembers
{
    /// <summary>
    /// The identifier.
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; init; } = string.Empty;

    /// <summary>
    /// The number of ensemble members that contributed.
    /// </summary>
    [JsonPropertyName("members")]
    public IReadOnlyList<MemberForecast> Members { get; init; } = [];
}
