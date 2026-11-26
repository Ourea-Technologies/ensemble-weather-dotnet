// SPDX-License-Identifier: BSD-2-Clause

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace EnsembleWeather;

/// <summary>
/// One served series' members in columns (<c>members=plume</c>). Every number is the row form's
/// (<c>members=true</c>) served value for the same request, at its served precision ("Precision"), in the caller's
/// units: temperature 0.1, an amount (precipitation, snowfall, a running total) 0.01 mm / 0.001 in, a height (snow
/// level) 1 m / 1 ft, wind speed 0.1, a direction 1 degree, SLR 0.1; <c>precipitation_hours</c> is a whole count.
/// Inside a column <c>null</c> means "this member published nothing at this step"; a variable the view withholds
/// (Free and Plus: <c>wind_speed</c>, <c>wind_direction</c>) is ABSENT. By tier (<c>members_view</c>). Plus
/// (<c>plume</c>) and Pro (<c>full</c>): <c>models[]</c>, the members by model on each model's own steps with each
/// model's median and mean lines, and every band with its <c>p50</c>; no <c>members</c> key. Free and anonymous
/// (<c>pool</c>): <c>models</c> is an EMPTY array, <c>members</c> carries every member as one unlabelled pool on
/// the bands' 6-hour grid (<c>PooledColumns</c>), and no band has a <c>p50</c>; <c>headline</c> and every band's
/// <c>mean</c>, <c>min</c>, <c>p10</c>, <c>p25</c>, <c>p75</c>, <c>p90</c>, <c>max</c> are the same numbers a paid
/// caller gets for the same request.
/// </summary>
public sealed class MemberPlume
{
    /// <summary>
    /// The lines a plume draws over its members, at every served hour (<c>valid_time</c>): the blend's
    /// <c>temperature</c> and <c>snow_level</c> means (<c>ensemble.*.mean</c>) and its running totals
    /// (<c>cumulative_precipitation</c>, <c>cumulative_snowfall</c>: the "Ourea forecast" line), and that snowfall
    /// line at a 10:1 snow-to-liquid ratio (<c>cumulative_snowfall_10to1</c>), and its <c>wind_speed</c> mean and
    /// its snowfall in each 6 hours (<c>snowfall_6h</c>). <c>null</c> where the hour serves none.
    /// </summary>
    [JsonPropertyName("headline")]
    public PlumeHeadline Headline { get; init; } = null!;

    /// <summary>
    /// The members' distributions, at the served hours that carry at least one (<c>valid_time</c>; the synoptic
    /// hours in practice): <c>cumulative_precipitation</c> / <c>cumulative_snowfall</c> are the row form's
    /// <c>members_cumulative</c> (the spread of the members' running totals, never a sum of hourly percentiles),
    /// <c>temperature</c> / <c>snow_level</c> its <c>members_spread</c>, <c>precipitation_6h</c> /
    /// <c>snowfall_6h</c> its <c>members_6h</c> (6-hour totals ending at a 00/06/12/18Z hour), <c>slr</c> its
    /// <c>members_spread.slr</c> (the key is absent when no hour carries it), and <c>cumulative_snowfall_10to1</c>
    /// the spread of the members' running snowfall at 10:1. A band is <c>null</c> at an hour without it.
    /// </summary>
    [JsonPropertyName("bands")]
    public PlumeBands Bands { get; init; } = null!;

    /// <summary>
    /// One entry per model with members in the served window, in the order aifs, aigefs, gefs, ifs_ens, rrfs. Empty
    /// for Free and anonymous callers (<c>members_view: pool</c>), who are served <c>members</c> instead.
    /// </summary>
    [JsonPropertyName("models")]
    public IReadOnlyList<ModelColumns> Models { get => field ?? []; init; } = [];

    /// <summary>
    /// (JSON <c>members</c>) The members as ONE unlabelled pool, for a caller without the plume's paid detail (Free and anonymous;
    /// <c>members_view: pool</c>). Present only in that view. Every column is <c>[member][step]</c>: row <c>i</c>
    /// is one member, the same member in every column and on every elevation of the response; there is no member id
    /// and no model. The rows are in an order fixed for the run and the location that does not follow the models (a
    /// new run reshuffles; the same request draws the same lines). RRFS's display-only members are not in the pool.
    /// <c>valid_time</c> is the pool's one grid: the served hours that carry a member distribution
    /// (<c>bands.valid_time</c>'s 6-hourly synoptic hours), up to the last hour the running-total bands are served
    /// (where a global ensemble's run ends a step before the served window does, the pool ends with it, so the
    /// global ensembles' members all run to the same last step); a member with no value at a step is <c>null</c>
    /// there (a short-range member is <c>null</c> from its last step on). Values are the members' own served
    /// values, in the columns' units and precision (<c>MemberPlume</c>), except an hourly member's
    /// <c>precipitation</c> and <c>snowfall</c>, restated over the grid step as the rise of its own running total
    /// since the previous step. What is not hidden: <c>model_run.models[]</c> states each model's member count; a
    /// member that ends early is a shorter-range model's; a row missing from a <c>*_band_members</c> list ended
    /// before that band did.
    /// </summary>
    [JsonPropertyName("members")]
    public PooledColumns? Pool { get; init; }
}
