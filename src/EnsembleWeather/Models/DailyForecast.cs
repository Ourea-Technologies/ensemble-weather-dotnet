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
    public Condition Condition { get; init; }

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
    /// The day's precipitation type, <c>null</c> when <c>precipitation_occurrence_probability</c> is under 0.15. Read off
    /// the day's HOURLY phases: the day's snow share is the precipitation-weighted mean of the
    /// <c>timeseries[].precipitation_type.snow_probability</c> of its wet hours (the share of each hour's water that falls
    /// as snow), read at that field's 0.01 resolution — <c>snow</c> when at least 99.5% of the day's water is snow,
    /// <c>rain</c> when under 0.5% is (or no hour is typed), <c>rain_snow_mix</c> between. So a day whose wet hours
    /// are all <c>rain_snow_mix</c> is <c>rain_snow_mix</c>, and a rain day with one heavy snow hour is
    /// <c>rain_snow_mix</c>, not <c>rain</c>. <c>condition</c> states this type's phase at its tier: a Snow or
    /// Rain/Snow Mix label, or a liquid label. <c>freezing_rain</c> is an hourly type only; such hours count as
    /// liquid.
    /// </summary>
    [JsonPropertyName("precipitation_type")]
    public PrecipitationKind? PrecipitationType { get; init; }

    /// <summary>
    /// Calendar-day maximum: the max over local midnight to midnight on <c>date</c> (earlier releases: 06:00-18:00
    /// only). On day 0, the part of the day the model run covers.
    /// </summary>
    [JsonPropertyName("high")]
    public Quantiles High { get; init; } = null!;

    /// <summary>
    /// Calendar-day minimum: the min over local midnight to midnight on <c>date</c>, the same hours as <c>high</c>
    /// (earlier releases: the night that BEGINS on <c>date</c>, 18:00 through 06:00 the next morning). On day 0, the
    /// part of the day the model run covers. Label it "Low", not "Tonight".
    /// </summary>
    [JsonPropertyName("low")]
    public Quantiles Low { get; init; } = null!;

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
    /// The day's total precipitation.
    /// </summary>
    [JsonPropertyName("total_precipitation")]
    public Quantiles TotalPrecipitation { get; init; } = null!;

    /// <summary>
    /// The day's total snowfall.
    /// </summary>
    [JsonPropertyName("total_snowfall")]
    public Quantiles? TotalSnowfall { get; init; }

    /// <summary>
    /// **MEANINGFUL precipitation: <c>P(day total &gt;= 2.5 mm)</c>** (0-1) — the headline number, the chance the day
    /// accumulates a tenth of an inch or more. For "will it precipitate at all", read
    /// <c>precipitation_occurrence_probability</c> instead; the two answer different questions and <c>precipitation_probability</c>
    /// is always the smaller of the two.
    /// </summary>
    [JsonPropertyName("precipitation_probability")]
    public double PrecipitationProbability { get; init; }

    /// <summary>
    /// **OCCURRENCE: <c>P(day total &gt;= 1.0 mm)</c>** (0-1) — the chance the day precipitates at all, as opposed to
    /// <c>precipitation_probability</c>'s chance that it amounts to anything. This is the number <c>condition</c>'s tier and
    /// <c>precipitation_type</c>'s dry gate read; their PHASE comes from the day's hourly phases (see <c>precipitation_type</c>).
    /// </summary>
    [JsonPropertyName("precipitation_occurrence_probability")]
    public double PrecipitationOccurrenceProbability { get; init; }

    /// <summary>
    /// <c>P(day snowfall total &gt;= 2.5 mm)</c> (0-1), inclusive. Same estimator, threshold and caveats as
    /// <c>precipitation_probability</c> — see its description. There is no snowfall counterpart to
    /// <c>precipitation_occurrence_probability</c>: snow occurrence is not separately calibrated.
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
    /// The day total GIVEN a wet day (total &gt;= the <c>daily_precipitation_occurrence_probability</c> threshold, 1.0 mm):
    /// the upper part of the day-total distribution, renormalized. Not the mean divided by the probability. Omitted
    /// under a 0.1 % chance.
    /// </summary>
    [JsonPropertyName("total_precipitation_if_wet")]
    public Quantiles? TotalPrecipitationIfWet { get; init; }

    /// <summary>
    /// The day's snowfall given a snowy day (&gt;= the <c>daily_snow_probability</c> threshold, 2.5 mm), as
    /// <c>total_precipitation_if_wet</c>.
    /// </summary>
    [JsonPropertyName("total_snowfall_if_wet")]
    public Quantiles? TotalSnowfallIfWet { get; init; }

    /// <summary>
    /// The day's precipitation headline.
    /// </summary>
    [JsonPropertyName("precipitation_display")]
    public PrecipitationDisplay PrecipitationDisplay { get; init; } = null!;

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
    public DailyAstronomy Astronomy { get; init; } = null!;

    /// <summary>
    /// The day's temperature climatology.
    /// </summary>
    [JsonPropertyName("temperature_climatology")]
    public DayClimatology? TemperatureClimatology { get; init; }

    /// <summary>
    /// Present only on a part day -- the leading date of a run whose reference init falls after its local midnight
    /// (today, for most cycles) -- and absent on a whole day. The instant (UTC) from which the row's values
    /// describe the date: its <c>high</c>, <c>low</c>, totals and probabilities are over <c>covered_from</c> to the
    /// end of the local day, not the whole calendar day. The hours between local midnight and <c>covered_from</c>
    /// are not in the forecast series, so a part day's total can be well under the day's; compare a whole day with
    /// the climate (<c>departure</c> and <c>percentile</c> are absent on a part day for the same reason). One
    /// exception. With the nowcast applied, the precipitation total, its probabilities and its if-wet amounts also
    /// include the radar-measured precipitation of the hours between local midnight and <c>covered_from</c> (MRMS
    /// radar-only, where the radar observed the point well), so today's total keeps the rain that already fell when
    /// the reference run advances. The high and low still start at <c>covered_from</c>. The snowfall total includes
    /// the snow of that measured precipitation, each measured hour's water at the snow-to-water ratio of the older
    /// run's forecast for that hour, and the day's precipitation type and label count it as that forecast's type
    /// (snow on a cold day, not rain); until that older run's forecast has been read for the point (in the
    /// background, after each new run), the measured water counts without snow, as before. Not on a pinned
    /// <c>run</c>. Its row starts at the run's init in every value: no hour before the init is counted, measured or
    /// forecast.
    /// </summary>
    [JsonPropertyName("covered_from")]
    public DateTimeOffset? CoveredFrom { get; init; }

    /// <summary>
    /// Present only on a day part of which is a measurement -- the leading date, when the radar (MRMS radar-only)
    /// measured hours between its local midnight and the reference init -- and absent otherwise. Always absent on a
    /// pinned <c>run</c>: a pinned row counts nothing from before the run's init, so its <c>total_precipitation</c>
    /// and <c>total_snowfall</c> are its served hours'. <c>amount</c> is that measured precipitation, in the
    /// response's precipitation unit (as <c>precipitation_display.amount</c>); <c>through</c> is the end (UTC) of
    /// its last measured hour. The day's <c>total_precipitation</c> and <c>total_snowfall</c> include it with its
    /// measurement error: its band is no longer only the forecast hours' spread, because a radar amount is itself
    /// uncertain (against MRMS MultiSensor QPE, the scoring truth, a radar-only day sum runs from about 0.7x to
    /// 1.2x at 15 mm or more, wider when lighter). The day's <c>mean</c> does not count <c>amount</c> in full, so a
    /// first day's <c>total_precipitation.mean</c> can be below <c>measured_precipitation.amount</c>. <c>amount</c>
    /// is the radar's own sum; the total counts it at the measured ratio of truth to radar for an amount of its
    /// size, about 0.87 to 0.93: 0.914 of it up to 1.9 mm, 0.868 at 9.0 mm, 0.925 from 22.1 mm (the radar amount in
    /// mm, whatever the response unit). Between those amounts the water counted rises in a straight line with the
    /// amount measured, so the ratio has no step and more measured water never counts for less. Example: 11.33 mm
    /// measured counts as 10.06 mm (0.888), and with 0.02 mm in the hours still forecast the day's <c>mean</c> is
    /// 10.08 mm. Each percentile takes the ratio's own quantile the same way.
    /// </summary>
    [JsonPropertyName("measured_precipitation")]
    public MeasuredPrecipitation? MeasuredPrecipitation { get; init; }

    /// <summary>
    /// The day's snow character. <c>wet</c> when half or more of the water that fell as the day's snow fell in
    /// hours whose snow was wet (each hour weighted by its <c>ensemble.snowfall.mean</c> over its
    /// <c>ensemble.slr.p50</c>, the water it brought); otherwise the density class of the day's snow-to-liquid
    /// ratio, <c>slr</c>: its snow over the water that fell as that snow, the way a day's ratio is observed. A
    /// heavy dense hour outweighs a dusting of powder. Present exactly when the day serves snowfall
    /// (<c>total_snowfall.mean</c> above 0) and <c>precipitation_display.kind</c> is not <c>none</c>; absent
    /// otherwise. There is no daily Light/Heavy Snow condition: intensity is a rate, and the NWS heavy-snow amount
    /// criteria differ by region.
    /// </summary>
    [JsonPropertyName("snow_character")]
    public SnowCharacter? SnowCharacter { get; init; }

    /// <summary>
    /// The day's snow-to-liquid ratio, to one decimal: the ratio <c>snow_character</c> classifies on a day that is
    /// not <c>wet</c>. Present exactly when <c>snow_character</c> is.
    /// </summary>
    [JsonPropertyName("slr")]
    public double? SnowToLiquidRatio { get; init; }

    /// <summary>
    /// The snow level over the day's wet hours -- the hours the day's <c>precipitation_type</c> reads (the hours
    /// that serve a precipitation type): the lowest and highest hourly <c>ensemble.snow_level.mean</c> and their
    /// precipitation-weighted mean. <c>Snow_level</c> and <c>snow_level_mean</c> read the same hours with the same
    /// weights on such a day, so <c>snow_level.mean</c> is this <c>mean</c>; the p10-p90 of <c>snow_level</c> is
    /// the members' spread, and this is the day's own range in time while it precipitates. Omitted on a day with no
    /// wet hour. The RAIN LINE, as <c>snow_level</c> is.
    /// </summary>
    [JsonPropertyName("snow_level_range")]
    public SnowLevelRange? SnowLevelRange { get; init; }

    /// <summary>
    /// The day's snow level on the NWS scale -- the height of the 0.5 C wet-bulb. It is the day's <c>snow_level</c>
    /// on that scale, the same statistic over the same hours -- each rank the mean of the hourly
    /// <c>ensemble.snow_level_wet_bulb</c> ranks over the hours <c>snow_level</c> reads (the wet hours, weighted by
    /// their precipitation, on a day that has any; every hour on a dry day) -- so each rank is the
    /// <c>snow_level</c> rank raised by 275 m (901 ft), as on the hour, and <c>mean</c> is
    /// <c>snow_level_wet_bulb_range.mean</c>. (An hourly rank at <c>0</c>, snow to sea level, is <c>0</c> on both
    /// scales, so on a day with such hours a rank is less than 275 m above <c>snow_level</c>'s, and is <c>0</c>
    /// only where every hour it reads is.) Omitted when no hour serves a snow level.
    /// </summary>
    [JsonPropertyName("snow_level_wet_bulb")]
    public Quantiles? SnowLevelWetBulb { get; init; }

    /// <summary>
    /// <c>Snow_level_range</c> on the NWS scale -- over the same wet hours, the lowest and highest hourly
    /// <c>ensemble.snow_level_wet_bulb.mean</c> and their precipitation-weighted mean (which is
    /// <c>snow_level_wet_bulb.mean</c>). Each is the <c>snow_level_range</c> value raised by 275 m (901 ft), an
    /// hour at <c>0</c> (snow to sea level) counted at <c>0</c>. Show this range as the day's "snow level". Omitted
    /// exactly when <c>snow_level_range</c> is.
    /// </summary>
    [JsonPropertyName("snow_level_wet_bulb_range")]
    public SnowLevelRange? SnowLevelWetBulbRange { get; init; }

    /// <summary>
    /// The day's climate normal at the forecast point: the mean, over the day of year +/-15 days of every year of
    /// the station's record, of the very quantities this row states -- the local CALENDAR date's maximum hourly
    /// temperature (<c>high</c>, the quantity <c>high</c> is), its minimum (<c>low</c>) and its precipitation TOTAL
    /// (<c>precipitation</c>, dry days included; a daily total, never an hourly amount). Units follow the request's
    /// <c>units</c> (°C / mm metric, °F / in imperial), converted as the row's own values are; <c>distance_km</c>
    /// is always km. Where it comes from. The nearest climatology station within 60 km of the point and within 300
    /// m of its elevation (a station far above or below the point describes another climate, and no lapse rate
    /// carries a normal across a valley inversion) that holds calendar-day normals (<c>source: station</c>, today a
    /// SNOTEL station's own hourly record, at least ten years of complete local days); else the ERA5 0.25° cell the
    /// point lies in (<c>source: era5_cell</c>). The ERA5 normals are sampled at four hours a day and state no
    /// calendar-day maximum, minimum or total, so no cell answers today. No approximation: where neither has a
    /// normal the whole object is absent, and an absent member (<c>precipitation</c> in a window too dry to fit) is
    /// absent rather than estimated. The source is credited in <c>attribution</c>.
    /// </summary>
    [JsonPropertyName("normal")]
    public DayNormal? Normal { get; init; }

    /// <summary>
    /// The served headline minus the normal, in the request's units: <c>high.p50 - normal.high</c> and <c>low.p50 -
    /// normal.low</c>. Positive is warmer than normal. Present only with <c>normal</c>, and absent on a date the
    /// forecast covers only from partway through (the first date of a run that starts after local midnight), whose
    /// high and low are over part of the day while the normal is over all of it.
    /// </summary>
    [JsonPropertyName("departure")]
    public DayDeparture? Departure { get; init; }

    /// <summary>
    /// How unusual the day is: where the served headline (<c>p50</c>) falls in the day's climatological
    /// distribution, as a percentile 0-100, <c>Pr{climate &lt;= forecast}</c> -- 95 for <c>precipitation</c> reads
    /// "wetter than 95% of this date's days". <c>high</c> and <c>low</c> read the normal's Gaussian (mean and
    /// standard deviation of the calendar-day maximum / minimum); <c>precipitation</c> reads its zero-inflated
    /// gamma (a dry atom plus a gamma fitted to the wet days), so a dry forecast sits AT the atom and reads the
    /// climatological share of dry days. A member is absent where its normal is, or where that normal has no
    /// distribution to place a value in (no fitted mixture, zero spread). Absent as <c>departure</c> is.
    /// </summary>
    [JsonPropertyName("percentile")]
    public DayPercentile? Percentile { get; init; }
}
