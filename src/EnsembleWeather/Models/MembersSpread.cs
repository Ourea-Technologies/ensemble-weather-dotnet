// SPDX-License-Identifier: BSD-2-Clause

using System.Text.Json.Serialization;

namespace EnsembleWeather;

/// <summary>
/// The members' spread at this hour for the plume's instantaneous variables -- the Quantiles of every member's own
/// <c>temperature</c> (temperature unit) and <c>snow_level</c> (length unit) here, over the members as served. Draw
/// it as the plume's band on the temperature and snow-level panels: the hour's <c>ensemble.temperature</c> /
/// <c>ensemble.snow_level</c> percentiles are the calibrated blend's distribution, a different quantity. Present
/// with <c>members=true</c> at an hour where at least two members carry a value -- in practice the 6-hourly
/// synoptic hours, as <c>members_cumulative</c>; a quantity fewer than two members carry is omitted. Equal-weight
/// member spread.
/// </summary>
public sealed class MembersSpread
{
    /// <summary>
    /// A distribution over the ensemble MEMBERS (<c>members_cumulative</c>, <c>members_spread</c>,
    /// <c>members_6h</c>): the <c>Quantiles</c> shape. <c>P50</c>, the members' median, is the plume's paid detail
    /// (the <c>plume_detail</c> entitlement: Plus and Pro) and is ABSENT for Free and anonymous callers
    /// (<c>members_view: pool</c>); the mean, <c>p10</c>, <c>p25</c>, <c>p75</c>, <c>p90</c>, <c>min</c> and
    /// <c>max</c> are served to every caller. This does not touch the forecast's own calibrated quantiles
    /// (<c>ensemble.&lt;variable&gt;.p50</c>, the daily statistics).
    /// </summary>
    [JsonPropertyName("temperature")]
    public MemberQuantiles? Temperature { get; init; }

    /// <summary>
    /// A distribution over the ensemble MEMBERS (<c>members_cumulative</c>, <c>members_spread</c>,
    /// <c>members_6h</c>): the <c>Quantiles</c> shape. <c>P50</c>, the members' median, is the plume's paid detail
    /// (the <c>plume_detail</c> entitlement: Plus and Pro) and is ABSENT for Free and anonymous callers
    /// (<c>members_view: pool</c>); the mean, <c>p10</c>, <c>p25</c>, <c>p75</c>, <c>p90</c>, <c>min</c> and
    /// <c>max</c> are served to every caller. This does not touch the forecast's own calibrated quantiles
    /// (<c>ensemble.&lt;variable&gt;.p50</c>, the daily statistics).
    /// </summary>
    [JsonPropertyName("snow_level")]
    public MemberQuantiles? SnowLevel { get; init; }

    /// <summary>
    /// The Quantiles of the members' own <c>snow_level_wet_bulb</c> (the snow level on the NWS scale), over the
    /// members <c>snow_level</c> is over. Draw it as the band of a snow-level panel labelled on the NWS scale.
    /// Omitted when <c>snow_level</c> is.
    /// </summary>
    [JsonPropertyName("snow_level_wet_bulb")]
    public MemberQuantiles? SnowLevelWetBulb { get; init; }

    /// <summary>
    /// The snow-to-liquid ratio over the members SNOWING at this hour (their own <c>snowfall</c> above 0), at
    /// 00/06/12/18Z only, in both member views. Omitted at other hours and where fewer than five members snow,
    /// never a zero, so a dry step has no violin. It is the same diagnostic ratio as <c>ensemble.slr</c> and can
    /// differ from it: <c>ensemble.slr</c> is the weighted blend of every member's ratio, dry members included, at
    /// an hour whose served snowfall is above 0. As few as five snowing members can sit behind a step, not the
    /// whole ensemble. RRFS <c>member_id</c> 1..5 have no ratio.
    /// </summary>
    [JsonPropertyName("slr")]
    public MemberQuantiles? SnowToLiquidRatio { get; init; }

    /// <summary>
    /// The members' own wind speed at this hour, in the wind unit. Draw it as the band on the wind panel:
    /// <c>ensemble.wind_speed</c> is the calibrated blend's distribution, a different quantity. Omitted from the
    /// Free plume view, which withholds the members' wind.
    /// </summary>
    [JsonPropertyName("wind_speed")]
    public MemberQuantiles? WindSpeed { get; init; }
}
