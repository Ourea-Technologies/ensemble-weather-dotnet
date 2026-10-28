// SPDX-License-Identifier: BSD-2-Clause

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace EnsembleWeather;

/// <summary>
/// The resolved page template as the public API serves it — identity + scope + spec; editorial audit fields stay
/// internal.
/// </summary>
public sealed class CatalogTemplateSummary
{
    /// <summary>
    /// The identifier.
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; init; } = string.Empty;

    /// <summary>
    /// The location type the template applies to, snake_case (<c>ski_area</c>); <see langword="null"/> = wildcard scope.
    /// </summary>
    [JsonPropertyName("location_type")]
    public string? LocationType { get; init; }

    /// <summary>
    /// The page-template variant, snake_case (<c>mountain</c>); <see langword="null"/> = wildcard scope.
    /// </summary>
    [JsonPropertyName("template_variant")]
    public string? TemplateVariant { get; init; }

    /// <summary>
    /// The template sections.
    /// </summary>
    [JsonPropertyName("spec")]
    public IReadOnlyList<CatalogTemplateSection> Sections { get; init; } = [];
}
