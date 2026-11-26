// SPDX-License-Identifier: BSD-2-Clause

using System;
using System.Text.Json.Serialization;

namespace EnsembleWeather;

/// <summary>
/// A layer of the modelled snowpack.
/// </summary>
public sealed class SnowpackLayer
{
    /// <summary>
    /// A key of <c>definitions.layer_form</c>: today <c>faceted_grains</c>, <c>depth_hoar</c>,
    /// <c>buried_surface_hoar</c>, <c>melt_freeze_crust</c>, <c>ice_layer</c> or <c>rounding_facets</c>. Tolerate
    /// values you do not know.
    /// </summary>
    [JsonPropertyName("form")]
    public string Form { get; init; } = string.Empty;

    /// <summary>
    /// cm or in.
    /// </summary>
    [JsonPropertyName("thickness")]
    public double Thickness { get; init; }

    /// <summary>
    /// From the surface down to the layer's top (cm or in).
    /// </summary>
    [JsonPropertyName("depth")]
    public double Depth { get; init; }

    /// <summary>
    /// When the layer formed.
    /// </summary>
    [JsonPropertyName("formed")]
    [JsonConverter(typeof(Serialization.CalendarDateConverter))]
    public DateTime Formed { get; init; }
}
