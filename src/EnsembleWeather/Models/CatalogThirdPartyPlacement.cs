// SPDX-License-Identifier: BSD-2-Clause

using System.Text.Json;
using System.Text.Json.Serialization;

namespace EnsembleWeather;

/// <summary>
/// A resolved third-party placement (network config served verbatim).
/// </summary>
public sealed class CatalogThirdPartyPlacement
{
    /// <summary>
    /// The ad unit id.
    /// </summary>
    [JsonPropertyName("unit_id")]
    public string UnitId { get; init; } = string.Empty;

    /// <summary>
    /// The provider id.
    /// </summary>
    [JsonPropertyName("provider")]
    public string Provider { get; init; } = string.Empty;

    /// <summary>
    /// Provider configuration.
    /// </summary>
    [JsonPropertyName("config")]
    public JsonElement? Config { get; init; }
}
