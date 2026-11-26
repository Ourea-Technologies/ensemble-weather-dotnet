// SPDX-License-Identifier: BSD-2-Clause

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace EnsembleWeather;

/// <summary>
/// What the snowpack codes mean: surface and layer-form code -> text. The keys are data, kept exactly as sent.
/// </summary>
public sealed class SnowpackDefinitions
{
    /// <summary>
    /// Each surface class and what it means.
    /// </summary>
    [JsonPropertyName("surface")]
    public IReadOnlyDictionary<string, string> Surface { get; init; } = null!;

    /// <summary>
    /// Each layer form and what it is.
    /// </summary>
    [JsonPropertyName("layer_form")]
    public IReadOnlyDictionary<string, string> LayerForm { get; init; } = null!;
}
