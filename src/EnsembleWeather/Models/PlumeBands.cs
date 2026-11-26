// SPDX-License-Identifier: BSD-2-Clause

using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace EnsembleWeather;

/// <summary>
/// The members' distributions, at the served hours that carry at least one (<c>valid_time</c>; the synoptic hours
/// in practice): <c>cumulative_precipitation</c> / <c>cumulative_snowfall</c> are the row form's
/// <c>members_cumulative</c> (the spread of the members' running totals, never a sum of hourly percentiles),
/// <c>temperature</c> / <c>snow_level</c> its <c>members_spread</c>, <c>precipitation_6h</c> / <c>snowfall_6h</c>
/// its <c>members_6h</c> (6-hour totals ending at a 00/06/12/18Z hour), <c>slr</c> its <c>members_spread.slr</c>
/// (the key is absent when no hour carries it), and <c>cumulative_snowfall_10to1</c> the spread of the members'
/// running snowfall at 10:1. A band is <c>null</c> at an hour without it.
/// </summary>
public sealed class PlumeBands
{
    /// <summary>
    /// The valid times, one per column.
    /// </summary>
    [JsonPropertyName("valid_time")]
    public IReadOnlyList<DateTimeOffset> ValidTime { get; init; } = [];

    /// <summary>
    /// One band's statistics (Hyndman-Fan type 7 over the served member values), each an array aligned to
    /// <c>PlumeBands.valid_time</c>, <c>null</c> where the band is absent at that hour. <c>P50</c>, the members'
    /// median, is served to Plus and Pro (<c>plume_detail</c>) and is ABSENT, the key itself, for Free and
    /// anonymous callers (<c>members_view: pool</c>); every other column is served to every caller.
    /// </summary>
    [JsonPropertyName("cumulative_precipitation")]
    public BandColumns CumulativePrecipitation { get; init; } = null!;

    /// <summary>
    /// One band's statistics (Hyndman-Fan type 7 over the served member values), each an array aligned to
    /// <c>PlumeBands.valid_time</c>, <c>null</c> where the band is absent at that hour. <c>P50</c>, the members'
    /// median, is served to Plus and Pro (<c>plume_detail</c>) and is ABSENT, the key itself, for Free and
    /// anonymous callers (<c>members_view: pool</c>); every other column is served to every caller.
    /// </summary>
    [JsonPropertyName("cumulative_snowfall")]
    public BandColumns CumulativeSnowfall { get; init; } = null!;

    /// <summary>
    /// The members' running snowfall at 10:1 -- each member's snowfall with its SLR replaced by 10 (its snowing
    /// liquid x 10, every phase gate of its <c>snowfall</c> kept, the <c>snowfall_total_10to1</c> map's member
    /// rule), totalled as <c>cumulative_snowfall</c> is -- by <c>cumulative_snowfall</c>'s rule: one fixed set of
    /// members, so every rung never decreases. Not in the row form; RRFS display members are not in it.
    /// </summary>
    [JsonPropertyName("cumulative_snowfall_10to1")]
    public BandColumns CumulativeSnowfall10To1 { get; init; } = null!;

    /// <summary>
    /// One band's statistics (Hyndman-Fan type 7 over the served member values), each an array aligned to
    /// <c>PlumeBands.valid_time</c>, <c>null</c> where the band is absent at that hour. <c>P50</c>, the members'
    /// median, is served to Plus and Pro (<c>plume_detail</c>) and is ABSENT, the key itself, for Free and
    /// anonymous callers (<c>members_view: pool</c>); every other column is served to every caller.
    /// </summary>
    [JsonPropertyName("temperature")]
    public BandColumns Temperature { get; init; } = null!;

    /// <summary>
    /// One band's statistics (Hyndman-Fan type 7 over the served member values), each an array aligned to
    /// <c>PlumeBands.valid_time</c>, <c>null</c> where the band is absent at that hour. <c>P50</c>, the members'
    /// median, is served to Plus and Pro (<c>plume_detail</c>) and is ABSENT, the key itself, for Free and
    /// anonymous callers (<c>members_view: pool</c>); every other column is served to every caller.
    /// </summary>
    [JsonPropertyName("snow_level")]
    public BandColumns SnowLevel { get; init; } = null!;

    /// <summary>
    /// The row form's <c>members_spread.snow_level_wet_bulb</c> -- the members' snow level on the NWS scale, at the
    /// hours <c>snow_level</c> carries a band.
    /// </summary>
    [JsonPropertyName("snow_level_wet_bulb")]
    public BandColumns SnowLevelWetBulb { get; init; } = null!;

    /// <summary>
    /// One band's statistics (Hyndman-Fan type 7 over the served member values), each an array aligned to
    /// <c>PlumeBands.valid_time</c>, <c>null</c> where the band is absent at that hour. <c>P50</c>, the members'
    /// median, is served to Plus and Pro (<c>plume_detail</c>) and is ABSENT, the key itself, for Free and
    /// anonymous callers (<c>members_view: pool</c>); every other column is served to every caller.
    /// </summary>
    [JsonPropertyName("precipitation_6h")]
    public BandColumns Precipitation6Hours { get; init; } = null!;

    /// <summary>
    /// One band's statistics (Hyndman-Fan type 7 over the served member values), each an array aligned to
    /// <c>PlumeBands.valid_time</c>, <c>null</c> where the band is absent at that hour. <c>P50</c>, the members'
    /// median, is served to Plus and Pro (<c>plume_detail</c>) and is ABSENT, the key itself, for Free and
    /// anonymous callers (<c>members_view: pool</c>); every other column is served to every caller.
    /// </summary>
    [JsonPropertyName("snowfall_6h")]
    public BandColumns Snowfall6Hours { get; init; } = null!;

    /// <summary>
    /// One band's statistics (Hyndman-Fan type 7 over the served member values), each an array aligned to
    /// <c>PlumeBands.valid_time</c>, <c>null</c> where the band is absent at that hour. <c>P50</c>, the members'
    /// median, is served to Plus and Pro (<c>plume_detail</c>) and is ABSENT, the key itself, for Free and
    /// anonymous callers (<c>members_view: pool</c>); every other column is served to every caller.
    /// </summary>
    [JsonPropertyName("slr")]
    public BandColumns? SnowToLiquidRatio { get; init; }

    /// <summary>
    /// <c>members_spread.wind_speed</c>. Absent when no hour carries it, which is every hour of the Free view.
    /// </summary>
    [JsonPropertyName("wind_speed")]
    public BandColumns? WindSpeed { get; init; }
}
