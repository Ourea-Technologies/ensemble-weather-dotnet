// SPDX-License-Identifier: BSD-2-Clause

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace EnsembleWeather;

/// <summary>
/// An ensemble forecast: every value is a distribution, in the request's units (<see cref="Units"/>).
/// </summary>
/// <remarks>
/// Hourly values are in <see cref="ForecastData.Timeseries"/> (from the current hour), daily summaries in
/// <see cref="ForecastData.Daily"/>; <see cref="UnitLabels"/> names the unit of each kind of quantity.
/// </remarks>
public sealed class ForecastResponse : ApiResult
{
    /// <summary>
    /// The event behind each served probability, in the request's units.
    /// </summary>
    [JsonPropertyName("thresholds")]
    public ProbabilityThresholds? Thresholds { get; init; }

    /// <summary>
    /// Gets the answers to <see cref="ForecastOptions.Probabilities"/>, in the caller's order (one per term asked;
    /// duplicate terms share one answer). Empty when none were asked.
    /// </summary>
    /// <remarks>
    /// The server answers in canonical (sorted) order; the SDK maps each answer back to the term that asked it. Each
    /// answer carries its reference class: variable, comparison, threshold, window and member count. A
    /// <see langword="null"/> <see cref="ProbabilityResult.Value"/> means the series does not reach the
    /// window's end; it is never 0.
    /// </remarks>
    [JsonPropertyName("probabilities")]
    [JsonInclude]
    public IReadOnlyList<ProbabilityResult> Probabilities { get; internal set; } = [];

    /// <summary>
    /// Gets the answers to <see cref="ForecastOptions.WindowTotals"/>, in the caller's order. Empty when none were
    /// asked. A window the forecast does not cover has <see langword="null"/> <see cref="WindowTotal.Quantiles"/>.
    /// </summary>
    [JsonPropertyName("window_totals")]
    [JsonInclude]
    public IReadOnlyList<WindowTotal> WindowTotals { get; internal set; } = [];

    /// <summary>
    /// The options the server applied after tier clamps.
    /// </summary>
    [JsonPropertyName("applied")]
    public AppliedOptions? Applied { get; init; }

    /// <summary>
    /// The location.
    /// </summary>
    [JsonPropertyName("location")]
    public ForecastLocation Location { get; init; } = null!;

    /// <summary>
    /// The model run this forecast was served from.
    /// </summary>
    [JsonPropertyName("model_run")]
    public ModelRunInfo ModelRun { get; init; } = null!;

    /// <summary>
    /// The unit system every value in this body is in -- the request's <c>units=</c>, <c>metric</c> when it is not
    /// given. Always present, so a cached or shared body says
    /// whether <c>32.0</c> is °F or °C. The Cap'n Proto body carries the same as <c>ForecastResponse.units</c> (set
    /// when <c>hasUnits</c>).
    /// </summary>
    [JsonPropertyName("units")]
    public UnitSystem Units { get; init; }

    /// <summary>Gets the unit of each kind of quantity in this body, for <see cref="Units"/>.</summary>
    [JsonIgnore]
    public UnitLabels UnitLabels => UnitLabels.For(Units);

    /// <summary>
    /// (JSON <c>forecast</c>) The forecast series and daily summaries.
    /// </summary>
    [JsonPropertyName("forecast")]
    public ForecastData Series { get; init; } = null!;

    /// <summary>
    /// (JSON <c>elevation_forecasts</c>) Forecasts at the location's other named elevations (ski areas), when requested.
    /// </summary>
    [JsonPropertyName("elevation_forecasts")]
    public IReadOnlyList<ElevationForecast> Elevations { get => field ?? []; init; } = [];

    /// <summary>
    /// Whether the live nowcast layer is baked in. <c>true</c> = merged legacy shape (uncacheable). <c>false</c> =
    /// pure Layer-A base forecast (edge-cacheable; overlay <c>…/current</c>'s <c>nowcast_hours</c> on top).
    /// </summary>
    [JsonPropertyName("nowcast_applied")]
    public bool NowcastApplied { get; init; }

    /// <summary>
    /// The live nowcast sources this body was built WITHOUT because their read did not answer in time (or failed):
    /// the nearest station's reading (<c>observation</c>), MRMS <c>radar</c>, GOES <c>cloud</c>, or the
    /// <c>nearest_sensors</c> list. A forecast request never waits on a slow observation read past a short deadline
    /// (250 ms for the radar and cloud grids, 1 s for a location's first station read); it serves the forecast
    /// without that source and names it here. Omitted when every source was read, and on <c>nowcast=false</c>.
    /// </summary>
    [JsonPropertyName("nowcast_unavailable")]
    public IReadOnlyList<NowcastSource> NowcastUnavailable { get => field ?? []; init; } = [];

