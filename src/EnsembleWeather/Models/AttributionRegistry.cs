// SPDX-License-Identifier: BSD-2-Clause

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace EnsembleWeather;

/// <summary>
/// Every data source Ourea uses, grouped by provider, with licences and required notices.
/// </summary>
public sealed class AttributionRegistry : ApiResult
{
    /// <summary>
    /// The data providers.
    /// </summary>
    [JsonPropertyName("providers")]
    public IReadOnlyList<AttributionProvider> Providers { get; init; } = [];
}
