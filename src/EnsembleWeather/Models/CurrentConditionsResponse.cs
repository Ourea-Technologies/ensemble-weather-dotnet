// SPDX-License-Identifier: BSD-2-Clause

using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace EnsembleWeather;

/// <summary>
/// Current sensor observations, active alerts and the nowcast overlay for a location or point.
/// </summary>
public sealed class CurrentConditionsResponse : ApiResult
{
    /// <summary>
    /// The one location reference (it was the display name beside flat <c>lat</c>, <c>lon</c>, <c>elevation_m</c> and
    /// <c>timezone</c>).
    /// </summary>
    [JsonPropertyName("location")]
    public LocationReference? Location { get; init; }

    /// <summary>
    /// Active NWS watches/warnings/advisories for this location (zone- and polygon-matched). Rides the fresh /current
    /// lane deliberately — the cacheable forecast body never carries alerts.
    /// </summary>
    [JsonPropertyName("alerts")]
    public IReadOnlyList<ActiveAlert> Alerts { get; init; } = [];

    /// <summary>
    /// Timestamp of the alerts snapshot backing this response (refreshed ~60s; staleness beyond a few minutes means
    /// the alerts pipeline is degraded).
    /// </summary>
    [JsonPropertyName("alerts_as_of")]
    public DateTimeOffset AlertsAsOf { get; init; }

    /// <summary>
    /// The number of sensors reporting.
    /// </summary>
    [JsonPropertyName("sensor_count")]
    public int SensorCount { get; init; }

    /// <summary>
    /// Each sensor's latest reading set (in the request's <c>units</c>, under plain names -- it was the observation
    /// table's column names and units, <c>temperature_c</c>, <c>wind_speed_ms</c>). A reading is omitted where the
    /// sensor does not take it, and where verification does not take it as truth at that station (the one rule that
    /// also governs scoring and training): HADS river-gauge temperature and dewpoint and CWOP backyard-station
    /// dewpoint and wind. A sensor left with no reading at all is not listed, and is neither counted in
    /// <c>sensor_count</c> nor credited in <c>attribution</c>.
    /// </summary>
    [JsonPropertyName("sensors")]
    public IReadOnlyList<SensorObservation> Sensors { get; init; } = [];

    /// <summary>
    /// "Right now" flattened: the <c>nowcast_hours</c> hour nearest the request time, read field by field (see the
    /// schema). Null when no cycle is servable or no band hour lies within an hour of now.
    /// </summary>
    [JsonPropertyName("current")]
    public CurrentConditions? Current { get; init; }

    /// <summary>
    /// When this overlay was assembled (the request time); the <c>nowcast_hours</c> window is relative to it.
    /// </summary>
    [JsonPropertyName("as_of")]
    public DateTimeOffset AsOf { get; init; }

    /// <summary>
    /// The reference init of the cycle the band was read from (null when no cycle is servable). A change means the
    /// cached base forecast is from an older cycle.
    /// </summary>
    [JsonPropertyName("model_run_initialized_at")]
    public DateTimeOffset? ModelRunInitializedAt { get; init; }

    /// <summary>
    /// The obs-blended forecast hours with valid times in [as_of − 1 h, as_of + 10 h], shaped like the forecast's
    /// <c>timeseries</c> points; null when no cycle is servable. Present only with <c>?include=nowcast_hours</c> (it
    /// was 42 KB of a 44 KB body on a 30 s TTL); <c>current</c> is the flat "right now".
    /// </summary>
    [JsonPropertyName("nowcast_hours")]
    public IReadOnlyList<TimeSeriesPoint>? NowcastHours { get; init; }

    /// <summary>
    /// The unit system every value in this body is in.
    /// </summary>
    [JsonPropertyName("units")]
    public UnitSystem Units { get; init; }

    /// <summary>
    /// The data sources this response drew on and the credit they require.
    /// </summary>
    [JsonPropertyName("attribution")]
    public ResponseAttribution? Attribution { get; init; }

    /// <summary>
    /// The live nowcast sources <c>nowcast_hours</c> and <c>current</c> were built without because their read did not
    /// answer in time (or failed). A request never waits on a slow observation read past a short deadline; it serves
    /// without that source and names it here. Omitted when every source was read.
    /// </summary>
    [JsonPropertyName("nowcast_unavailable")]
    public IReadOnlyList<NowcastSource> NowcastUnavailable { get; init; } = [];
}