    /// <summary>
    /// Which member view <c>forecast.timeseries[*].members</c> (and <c>elevation_forecasts[*].hourly[*].members</c>)
    /// carries; present exactly when members are. <c>plume</c> (Free) withholds <c>wind_speed</c>,
    /// <c>wind_direction</c> and <c>slr</c> — a <c>null</c> there is withheld by the view, not a member that
    /// published nothing. <c>full</c> (Pro) carries every member variable.
    /// </summary>
    [JsonPropertyName("members_view")]
    public MembersView? MembersView { get; init; }

    /// <summary>
    /// Columbia Gorge gap flow status at the first served hour, from the forecast mean-sea-level-pressure difference
    /// across the Gorge (The Dalles minus Portland, each model differenced on its own, then averaged). Only present
    /// for locations within the Gorge corridor and when the forecast carries MSLP at both ends. Omitted for
    /// nowcast=false.
    /// </summary>
    [JsonPropertyName("gap_flow")]
    public GapFlow? GapFlow { get; init; }

    /// <summary>
    /// Nearby observation sensors ranked by an elevation-weighted distance. <c>effective_distance_km</c> combines
    /// horizontal distance with a 100 m / 1 km elevation penalty — a sensor 200 m below the forecast point
    /// effectively moves 2 km further away in the ranking. Typically top 5 returned. Empty when no sensor reported
    /// within 50 km / 3 hours.
    /// </summary>
    [JsonPropertyName("nearest_sensors")]
    public IReadOnlyList<NearestSensor> NearestSensors { get => field ?? []; init; } = [];

    /// <summary>
    /// Where a coordinate forecast was computed (coordinate forecasts only).
    /// </summary>
    [JsonPropertyName("point")]
    public PointInfo? Point { get; init; }

    /// <summary>
    /// Current conditions read off the served hours.
    /// </summary>
    [JsonPropertyName("current")]
    public CurrentConditions? Current { get; init; }

    /// <summary>
    /// The data sources this response drew on and the credit they require.
    /// </summary>
    [JsonPropertyName("attribution")]
    public ResponseAttribution Attribution { get; init; } = null!;

    /// <summary>
    /// The avalanche center's danger rating for the forecast zone the location's point is in, from avalanche.org's
    /// public map layer. Link-first: the danger level, the zone and center names, and a link to the center's full
    /// forecast, which is the thing to read -- Ourea never serves the forecast's text and never rates danger
    /// itself. Absent when the feature is off on this server or the point is in no avalanche center's zone. Fail
    /// closed: when the zone has not been seen in a successful poll recently, or its product has expired
    /// (<c>valid_until</c> passed), <c>status</c> is <c>unavailable</c> and <c>danger_level</c> is <c>null</c> --
    /// an old rating is never served. Show <c>attribution</c> with the block. Colorado (additive): where this
    /// server reads CAIC's own feed, a Colorado point gets its CAIC forecast area's rating instead of
    /// avalanche.org's single statewide CAIC zone. The level is the HIGHEST of CAIC's three elevation bands (above,
    /// near and below treeline) for the current day, the convention of the avalanche.org map. CAIC regroups its
    /// areas as conditions change, so the area's <c>zone_id</c> can change from day to day, and <c>forecast_url</c>
    /// opens CAIC's map at the point. If CAIC's feed is stale or down, a Colorado point gets <c>unavailable</c>,
    /// never the statewide rating. A point in an area where CAIC issues no forecast gets <c>no_rating</c>. CAIC's
    /// areas have no names, so <c>zone_name</c> is <c>CAIC forecast area</c>. The Cap'n Proto body carries the same
    /// as <c>ForecastResponse.avalanche</c> (set when <c>hasAvalanche</c>).
    /// </summary>
    [JsonPropertyName("avalanche")]
    public AvalancheForecast? Avalanche { get; init; }

    /// <summary>
    /// The snow on the ground at the location's own point at the hour the request is served, from the snowpack
    /// model. Present only where a snowpack column runs for the point and its newest run is not stale; there is no
    /// other source for this number, so elsewhere the field is absent. A sensor's own measured depth is on its
    /// sensor row (<c>/current</c>).
    /// </summary>
    [JsonPropertyName("snow_on_ground")]
    public SnowOnGround? SnowOnGround { get; init; }
}
