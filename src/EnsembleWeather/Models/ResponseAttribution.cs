// SPDX-License-Identifier: BSD-2-Clause

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace EnsembleWeather;

/// <summary>
/// The data sources THIS response drew on, and what their licences ask us to say. <c>sources</c> names exactly
/// the contributors: the blend's models that moved a served number (a model read but pooled at zero weight is
/// absent), the climatology behind the daily normal (<c>era5</c>, <c>snotel</c>), PRISM where a precipitation-
/// elevation carry was applied, the DEM for a coordinate forecast whose elevation came from it
/// (<c>usgs_3dep</c>), and the observation networks of the nowcast and the nearest sensors. <c>credit</c> is one
/// line per provider (ECMWF-derived output says it was modified, as its CC BY 4.0 terms require); carry it with
/// any redistributed output. The full notices are at <c>url</c>.
/// </summary>
public sealed class ResponseAttribution
{
    /// <summary>
    /// Source ids from <c>GET /api/v1/attribution</c>, in registry order.
    /// </summary>
    [JsonPropertyName("sources")]
    public IReadOnlyList<string> Sources { get; init; } = [];

    /// <summary>
    /// The credit line to carry with any redistributed output.
    /// </summary>
    [JsonPropertyName("credit")]
    public string Credit { get; init; } = string.Empty;

    /// <summary>
    /// Path of the full registry.
    /// </summary>
    [JsonPropertyName("url")]
    public string Url { get; init; } = string.Empty;
}
