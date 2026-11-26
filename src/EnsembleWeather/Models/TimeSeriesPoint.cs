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
    public Condition Condition { get; init; }

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
    [JsonPropertyName("precipitation_type")]
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
    public IReadOnlyList<ModelStatistics> ModelMeans { get => field ?? []; init; } = [];

    /// <summary>
    /// Per-member ensemble values, one <c>{id, members}</c> per model with members at this hour, in
    /// <c>model_run.models</c> order. Only present with <c>?members=true</c>
    /// -- for spaghetti plots and plumes. A model with no members at this hour is absent. The 6-hourly models have
    /// members at every 6th hour, rrfs at every hour to 84 h.
    /// </summary>
    [JsonPropertyName("members")]
    public IReadOnlyList<ModelMembers> Members { get => field ?? []; init; } = [];

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

    /// <summary>
    /// Whether precipitation was MEASURED in this hour: MRMS published a radar accumulation for this clock hour
    /// (the whole hour just ended, the quarters published so far of the hour in progress, or an older hour whose
    /// measurement is kept) and it is served above 0: it rounds above 0.00 mm and above 0.000 in, so a measurement
    /// that would be served as 0 does not count. Such an hour's <c>condition_text</c> states the precipitation
    /// falling ("Light Snow"), while <c>ensemble.precipitation.probability</c> stays the chance that the hour's
    /// total reaches 0.254 mm, which for a trace can be a few percent: show "Measured" (or a radar mark) in place
    /// of that chance. False on every other hour: one the radar holds only through a scan's rate (a rate predicts
    /// the hour, it measures none of it), one the radar measured dry, a forecast hour, and every hour served
    /// without the nowcast layer (<c>nowcast=false</c>). Always present. Additive; Cap'n Proto
    /// <c>TimeSeriesPoint.measured</c>.
    /// </summary>
    [JsonPropertyName("measured")]
    public bool Measured { get; init; }

    /// <summary>
    /// The hour's snow character. <c>wet</c> when the hour's snow is wet at the surface (see <c>SnowCharacter</c>),
    /// whatever its ratio; otherwise the density class of <c>ensemble.slr.p50</c> at its served precision. Present
    /// exactly when the hour serves <c>ensemble.slr</c> (its snowfall is above 0); absent, never null, on an hour
    /// without snow.
    /// </summary>
    [JsonPropertyName("snow_character")]
    public SnowCharacter? SnowCharacter { get; init; }

    /// <summary>
    /// The members' spread at this hour for the plume's instantaneous variables -- the Quantiles of every member's
    /// own <c>temperature</c> (temperature unit) and <c>snow_level</c> (length unit) here, over the members as
    /// served. Draw it as the plume's band on the temperature and snow-level panels: the hour's
    /// <c>ensemble.temperature</c> / <c>ensemble.snow_level</c> percentiles are the calibrated blend's
    /// distribution, a different quantity. Present with <c>members=true</c> at an hour where at least two members
    /// carry a value -- in practice the 6-hourly synoptic hours, as <c>members_cumulative</c>; a quantity fewer
    /// than two members carry is omitted. Equal-weight member spread.
    /// </summary>
    [JsonPropertyName("members_spread")]
    public MembersSpread? MembersSpread { get; init; }

    /// <summary>
    /// The members' 6-HOUR totals ending at this hour -- per quantity (<c>precipitation</c>, <c>snowfall</c>), the
    /// Quantiles of every member's total over <c>(valid_time - 6 h, valid_time]</c>: the violin of a Utah-style
    /// plume, one per 6 h period. Served with <c>members=true</c> at the 00/06/12/18Z hours whose whole window lies
    /// in the served window (the window the running totals open on: one hour before the first served
    /// <c>valid_time</c>), so the first period after "now" is left out when "now" is inside it. A member counts
    /// when its values cover all six hours: a 6-hourly bucket ending here, an hourly member's six values, a bucket
    /// straddling the window's start counting its share inside (evenly spread, as the running totals count it).
    /// Equal-weight member spread, as <c>members_cumulative</c>; never sum hourly percentiles for it. Over another
    /// window use <c>?totals=</c>.
    /// </summary>
    [JsonPropertyName("members_6h")]
    public MembersCumulative? Members6Hours { get; init; }

    /// <summary>
    /// This hour's headline precipitation (<c>ensemble.precipitation.mean</c>) split by phase, in the precipitation
    /// units -- <c>rain + snow + rain_snow_mix + freezing_rain</c> is the hour's mean (to the served precision), so
    /// a stacked bar is the headline bar. The shares come from the members (<c>members=true</c>): each member value
    /// votes its precipitation, spread evenly over the hours its <c>precipitation_hours</c> span, as rain (none of
    /// it falls as snow), snow (all of it does) or rain/snow mix (part does) -- its share being its served snowfall
    /// over precipitation x SLR, the snowfall rule's melting-layer and surface shares, so the split agrees with the
    /// members' snowfall. A liquid member whose drops freeze on contact -- fully melted aloft over a sub-freezing
    /// surface, the rule <c>precipitation_type</c> calls freezing rain, applied to the member's own column -- votes
    /// <c>freezing_rain</c>, not <c>rain</c> (before it was counted in <c>rain</c>), so an hour whose
    /// <c>precipitation_type</c> is <c>freezing_rain</c> draws its icing members as freezing rain. Ice pellets are
    /// not split out: <c>precipitation_type</c> has no ice-pellet type (a partly melted drop over a frozen surface
    /// is <c>rain_snow_mix</c> in both). Served with <c>members=true</c> at every hour some member's precipitation
    /// covers; absent where no member's does, and where every covering member is dry under a wet headline (no phase
    /// to apportion). A dry hour serves zeros.
    /// </summary>
    [JsonPropertyName("precipitation_by_type")]
    public PrecipitationByType? PrecipitationByType { get; init; }
}
