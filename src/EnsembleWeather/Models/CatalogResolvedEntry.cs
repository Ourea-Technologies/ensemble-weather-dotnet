// SPDX-License-Identifier: BSD-2-Clause

using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EnsembleWeather;

/// <summary>
/// One published content entry as the public catalog detail serves it: entry fields + the attachment's slot/order
/// + which locale the read path actually resolved. Draft/audit internals are deliberately absent.
/// </summary>
public sealed class CatalogResolvedEntry
{
    /// <summary>
    /// The identifier.
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; init; } = string.Empty;

    /// <summary>
    /// The content type.
    /// </summary>
    [JsonPropertyName("content_type_key")]
    public string ContentTypeKey { get; init; } = string.Empty;

    /// <summary>
    /// The title.
    /// </summary>
    [JsonPropertyName("title")]
    public string Title { get; init; } = string.Empty;

    /// <summary>
    /// Shaped by the content type's <c>field_schema</c>.
    /// </summary>
    [JsonPropertyName("body")]
    public JsonElement? Body { get; init; }

    /// <summary>
    /// The template <c>content_slot</c> key this entry fills.
    /// </summary>
    [JsonPropertyName("slot")]
    public string Slot { get; init; } = string.Empty;

    /// <summary>
    /// The display order.
    /// </summary>
    [JsonPropertyName("display_order")]
    public int DisplayOrder { get; init; }

    /// <summary>
    /// The requested locale if ANY field of this entry carried a translation, else the entry's base locale. Partial
    /// coverage overlays per field — that IS the fallback model.
    /// </summary>
    [JsonPropertyName("resolved_locale")]
    public string ResolvedLocale { get; init; } = string.Empty;

    /// <summary>
    /// When the entry was last updated.
    /// </summary>
    [JsonPropertyName("updated_at")]
    public DateTimeOffset UpdatedAt { get; init; }
}
