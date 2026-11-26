// SPDX-License-Identifier: BSD-2-Clause

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace EnsembleWeather;

/// <summary>
/// A GeoJSON <c>FeatureCollection</c> (RFC 7946) of the avalanche centers' forecast zones, each with its published
/// rating, and this response's <c>attribution</c> as a foreign member. GeoJSON: every key is the wire's, unchanged
/// (BEHAVIOR.md 3.0).
/// </summary>
[SuppressMessage("Naming", "CA1711:Identifiers should not have incorrect suffix", Justification = "The spec's schema name: a GeoJSON FeatureCollection.")]
public sealed class AvalancheZoneCollection : ApiResult
{
    /// <summary>
    /// One of <c>FeatureCollection</c>.
    /// </summary>
    [JsonPropertyName("type")]
    public string Type { get; init; } = string.Empty;

    /// <summary>
    /// One Feature per zone, the less specific first: where zones overlap, the one drawn last is the one a point
    /// there is served.
    /// </summary>
    [JsonPropertyName("features")]
    public IReadOnlyList<AvalancheZoneFeature> Features { get => field ?? []; init; } = [];

    /// <summary>
    /// The data sources THIS response drew on, and what their licences ask us to say. <c>sources</c> names exactly
    /// the contributors: the blend's models that moved a served number (a model read but pooled at zero weight is
    /// absent), the climatology behind the daily normal (<c>era5</c>, <c>snotel</c>), PRISM where a
    /// precipitation-elevation carry was applied, the DEM for a coordinate forecast whose elevation came from it
    /// (<c>usgs_3dep</c>), and the observation networks of the nowcast and the nearest sensors. <c>credit</c> is
    /// one line per provider (ECMWF-derived output says it was modified, as its CC BY 4.0 terms require); carry it
    /// with any redistributed output. The full notices are at <c>url</c>.
    /// </summary>
    [JsonPropertyName("attribution")]
    public ResponseAttribution Attribution { get; init; } = null!;
}
