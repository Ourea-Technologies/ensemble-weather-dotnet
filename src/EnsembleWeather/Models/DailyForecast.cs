// SPDX-License-Identifier: BSD-2-Clause

using System;
using System.Text.Json.Serialization;

namespace EnsembleWeather;

/// <summary>
/// Daily summary keyed by one local calendar date (the location's own US timezone, not always Pacific).
/// </summary>
public sealed class DailyForecast
{
    /// <summary>
    /// The local calendar date.
    /// </summary>
    [JsonPropertyName("date")]
    [JsonConverter(typeof(Serialization.CalendarDateConverter))]
    public DateTime Date { get; init; }

    /// <summary>
    /// A stable machine code to switch on -- the display text (<c>condition_text</c>) lowercased, each run of other
    /// characters one <c>_</c> (<c>partly_cloudy</c>, <c>rain_snow_mix_likely</c>). It was the display text. Treat
    /// unknown codes as additive.
    /// </summary>
    [JsonPropertyName("condition")]
    public WeatherCondition Condition { get; init; }

    /// <summary>
    /// Human-readable weather condition for the day — a daytime label, on the same rule as the hourly
    /// <c>condition</c>.
    /// </summary>
    [JsonPropertyName("condition_text")]
    public string ConditionText { get; init; } = string.Empty;

    /// <summary>
    /// Machine-readable icon hint for frontends. Sky-only hints name the five sky classes. Precipitating hints carry
    /// the SIDE of the sky: <c>sun_*</c> when the daytime sky leans clear (<c>P(&lt; 40%) &gt; P(&gt;= 80%)</c>) — a
    /// sun beside the rain cloud — and plain otherwise. Additive: <c>mostly_sunny</c>, <c>mostly_cloudy</c>,
    /// <c>sun_chance_showers</c>, <c>sun_showers_likely</c>, <c>sun_showers</c>, <c>chance_rain</c>,
    /// <c>showers_likely</c>, <c>showers</c>. Retired: <c>sun_rain</c>, <c>scattered_showers</c>,
    /// <c>night_scattered_showers</c>.
    /// </summary>
    [JsonPropertyName("condition_icon")]
    public ConditionIcon ConditionIcon { get; init; }

    /// <summary>
    /// The day's precipitation type, <c>null</c> when <c>precip_occurrence_probability</c> is under 0.15. Read off
    /// the day's HOURLY phases: the day's snow share is the precipitation-weighted mean of the
    /// <c>timeseries[].precip_type.snow_probability</c> of its wet hours (the share of each hour's water that falls
    /// as snow), read at that field's 0.01 resolution — <c>snow</c> when at least 99.5% of the day's water is snow,
    /// <c>rain</c> when under 0.5% is (or no hour is typed), <c>rain_snow_mix</c> between. So a day whose wet hours
    /// are all <c>rain_snow_mix</c> is <c>rain_snow_mix</c>, and a rain day with one heavy snow hour is
    /// <c>rain_snow_mix</c>, not <c>rain</c>. <c>condition</c> states this type's phase at its tier: a Snow or
    /// Rain/Snow Mix label, or a liquid label. <c>freezing_rain</c> is an hourly type only; such hours count as
    /// liquid.
    /// </summary>
    [JsonPropertyName("precip_type")]
    public PrecipitationKind? PrecipitationType { get; init; }

    /// <summary>
    /// Calendar-day maximum: the max over local midnight to midnight on <c>date</c> (earlier releases: 06:00-18:00
    /// only). On day 0, the part of the day the model run covers.
    /// </summary>
    [JsonPropertyName("high")]
    public Quantiles? High { get; init; }

    /// <summary>
    /// Calendar-day minimum: the min over local midnight to midnight on <c>date</c>, the same hours as <c>high</c>
    /// (earlier releases: the night that BEGINS on <c>date</c>, 18:00 through 06:00 the next morning). On day 0, the
    /// part of the day the model run covers. Label it "Low", not "Tonight".
    /// </summary>
    [JsonPropertyName("low")]
    public Quantiles? Low { get; init; }

    /// <summary>
    /// Calendar-day maximum of the hourly <c>feels_like</c> (the NWS apparent temperature), per member: each ensemble
    /// member's own hourly feels-like, that member's maximum over the same local calendar day as <c>high</c>, and the
    /// statistics across those members' maxima (<c>mean</c> their mean; <c>min</c>/<c>max</c> the lowest and highest
    /// member's). Every value is a day some member forecast. In wind it sits at or below <c>high</c>, in hot humid
    /// air at or above; on a mild day (above 10 C, below 26.7 C) it is the members' own daily high. Omitted when no
    /// member carries a feels-like through the window.
    /// </summary>
    [JsonPropertyName("feels_like_high")]
    public Quantiles? FeelsLikeHigh { get; init; }

    /// <summary>
    /// Calendar-day minimum of the hourly <c>feels_like</c> (the NWS apparent temperature), per member: each member's
    /// own minimum over the same local calendar day as <c>low</c>, and the statistics across those minima. It sits
    /// below <c>low</c> only by the wind chill of members that are both cold and windy. Omitted when no member
    /// carries a feels-like through the window.
    /// </summary>
    [JsonPropertyName("feels_like_low")]
    public Quantiles? FeelsLikeLow { get; init; }

    /// <summary>
    /// The daily UTCI maximum, built like <c>feels_like_high</c> from the hourly <c>feels_like_utci</c>. Present only
    /// with <c>feels_like=utci</c>, not served yet: absent today.
    /// </summary>
    [JsonPropertyName("feels_like_utci_high")]
    public Quantiles? FeelsLikeUtciHigh { get; init; }

