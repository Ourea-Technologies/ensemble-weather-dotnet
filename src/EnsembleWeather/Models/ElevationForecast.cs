// SPDX-License-Identifier: BSD-2-Clause

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace EnsembleWeather;

/// <summary>
/// Forecast at one of a location's additional elevations (ski-area base / mid / summit). Each is a full point
/// forecast built by the SAME pipeline as the primary <c>forecast</c> — grid assembly, snowfall derivation,
/// NGR/CSGD correction, nowcast blend, conditions, feels-like, provenance, the daily windows and day totals, the
/// snow outlook — at this elevation's coordinates (<c>lat</c>/<c>lon</c>) and elevation. Every model's 2 m
/// temperature and dewpoint are first carried from the model's own terrain height to the primary's elevation (a
/// diurnal climatological lapse at the local solar hour; dewpoint 2 °C/km), as for every served point; they are
/// then carried from the primary's elevation by a lapse rate (the hour's own profile when it brackets the layer,
/// else climatology; see <c>provenance.lapse_profile</c> / <c>lapse_climatology</c>), and the learned corrections
/// are the primary's. Precipitation and snowfall are carried from the primary's elevation by the local PRISM
/// precipitation–elevation regression (see <c>provenance.lapse_prism</c>). Wind and snow level are the forecast
/// at those coordinates — no elevation multiplier is applied to them. The location's own point is never repeated
/// here.
/// </summary>
public sealed class ElevationForecast
{
    /// <summary>
    /// The name.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; init; } = string.Empty;

    /// <summary>
    /// The elevation, in the request's length unit.
    /// </summary>
    [JsonPropertyName("elevation")]
    public double Elevation { get; init; }

    /// <summary>
    /// Whether this is the default elevation to show in the UI.
    /// </summary>
    [JsonPropertyName("default")]
    public bool? IsDefault { get; init; }

    /// <summary>
    /// Where this elevation was forecast — its own coordinates when the catalog has them, else the location's.
    /// </summary>
    [JsonPropertyName("lat")]
    public double Latitude { get; init; }

    /// <summary>
    /// Longitude, decimal degrees (WGS84, west negative).
    /// </summary>
    [JsonPropertyName("lon")]
    public double Longitude { get; init; }

    /// <summary>
    /// LEGACY compact hourly points, derived from <c>hourly</c> (kept for clients that read only these; new clients
    /// should read <c>hourly</c>). Each also carries the hour's <c>precipitation</c>, <c>condition</c> and
    /// <c>condition_icon</c> -- the same values as the <c>hourly</c> hour at that <c>valid_time</c>, so an hour's
    /// precipitation type (<c>snow_level_delta</c>) can be shown only when something is likely to fall. Omitted with
    /// <c>hourly=false</c>. Carries the same gaps as the primary <c>forecast.timeseries</c> — a refused hour is
    /// absent here too — so key on <c>valid_time</c>, not position.
    /// </summary>
    [JsonPropertyName("timeseries")]
    public IReadOnlyList<ElevationTimeSeriesPoint> Timeseries { get; init; } = [];

    /// <summary>
    /// The full hourly series at this elevation — the same shape and the same code as <c>forecast.timeseries</c>
    /// (never with <c>model_means</c>). With <c>members=true</c> each hour carries THIS elevation's <c>members</c>:
    /// every member lapsed by the same per-hour offset as the headline temperature and dewpoint, its precipitation
    /// carried by the same PRISM ratio (over the hours its <c>precipitation_hours</c> span), and its snowfall derived
    /// by the member snowfall rule at this elevation; in the response's <c>members_view</c> (Free <c>plume</c> / Pro
    /// <c>full</c>). Same gap rule: key on <c>valid_time</c>. Omitted with <c>hourly=false</c>.
    /// </summary>
    [JsonPropertyName("hourly")]
    public IReadOnlyList<TimeSeriesPoint> Hourly { get; init; } = [];

    /// <summary>
    /// Daily summaries at this elevation — the same daily build as <c>forecast.daily</c> (omitted when empty)
    /// </summary>
    [JsonPropertyName("daily")]
    public IReadOnlyList<DailyForecast> Daily { get; init; } = [];

    /// <summary>
    /// Rolling snow totals at this elevation, as <c>forecast.snow_outlook</c> (omitted when there are no dailies)
    /// </summary>
    [JsonPropertyName("snow_outlook")]
    public SnowOutlook? SnowOutlook { get; init; }
}
