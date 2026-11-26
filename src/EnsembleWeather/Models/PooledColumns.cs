// SPDX-License-Identifier: BSD-2-Clause

using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace EnsembleWeather;

/// <summary>
/// The members as ONE unlabelled pool, for a caller without the plume's paid detail (Free and anonymous;
/// <c>members_view: pool</c>). Present only in that view. Every column is <c>[member][step]</c>: row <c>i</c> is
/// one member, the same member in every column and on every elevation of the response; there is no member id and no
/// model. The rows are in an order fixed for the run and the location that does not follow the models (a new run
/// reshuffles; the same request draws the same lines). RRFS's display-only members are not in the pool.
/// <c>valid_time</c> is the pool's one grid: the served hours that carry a member distribution
/// (<c>bands.valid_time</c>'s 6-hourly synoptic hours), up to the last hour the running-total bands are served
/// (where a global ensemble's run ends a step before the served window does, the pool ends with it, so the global
/// ensembles' members all run to the same last step); a member with no value at a step is <c>null</c> there (a
/// short-range member is <c>null</c> from its last step on). Values are the members' own served values, in the
/// columns' units and precision (<c>MemberPlume</c>), except an hourly member's <c>precipitation</c> and
/// <c>snowfall</c>, restated over the grid step as the rise of its own running total since the previous step. What
/// is not hidden: <c>model_run.models[]</c> states each model's member count; a member that ends early is a
/// shorter-range model's; a row missing from a <c>*_band_members</c> list ended before that band did.
/// </summary>
public sealed class PooledColumns
{
    /// <summary>
    /// The valid times, one per column.
    /// </summary>
    [JsonPropertyName("valid_time")]
    public IReadOnlyList<DateTimeOffset> ValidTime { get; init; } = [];

    /// <summary>
    /// <c>[member][step]</c>: one array per member (<c>member_ids</c> order) over the model's <c>valid_time</c>;
    /// <c>null</c> = no value at that step.
    /// </summary>
    [JsonPropertyName("temperature")]
    public IReadOnlyList<IReadOnlyList<double?>> Temperature { get; init; } = [];

    /// <summary>
    /// <c>[member][step]</c>: one array per member (<c>member_ids</c> order) over the model's <c>valid_time</c>;
    /// <c>null</c> = no value at that step.
    /// </summary>
    [JsonPropertyName("precipitation")]
    public IReadOnlyList<IReadOnlyList<double?>> Precipitation { get; init; } = [];

    /// <summary>
    /// <c>[member][step]</c>: the hours each <c>precipitation</c> and <c>snowfall</c> value covers: the grid step
    /// (6), less where a member's first value covers less; <c>null</c> where the value is absent.
    /// </summary>
    [JsonPropertyName("precipitation_hours")]
    public IReadOnlyList<IReadOnlyList<int?>> PrecipitationHours { get; init; } = [];

    /// <summary>
    /// <c>[member][step]</c>: one array per member (<c>member_ids</c> order) over the model's <c>valid_time</c>;
    /// <c>null</c> = no value at that step.
    /// </summary>
    [JsonPropertyName("cumulative_precipitation")]
    public IReadOnlyList<IReadOnlyList<double?>> CumulativePrecipitation { get; init; } = [];

    /// <summary>
    /// <c>[member][step]</c>: one array per member (<c>member_ids</c> order) over the model's <c>valid_time</c>;
    /// <c>null</c> = no value at that step.
    /// </summary>
    [JsonPropertyName("snow_level")]
    public IReadOnlyList<IReadOnlyList<double?>> SnowLevel { get; init; } = [];

    /// <summary>
    /// <c>[member][step]</c>: one array per member (<c>member_ids</c> order) over the model's <c>valid_time</c>;
    /// <c>null</c> = no value at that step.
    /// </summary>
    [JsonPropertyName("snow_level_wet_bulb")]
    public IReadOnlyList<IReadOnlyList<double?>> SnowLevelWetBulb { get; init; } = [];

    /// <summary>
    /// <c>[member][step]</c>: one array per member (<c>member_ids</c> order) over the model's <c>valid_time</c>;
    /// <c>null</c> = no value at that step.
    /// </summary>
    [JsonPropertyName("snowfall")]
    public IReadOnlyList<IReadOnlyList<double?>> Snowfall { get; init; } = [];

    /// <summary>
    /// <c>[member][step]</c>: one array per member (<c>member_ids</c> order) over the model's <c>valid_time</c>;
    /// <c>null</c> = no value at that step.
    /// </summary>
    [JsonPropertyName("cumulative_snowfall")]
    public IReadOnlyList<IReadOnlyList<double?>> CumulativeSnowfall { get; init; } = [];

    /// <summary>
    /// <c>[member][step]</c>: one array per member (<c>member_ids</c> order) over the model's <c>valid_time</c>;
    /// <c>null</c> = no value at that step.
    /// </summary>
    [JsonPropertyName("slr")]
    public IReadOnlyList<IReadOnlyList<double?>> SnowToLiquidRatio { get; init; } = [];

    /// <summary>
    /// The rows (indices into every column) in <c>bands.cumulative_precipitation</c>'s fixed population, ascending:
    /// that band's <c>mean</c> is the mean of these rows' running totals.
    /// </summary>
    [JsonPropertyName("cumulative_precipitation_band_members")]
    public IReadOnlyList<int> CumulativePrecipitationBandMembers { get; init; } = [];

    /// <summary>
    /// Likewise for <c>bands.cumulative_snowfall</c>.
    /// </summary>
    [JsonPropertyName("cumulative_snowfall_band_members")]
    public IReadOnlyList<int> CumulativeSnowfallBandMembers { get; init; } = [];

    /// <summary>
    /// Likewise for <c>bands.cumulative_snowfall_10to1</c>.
    /// </summary>
    [JsonPropertyName("cumulative_snowfall_10to1_band_members")]
    public IReadOnlyList<int> CumulativeSnowfall10To1BandMembers { get; init; } = [];
}
