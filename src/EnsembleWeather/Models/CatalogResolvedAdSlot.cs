// SPDX-License-Identifier: BSD-2-Clause

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace EnsembleWeather;

/// <summary>
/// One filled ad slot as the public API serves it. Exactly one of <c>house</c> / <c>third_party</c> is present,
/// discriminated by <c>kind</c>. Report impressions/clicks for it via <c>POST /api/v1/events/ad</c>.
/// </summary>
public sealed class CatalogResolvedAdSlot
{
    /// <summary>
    /// The slot key.
    /// </summary>
    [JsonPropertyName("slot_key")]
    public string SlotKey { get; init; } = string.Empty;

    /// <summary>
    /// The kind.
    /// </summary>
    [JsonPropertyName("kind")]
    public string Kind { get; init; } = string.Empty;

    /// <summary>
    /// Present iff <c>kind = house</c>.
    /// </summary>
    [JsonPropertyName("house")]
    public CatalogHousePlacement? House { get; init; }

    /// <summary>
    /// Present iff <c>kind = third_party</c>.
    /// </summary>
    [JsonPropertyName("third_party")]
    public CatalogThirdPartyPlacement? ThirdParty { get; init; }

    /// <summary>
    /// The slot's client rendering hints (<c>default_sizes</c>).
    /// </summary>
    [JsonPropertyName("sizes")]
    public IReadOnlyList<IReadOnlyList<int>> Sizes { get; init; } = [];
}
