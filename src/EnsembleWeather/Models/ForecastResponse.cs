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
    /// <see langword="null"/> <see cref="ProbabilityResult.Probability"/> means the series does not reach the
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
    public LocationReference Location { get; init; } = null!;

    /// <summary>
    /// The model run this forecast was served from.
    /// </summary>
    [JsonPropertyName("model_run")]
    public ModelRunInfo ModelRun { get; init; } = null!;

    /// <summary>
    /// The unit system every value in this body is in -- the request's <c>units=</c> (or its alias
    /// <c>unit_system=</c>), <c>metric</c> when neither is given. Always present, so a cached or shared body says
    /// whether <c>32.0</c> is °F or °C. The Cap'n Proto body carries the same as <c>ForecastResponse.units</c> (set
    /// when <c>hasUnits</c>).
    /// </summary>
    [JsonPropertyName("units")]
    public UnitSystem Units { get; init; }

    /// <summary>Gets the unit of each kind of quantity in this body, for <see cref="Units"/>.</summary>
    [JsonIgnore]
    public UnitLabels UnitLabels => UnitLabels.For(Units);

    /// <summary>
    /// The forecast series and daily summaries.
    /// </summary>
    [JsonPropertyName("forecast")]
    public ForecastData Forecast { get; init; } = null!;

    /// <summary>
    /// Forecasts at the location's other named elevations (ski areas), when requested.
    /// </summary>
    [JsonPropertyName("elevation_forecasts")]
    public IReadOnlyList<ElevationForecast> ElevationForecasts { get; init; } = [];

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
    public IReadOnlyList<NowcastSource> NowcastUnavailable { get; init; } = [];

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
    public IReadOnlyList<NearestSensor> NearestSensors { get; init; } = [];

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
}
