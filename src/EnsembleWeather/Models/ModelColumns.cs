// SPDX-License-Identifier: BSD-2-Clause

using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace EnsembleWeather;

/// <summary>
/// One model's members over its NATIVE steps inside the served window (<c>valid_time</c>: the hours at which a
/// member of the model carries a value -- every hour for RRFS, every sixth for the global ensembles; never padded
/// to the hourly series). Each variable is one array per member: <c>temperature[i][j]</c> is member
/// <c>member_ids[i]</c> at <c>valid_time[j]</c>. The values, units and meanings are the row form's
/// <c>MemberForecast</c> fields: <c>precipitation</c> and <c>snowfall</c> are totals over the
/// <c>precipitation_hours</c> ending at the step, and the <c>cumulative_*</c> running totals start where
/// <c>snow_outlook</c> does.
/// </summary>
public sealed class ModelColumns
{
    /// <summary>
    /// The id.
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; init; } = string.Empty;

    /// <summary>
    /// The valid times, one per column.
    /// </summary>
    [JsonPropertyName("valid_time")]
    public IReadOnlyList<DateTimeOffset> ValidTime { get => field ?? []; init; } = [];

    /// <summary>
    /// The members, ascending (<c>MemberForecast.member_id</c>). Rrfs includes its display-only members
    /// (<c>model_run.models[].display_members</c>), whose rows are <c>null</c> past lead 60 and in the columns they
    /// do not carry (<c>snow_level</c>, <c>snowfall</c>, <c>cumulative_snowfall</c>, <c>slr</c>). <c>bands</c> and
    /// <c>headline</c> do not include them.
    /// </summary>
    [JsonPropertyName("member_ids")]
    public IReadOnlyList<int> MemberIds { get => field ?? []; init; } = [];

    /// <summary>
    /// <c>[member][step]</c>: one array per member (<c>member_ids</c> order) over the model's <c>valid_time</c>;
    /// <c>null</c> = no value at that step.
    /// </summary>
    [JsonPropertyName("temperature")]
    public IReadOnlyList<IReadOnlyList<double?>> Temperature { get => field ?? []; init; } = [];

    /// <summary>
    /// <c>[member][step]</c>: one array per member (<c>member_ids</c> order) over the model's <c>valid_time</c>;
    /// <c>null</c> = no value at that step.
    /// </summary>
    [JsonPropertyName("precipitation")]
    public IReadOnlyList<IReadOnlyList<double?>> Precipitation { get => field ?? []; init; } = [];

    /// <summary>
    /// <c>[member][step]</c>: the hours each <c>precipitation</c> value covers; <c>null</c> where it is absent or
    /// its span unknown.
    /// </summary>
    [JsonPropertyName("precipitation_hours")]
    public IReadOnlyList<IReadOnlyList<int?>> PrecipitationHours { get => field ?? []; init; } = [];

    /// <summary>
    /// <c>[member][step]</c>: one array per member (<c>member_ids</c> order) over the model's <c>valid_time</c>;
    /// <c>null</c> = no value at that step.
    /// </summary>
    [JsonPropertyName("cumulative_precipitation")]
    public IReadOnlyList<IReadOnlyList<double?>> CumulativePrecipitation { get => field ?? []; init; } = [];

    /// <summary>
    /// <c>[member][step]</c>: one array per member (<c>member_ids</c> order) over the model's <c>valid_time</c>;
    /// <c>null</c> = no value at that step.
    /// </summary>
    [JsonPropertyName("snow_level")]
    public IReadOnlyList<IReadOnlyList<double?>> SnowLevel { get => field ?? []; init; } = [];

    /// <summary>
    /// <c>[Member][step]</c>, each member's <c>MemberForecast.snow_level_wet_bulb</c> (the snow level on the NWS
    /// scale); <c>null</c> exactly where its <c>snow_level</c> is.
    /// </summary>
    [JsonPropertyName("snow_level_wet_bulb")]
    public IReadOnlyList<IReadOnlyList<double?>> SnowLevelWetBulb { get => field ?? []; init; } = [];

    /// <summary>
    /// <c>[member][step]</c>: one array per member (<c>member_ids</c> order) over the model's <c>valid_time</c>;
    /// <c>null</c> = no value at that step.
    /// </summary>
    [JsonPropertyName("snowfall")]
    public IReadOnlyList<IReadOnlyList<double?>> Snowfall { get => field ?? []; init; } = [];

    /// <summary>
    /// <c>[member][step]</c>: one array per member (<c>member_ids</c> order) over the model's <c>valid_time</c>;
    /// <c>null</c> = no value at that step.
    /// </summary>
    [JsonPropertyName("cumulative_snowfall")]
    public IReadOnlyList<IReadOnlyList<double?>> CumulativeSnowfall { get => field ?? []; init; } = [];

    /// <summary>
    /// One value per <c>valid_time</c> step (not per member): the median of the model's members'
    /// <c>cumulative_snowfall</c> there (Hyndman-Fan type 7, as <c>bands</c>: the middle value, or the midpoint of
    /// the two middle ones; a single-member model's own total), at the amount precision. The model's line on a snow
    /// meteogram. RRFS display members are not in it, so RRFS's is its deterministic run's. <c>null</c> where no
    /// member carries one.
    /// </summary>
    [JsonPropertyName("cumulative_snowfall_p50")]
    public IReadOnlyList<double?> CumulativeSnowfallP50 { get => field ?? []; init; } = [];

    /// <summary>
    /// As <c>cumulative_snowfall_p50</c>, over the members' running snowfall at 10:1 (see
    /// <c>PlumeBands.cumulative_snowfall_10to1</c>).
    /// </summary>
    [JsonPropertyName("cumulative_snowfall_10to1_p50")]
    public IReadOnlyList<double?> CumulativeSnowfall10To1P50 { get => field ?? []; init; } = [];

    /// <summary>
    /// The model's mean lines. Each <c>&lt;column&gt;_mean</c> is one value per <c>valid_time</c> step (not per
    /// member): the equal-weight arithmetic mean of the model's members' <c>&lt;column&gt;</c> values at that step,
    /// over the members carrying one (a single-member model's own value), in the column's units at the column's
    /// precision; <c>null</c> where no member carries one. Beside <c>cumulative_snowfall_p50</c>, the model's line
    /// on a chart. RRFS display members are in none, so RRFS's are its deterministic run's values. Here: of
    /// <c>temperature</c>.
    /// </summary>
    [JsonPropertyName("temperature_mean")]
    public IReadOnlyList<double?> TemperatureMean { get => field ?? []; init; } = [];

    /// <summary>
    /// The mean of <c>precipitation</c> (see <c>temperature_mean</c>). Like the column, a total over the
    /// <c>precipitation_hours</c> ending at the step: six hours for the global ensembles, one for RRFS.
    /// </summary>
    [JsonPropertyName("precipitation_mean")]
    public IReadOnlyList<double?> PrecipitationMean { get => field ?? []; init; } = [];

    /// <summary>
    /// The mean of <c>cumulative_precipitation</c> (see <c>temperature_mean</c>).
    /// </summary>
    [JsonPropertyName("cumulative_precipitation_mean")]
    public IReadOnlyList<double?> CumulativePrecipitationMean { get => field ?? []; init; } = [];

    /// <summary>
    /// The mean of <c>snow_level</c> (see <c>temperature_mean</c>).
    /// </summary>
    [JsonPropertyName("snow_level_mean")]
    public IReadOnlyList<double?> SnowLevelMean { get => field ?? []; init; } = [];

    /// <summary>
    /// The mean of <c>snow_level_wet_bulb</c> (see <c>temperature_mean</c>).
    /// </summary>
    [JsonPropertyName("snow_level_wet_bulb_mean")]
    public IReadOnlyList<double?> SnowLevelWetBulbMean { get => field ?? []; init; } = [];

    /// <summary>
    /// The mean of <c>snowfall</c> (see <c>temperature_mean</c>). Like the column, a total over the
    /// <c>precipitation_hours</c> ending at the step: the 6-hour snowfall for the global ensembles, the hourly one
    /// for RRFS.
    /// </summary>
    [JsonPropertyName("snowfall_mean")]
    public IReadOnlyList<double?> SnowfallMean { get => field ?? []; init; } = [];

    /// <summary>
    /// The mean of <c>cumulative_snowfall</c> (see <c>temperature_mean</c>), beside its median
    /// <c>cumulative_snowfall_p50</c>.
    /// </summary>
    [JsonPropertyName("cumulative_snowfall_mean")]
    public IReadOnlyList<double?> CumulativeSnowfallMean { get => field ?? []; init; } = [];

    /// <summary>
    /// The mean of the members' running snowfall at 10:1, beside <c>cumulative_snowfall_10to1_p50</c> (the members'
    /// 10:1 column itself is not served).
    /// </summary>
    [JsonPropertyName("cumulative_snowfall_10to1_mean")]
    public IReadOnlyList<double?> CumulativeSnowfall10To1Mean { get => field ?? []; init; } = [];

    /// <summary>
    /// The mean of <c>wind_speed</c> (see <c>temperature_mean</c>). Pro (<c>members_view: full</c>) only, as
    /// <c>wind_speed</c> is; absent in the plume view. There is no mean of <c>wind_direction</c> (a direction has
    /// no arithmetic mean) or of <c>slr</c> (a member's ratio means something only where it snows; see
    /// <c>bands.slr</c>).
    /// </summary>
    [JsonPropertyName("wind_speed_mean")]
    public IReadOnlyList<double?> WindSpeedMean { get => field ?? []; init; } = [];

    /// <summary>
    /// Which of this model's <c>member_ids</c> are in <c>bands.cumulative_precipitation</c>, ascending. The
    /// running-total bands are over ONE fixed set of members at every hour they are served (so no rung ever falls
    /// when a model's horizon ends): the members still carrying a running total at the band's last hour. A model
    /// that ends before the band does (RRFS at ~84 h; GEFS at 240 h in a 15-day plume) is in the band at NO hour,
    /// though its members' own running totals are still served and drawn, and its list here is empty. At every hour
    /// the band is served its <c>mean</c> is the equal-weight mean of the listed members' running totals there,
    /// over all models; it is not the mean of every member drawn. RRFS display members are never listed. Every
    /// other band (<c>temperature</c>, <c>snow_level</c>, <c>snow_level_wet_bulb</c>, <c>wind_speed</c>,
    /// <c>precipitation_6h</c>, <c>snowfall_6h</c>, <c>slr</c>) is over the members carrying a value at that hour,
    /// whatever their model.
    /// </summary>
    [JsonPropertyName("cumulative_precipitation_band_member_ids")]
    public IReadOnlyList<int> CumulativePrecipitationBandMemberIds { get => field ?? []; init; } = [];

    /// <summary>
    /// As <c>cumulative_precipitation_band_member_ids</c>, for <c>bands.cumulative_snowfall</c>.
    /// </summary>
    [JsonPropertyName("cumulative_snowfall_band_member_ids")]
    public IReadOnlyList<int> CumulativeSnowfallBandMemberIds { get => field ?? []; init; } = [];

    /// <summary>
    /// As <c>cumulative_precipitation_band_member_ids</c>, for <c>bands.cumulative_snowfall_10to1</c>.
    /// </summary>
    [JsonPropertyName("cumulative_snowfall_10to1_band_member_ids")]
    public IReadOnlyList<int> CumulativeSnowfall10To1BandMemberIds { get => field ?? []; init; } = [];

    /// <summary>
    /// Pro (<c>members_view: full</c>) only; absent in the plume view.
    /// </summary>
    [JsonPropertyName("wind_speed")]
    public IReadOnlyList<IReadOnlyList<double?>> WindSpeed { get => field ?? []; init; } = [];

    /// <summary>
    /// Pro only; absent in the plume view.
    /// </summary>
    [JsonPropertyName("wind_direction")]
    public IReadOnlyList<IReadOnlyList<double?>> WindDirection { get => field ?? []; init; } = [];

    /// <summary>
    /// In both views. As <c>MemberForecast.slr</c>: the ratio from the member's column at every step, whether or
    /// not it snows there.
    /// </summary>
    [JsonPropertyName("slr")]
    public IReadOnlyList<IReadOnlyList<double?>> SnowToLiquidRatio { get => field ?? []; init; } = [];
}
