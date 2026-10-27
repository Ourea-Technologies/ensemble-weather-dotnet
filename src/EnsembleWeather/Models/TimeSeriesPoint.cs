// SPDX-License-Identifier: BSD-2-Clause

using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace EnsembleWeather;

/// <summary>
/// One hour of the forecast: the ensemble distribution of every variable, plus the hour's condition.
/// </summary>
public sealed class TimeSeriesPoint
{
    /// <summary>
    /// The instant this point is valid for.
    /// </summary>
    [JsonPropertyName("valid_time")]
    public DateTimeOffset ValidTime { get; init; }

    /// <summary>
    /// Hours since the model run's init.
    /// </summary>
    [JsonPropertyName("lead_time_hours")]
    public int LeadTimeHours { get; init; }

    /// <summary>
    /// Whether this hour is nighttime at the forecast location.
    /// </summary>
    [JsonPropertyName("is_night")]
    public bool IsNight { get; init; }

    /// <summary>
    /// A stable machine code to switch on -- the display text (<c>condition_text</c>) lowercased, each run of other
    /// characters one <c>_</c> (<c>partly_cloudy</c>, <c>rain_snow_mix_likely</c>). It was the display text. Treat
    /// unknown codes as additive.
    /// </summary>
    [JsonPropertyName("condition")]
    public WeatherCondition Condition { get; init; }

    /// <summary>
    /// Human-readable weather condition for the hour, day/night aware. One rule shared with the daily
    /// <c>condition</c>: a SKY class crossed with a PRECIPITATION tier.
    /// </summary>
    [JsonPropertyName("condition_text")]
    public string ConditionText { get; init; } = string.Empty;

    /// <summary>
    /// Icon hint (prefixed with night_ for nighttime). Sky-only hints name the five sky classes. Precipitating hints
    /// carry the SIDE of the sky: <c>sun_*</c> (<c>night_moon_*</c>) when the sky leans clear (<c>P(&lt; 40%) &gt;
    /// P(&gt;= 80%)</c>) — a sun or moon beside the rain cloud — and plain otherwise. Additive:
    /// <c>sun_chance_showers</c>, <c>sun_showers_likely</c>, <c>night_moon_chance_showers</c>,
    /// <c>night_moon_showers_likely</c>, <c>mostly_sunny</c>, <c>mostly_cloudy</c>, <c>night_mostly_clear</c>,
    /// <c>night_mostly_cloudy</c>, <c>showers_likely</c>, <c>night_showers_likely</c>. Retired: <c>sun_rain</c>,
    /// <c>scattered_showers</c>, <c>night_scattered_showers</c>, <c>rain_likely</c>, <c>night_rain_likely</c> (the
    /// hourly never says "likely rain"; an hour below the definite tier is showers). Snow, mix and freezing-rain
    /// conditions take their phase's icon at every tier (<c>snow</c>, <c>rain_snow_mix</c>, <c>freezing_rain</c>, and
    /// the <c>night_</c> forms). Additive: <c>freezing_rain</c>, <c>night_freezing_rain</c>.
    /// </summary>
    [JsonPropertyName("condition_icon")]
    public ConditionIcon ConditionIcon { get; init; }

    /// <summary>
    /// Whether the hour's gust is worth showing beside the wind -- its median at least 1.3 times the wind's median
    /// and at least 5 mph (2.2 m/s) above it. Omitted where the hour serves no gust.
    /// </summary>
    [JsonPropertyName("gust_notable")]
    public bool? GustNotable { get; init; }

    /// <summary>
    /// Present only when precipitation &gt; 0.
    /// </summary>
    [JsonPropertyName("precip_type")]
    public PrecipitationTypeInfo? PrecipitationType { get; init; }

    /// <summary>
    /// Present only when a temperature inversion or cold pool is detected for this hour.
    /// </summary>
    [JsonPropertyName("inversion")]
    public InversionInfo? Inversion { get; init; }

    /// <summary>
    /// The hour's freezing level. Omitted when the profile has no level above the surface to read it from.
    /// </summary>
    [JsonPropertyName("freezing_level")]
    public FreezingLevel? FreezingLevel { get; init; }

    /// <summary>
    /// The ensemble distribution of every variable at this hour.
    /// </summary>
    [JsonPropertyName("ensemble")]
    public EnsembleStatistics Ensemble { get; init; } = null!;

    /// <summary>
    /// Present only with <c>?diagnostics=provenance</c> -- which post-processing rule shaped each variable this hour.
    /// A diagnostic for research and verification, not for display.
    /// </summary>
    [JsonPropertyName("provenance")]
    public HourProvenance? Provenance { get; init; }

    /// <summary>
    /// Per-model statistics at this hour (Pro, <c>models=true</c>).
    /// </summary>
    [JsonPropertyName("model_means")]
    public IReadOnlyList<ModelStatistics> ModelMeans { get; init; } = [];

    /// <summary>
    /// Per-member ensemble values, one <c>{id, members}</c> per model with members at this hour, in
    /// <c>model_run.models</c> order (D-8: it was an object keyed by model). Only present with <c>?members=true</c>
    /// -- for spaghetti plots and plumes. A model with no members at this hour is absent. The 6-hourly models have
    /// members at every 6th hour, rrfs at every hour to 84 h.
    /// </summary>
    [JsonPropertyName("members")]
    public IReadOnlyList<ModelMembers> Members { get; init; } = [];

    /// <summary>
    /// The distribution of the members' running totals at this synoptic hour.
    /// </summary>
    [JsonPropertyName("members_cumulative")]
    public MembersCumulative? MembersCumulative { get; init; }

    /// <summary>
    /// The HEADLINE's running precipitation total -- the sum of <c>ensemble.precipitation.mean</c> from the first
    /// served hour ("now") through this one, in the request's units. The running line <c>snow_outlook</c> is the 24 h
    /// / 48 h / 7 d point of; draw it on the plume beside the members' band. A mean is additive, so this is exact;
    /// percentiles are not, so there is no running headline band (the members' is <c>members_cumulative</c>). Never
    /// compute it on the client.
    /// </summary>
    [JsonPropertyName("cumulative_precipitation")]
    public double? CumulativePrecipitation { get; init; }

    /// <summary>
    /// The headline's running snowfall total, as <c>cumulative_precipitation</c> over <c>ensemble.snowfall.mean</c>;
    /// present exactly when the hour serves <c>ensemble.snowfall</c>.
    /// </summary>
    [JsonPropertyName("cumulative_snowfall")]
    public double? CumulativeSnowfall { get; init; }
}