    /// <summary>
    /// The daily UTCI minimum, built like <c>feels_like_low</c> from the hourly <c>feels_like_utci</c>. Present only
    /// with <c>feels_like=utci</c>, not served yet: absent today.
    /// </summary>
    [JsonPropertyName("feels_like_utci_low")]
    public Quantiles? FeelsLikeUtciLow { get; init; }

    /// <summary>
    /// The day's total precipitation.
    /// </summary>
    [JsonPropertyName("total_precip")]
    public Quantiles? TotalPrecipitation { get; init; }

    /// <summary>
    /// The day's total snowfall.
    /// </summary>
    [JsonPropertyName("total_snowfall")]
    public Quantiles? TotalSnowfall { get; init; }

    /// <summary>
    /// **MEANINGFUL precipitation: <c>P(day total &gt;= 2.5 mm)</c>** (0-1) — the headline number, the chance the day
    /// accumulates a tenth of an inch or more. For "will it precipitate at all", read
    /// <c>precip_occurrence_probability</c> instead; the two answer different questions and <c>precip_probability</c>
    /// is always the smaller of the two.
    /// </summary>
    [JsonPropertyName("precip_probability")]
    public double PrecipitationProbability { get; init; }

    /// <summary>
    /// **OCCURRENCE: <c>P(day total &gt;= 1.0 mm)</c>** (0-1) — the chance the day precipitates at all, as opposed to
    /// <c>precip_probability</c>'s chance that it amounts to anything. This is the number <c>condition</c>'s tier and
    /// <c>precip_type</c>'s dry gate read; their PHASE comes from the day's hourly phases (see <c>precip_type</c>).
    /// </summary>
    [JsonPropertyName("precip_occurrence_probability")]
    public double PrecipitationOccurrenceProbability { get; init; }

    /// <summary>
    /// <c>P(day snowfall total &gt;= 2.5 mm)</c> (0-1), inclusive. Same estimator, threshold and caveats as
    /// <c>precip_probability</c> — see its description. There is no snowfall counterpart to
    /// <c>precip_occurrence_probability</c>: snow occurrence is not separately calibrated.
    /// </summary>
    [JsonPropertyName("snow_probability")]
    public double SnowProbability { get; init; }

    /// <summary>
    /// The day's mean snow level.
    /// </summary>
    [JsonPropertyName("snow_level_mean")]
    public double? SnowLevelMean { get; init; }

    /// <summary>
    /// The day's snow level -- each rank the mean of the day's hourly ranks (the day-mean's distribution under the
    /// rank coherence the day totals assume). Omitted when no hour serves a snow level.
    /// </summary>
    [JsonPropertyName("snow_level")]
    public Quantiles? SnowLevel { get; init; }

    /// <summary>
    /// The day total GIVEN a wet day (total &gt;= the <c>daily_precip_occurrence_probability</c> threshold, 1.0 mm):
    /// the upper part of the day-total distribution, renormalized. Not the mean divided by the probability. Omitted
    /// under a 0.1 % chance.
    /// </summary>
    [JsonPropertyName("total_precip_if_wet")]
    public Quantiles? TotalPrecipitationIfWet { get; init; }

    /// <summary>
    /// The day's snowfall given a snowy day (&gt;= the <c>daily_snow_probability</c> threshold, 2.5 mm), as
    /// <c>total_precip_if_wet</c>.
    /// </summary>
    [JsonPropertyName("total_snowfall_if_wet")]
    public Quantiles? TotalSnowfallIfWet { get; init; }

    /// <summary>
    /// The day's precipitation headline.
    /// </summary>
    [JsonPropertyName("precip_display")]
    public PrecipitationDisplay? PrecipitationDisplay { get; init; }

    /// <summary>
    /// Average cloud cover for the day (0-100%). This is the mean of the day's hourly cloud-cover means and its
    /// meaning is unchanged — but it is NOT what <c>condition</c> is derived from: the sky label is the day's most
    /// likely cloud-cover CLASS, so a bimodal day can read <c>cloud_cover_mean</c> near 50 and still be labelled
    /// Cloudy.
    /// </summary>
    [JsonPropertyName("cloud_cover_mean")]
    public double? CloudCoverMean { get; init; }

    /// <summary>
    /// Daily wind summary. Uses gust data when available, otherwise sustained wind.
    /// </summary>
    [JsonPropertyName("wind")]
    public DailyWind? Wind { get; init; }

    /// <summary>
    /// How SLR changes over a snow storm. Only present on days with 3+ hours of snowfall. right_side_up: dense→fluffy
    /// (stable, powder on top). upside_down: fluffy→dense (weak base, avalanche risk). mixed: no clear trend.
    /// </summary>
    [JsonPropertyName("storm_orientation")]
    public StormOrientation? StormOrientation { get; init; }

    /// <summary>
    /// Plain-language companion to <c>storm_orientation</c>, suitable for direct display. Populated whenever
    /// <c>storm_orientation</c> is set — the pair is either both present or both absent. Example for upside_down:
    /// "Upside-down storm: fluffy snow fell first, denser snow on top. Weak base layer — elevated avalanche risk,
    /// heavier ski surface.".
    /// </summary>
    [JsonPropertyName("storm_orientation_description")]
    public string? StormOrientationDescription { get; init; }

    /// <summary>
    /// Sun and moon for the day.
    /// </summary>
    [JsonPropertyName("astro")]
    public DailyAstronomy? Astronomy { get; init; }

    /// <summary>
    /// The day's temperature climatology.
    /// </summary>
    [JsonPropertyName("temperature_climatology")]
    public DayClimatology? TemperatureClimatology { get; init; }
}
