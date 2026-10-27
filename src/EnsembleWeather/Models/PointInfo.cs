// SPDX-License-Identifier: BSD-2-Clause

using System.Text.Json.Serialization;

namespace EnsembleWeather;

/// <summary>
/// Where a coordinate forecast (<c>GET /api/v1/forecast</c>) was computed. Present only on that endpoint.
/// </summary>
public sealed class PointInfo
{
    /// <summary>
    /// Latitude as sent.
    /// </summary>
    [JsonPropertyName("requested_lat")]
    public double RequestedLatitude { get; init; }

    /// <summary>
    /// Longitude as sent.
    /// </summary>
    [JsonPropertyName("requested_lon")]
    public double RequestedLongitude { get; init; }

    /// <summary>
    /// The quantized latitude the forecast was computed at.
    /// </summary>
    [JsonPropertyName("lat")]
    public double Latitude { get; init; }

    /// <summary>
    /// The quantized longitude the forecast was computed at.
    /// </summary>
    [JsonPropertyName("lon")]
    public double Longitude { get; init; }

    /// <summary>
    /// Quantization step in degrees (0.01).
    /// </summary>
    [JsonPropertyName("resolution_deg")]
    public double ResolutionDeg { get; init; }

    /// <summary>
    /// Elevation the forecast was computed at, in the response's length unit (m; ft for <c>units=imperial</c>), like
    /// <c>location.elevation</c>.
    /// </summary>
    [JsonPropertyName("elevation")]
    public double Elevation { get; init; }

    /// <summary>
    /// <c>request</c> = the caller's <c>elevation_m</c> (it always wins); <c>dem_3dep_10m</c> = no <c>elevation_m</c>
    /// was sent, and this is the USGS 3DEP 1/3 arc-second (~10 m) DEM, bilinear at the quantized point.
    /// </summary>
    [JsonPropertyName("elevation_source")]
    public ElevationSource ElevationSource { get; init; }

    /// <summary>
    /// USPS code of the state the point is in (sets the daily summaries' timezone).
    /// </summary>
    [JsonPropertyName("state")]
    public string State { get; init; } = string.Empty;
}
