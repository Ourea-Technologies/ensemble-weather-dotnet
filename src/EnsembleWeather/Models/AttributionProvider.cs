// SPDX-License-Identifier: BSD-2-Clause

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace EnsembleWeather;

/// <summary>
/// A data provider, its licence and the statements its terms require.
/// </summary>
public sealed class AttributionProvider
{
    /// <summary>
    /// The identifier.
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; init; } = string.Empty;

    /// <summary>
    /// The name.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; init; } = string.Empty;

    /// <summary>
    /// The credit line a response carries for this provider.
    /// </summary>
    [JsonPropertyName("credit")]
    public string Credit { get; init; } = string.Empty;

    /// <summary>
    /// SPDX id where one exists (<c>CC-BY-4.0</c>, <c>CC-BY-3.0</c>, <c>ODbL-1.0</c>); otherwise <c>public-domain-us-
    /// gov</c>, <c>public-domain</c>, <c>none-stated</c>, or the name of the provider's own terms.
    /// </summary>
    [JsonPropertyName("licence")]
    public string Licence { get; init; } = string.Empty;

    /// <summary>
    /// The licence URL.
    /// </summary>
    [JsonPropertyName("licence_url")]
    public string LicenceUrl { get; init; } = string.Empty;

    /// <summary>
    /// The page the wording was taken from.
    /// </summary>
    [JsonPropertyName("terms_url")]
    public string TermsUrl { get; init; } = string.Empty;

    /// <summary>
    /// Ourea serves this provider's data modified and its terms ask that this be said.
    /// </summary>
    [JsonPropertyName("modified")]
    public bool Modified { get; init; }

    /// <summary>
    /// Statements the terms require shown in full, beyond <c>credit</c>.
    /// </summary>
    [JsonPropertyName("notices")]
    public IReadOnlyList<string> Notices { get; init; } = [];

    /// <summary>
    /// The data sources.
    /// </summary>
    [JsonPropertyName("sources")]
    public IReadOnlyList<AttributionSource> Sources { get; init; } = [];
}
