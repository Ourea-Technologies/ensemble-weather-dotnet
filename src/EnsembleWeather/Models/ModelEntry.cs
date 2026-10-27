// SPDX-License-Identifier: BSD-2-Clause

using System;
using System.Text.Json.Serialization;

namespace EnsembleWeather;

/// <summary>
/// (D-8) -- one contributing model.
/// </summary>
public sealed class ModelEntry
{
    /// <summary>
    /// The model id (<c>aifs</c>, <c>aigefs</c>, <c>gefs</c>, <c>ifs_ens</c>, <c>rrfs</c>; treat others as additive).
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; init; } = string.Empty;

    /// <summary>
    /// The model's own cycle init (per-model-latest). Omitted when it was not read.
    /// </summary>
    [JsonPropertyName("cycle")]
    public DateTimeOffset? Cycle { get; init; }

    /// <summary>
    /// How many of its members the forecast carries (in <c>model_run</c>); omitted in <c>/runs</c> <c>served</c>.
    /// </summary>
    [JsonPropertyName("members")]
    public int? Members { get; init; }
}
