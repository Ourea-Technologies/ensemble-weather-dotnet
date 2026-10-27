// SPDX-License-Identifier: BSD-2-Clause

using System.Text.Json.Serialization;

namespace EnsembleWeather;

/// <summary>
/// A resolved house placement (campaign + creative content).
/// </summary>
public sealed class CatalogHousePlacement
{
    /// <summary>
    /// The campaign id.
    /// </summary>
    [JsonPropertyName("campaign_id")]
    public string CampaignId { get; init; } = string.Empty;

    /// <summary>
    /// The creative id.
    /// </summary>
    [JsonPropertyName("creative_id")]
    public string CreativeId { get; init; } = string.Empty;

    /// <summary>
    /// Published content entries (<c>include=content</c>).
    /// </summary>
    [JsonPropertyName("content")]
    public CatalogAdCreative? Content { get; init; }
}
