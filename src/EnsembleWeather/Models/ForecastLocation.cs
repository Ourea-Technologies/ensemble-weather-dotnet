// SPDX-License-Identifier: BSD-2-Clause

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace EnsembleWeather;

/// <summary>
/// The forecast's <c>location</c> -- the one <c>LocationRef</c> (every field where it was) plus what a forecast
/// client labels and picks with, so the forecast body alone is enough to draw a plume or a meteogram at a named
/// elevation (no <c>/nearby</c> call, no client-side unit conversion). Additive.
/// </summary>
public sealed class ForecastLocation : LocationReference
{
    /// <summary>
    /// <c>elevation_m</c> in the request's length units (m for <c>metric</c>/<c>kelvin</c>, ft for
    /// <c>imperial</c>), to 0.1 -- the elevation <c>forecast.timeseries</c> (and its members) is served at. Label
    /// the primary's series and draw the snow-level reference line with it. <c>elevation_m</c> stays metres. (Cap'n
    /// Proto: <c>LocationInfo.elevation</c>.)
    /// </summary>
    [JsonPropertyName("elevation")]
    public double Elevation { get; init; }

    /// <summary>
    /// The location's named elevations -- exactly the list its <c>/nearby</c> and list rows serve
    /// (<c>LocationSummary.elevations</c>): the primary first and its ONE <c>default: true</c>, then each other
    /// elevation (ski-area base / mid / summit), in the request's length units. Name one in <c>?elevations=</c> to
    /// get its forecast (and, with <c>members=true</c>, its plume) in <c>elevation_forecasts[]</c>; label the
    /// primary's series (<c>forecast</c>) with the <c>default: true</c> entry's name. Listed whether or not
    /// <c>elevations</c> was asked for. Omitted for a location with one elevation and for a coordinate forecast.
    /// (Cap'n Proto: <c>LocationInfo.elevations</c>, empty there.)
    /// </summary>
    [JsonPropertyName("elevations")]
    public IReadOnlyList<ElevationSummary> Elevations { get => field ?? []; init; } = [];
}
