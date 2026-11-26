// SPDX-License-Identifier: BSD-2-Clause

using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace EnsembleWeather;

/// <summary>
/// The lines a plume draws over its members, at every served hour (<c>valid_time</c>): the blend's
/// <c>temperature</c> and <c>snow_level</c> means (<c>ensemble.*.mean</c>) and its running totals
/// (<c>cumulative_precipitation</c>, <c>cumulative_snowfall</c>: the "Ourea forecast" line), and that snowfall line
/// at a 10:1 snow-to-liquid ratio (<c>cumulative_snowfall_10to1</c>), and its <c>wind_speed</c> mean and its
/// snowfall in each 6 hours (<c>snowfall_6h</c>). <c>null</c> where the hour serves none.
/// </summary>
public sealed class PlumeHeadline
{
    /// <summary>
    /// The valid times, one per column.
    /// </summary>
    [JsonPropertyName("valid_time")]
    public IReadOnlyList<DateTimeOffset> ValidTime { get => field ?? []; init; } = [];

    /// <summary>
    /// Numbers aligned to the enclosing block's <c>valid_time</c>; <c>null</c> = no value there.
    /// </summary>
    [JsonPropertyName("temperature")]
    public IReadOnlyList<double?> Temperature { get => field ?? []; init; } = [];

    /// <summary>
    /// Numbers aligned to the enclosing block's <c>valid_time</c>; <c>null</c> = no value there.
    /// </summary>
    [JsonPropertyName("snow_level")]
    public IReadOnlyList<double?> SnowLevel { get => field ?? []; init; } = [];

    /// <summary>
    /// The blend's snow level on the NWS scale (<c>ensemble.snow_level_wet_bulb.mean</c>, the height of the 0.5 C
    /// wet-bulb); <c>snow_level</c> is the rain line. <c>null</c> exactly where <c>snow_level</c> is.
    /// </summary>
    [JsonPropertyName("snow_level_wet_bulb")]
    public IReadOnlyList<double?> SnowLevelWetBulb { get => field ?? []; init; } = [];

    /// <summary>
    /// Numbers aligned to the enclosing block's <c>valid_time</c>; <c>null</c> = no value there.
    /// </summary>
    [JsonPropertyName("cumulative_precipitation")]
    public IReadOnlyList<double?> CumulativePrecipitation { get => field ?? []; init; } = [];

    /// <summary>
    /// The hourly series' <c>cumulative_snowfall</c>: the running sum of the day cards' snowfall
    /// (<c>daily[].precipitation_display.snow_amount</c>), spread inside each local date by the blend's hourly mean
    /// snowfall, so at the end of each date served whole it equals the running sum of the cards. The card's snow is
    /// the day's mean, so this is the running mean total. The "Ourea forecast" line and its "Total by" readout.
    /// </summary>
    [JsonPropertyName("cumulative_snowfall")]
    public IReadOnlyList<double?> CumulativeSnowfall { get => field ?? []; init; } = [];

    /// <summary>
    /// The headline's running snowfall at a fixed 10:1 snow-to-liquid ratio, in the snowfall units (in / mm), at
    /// the amount precision: each hour's served snowfall with its SLR (<c>ensemble.slr.mean</c>) replaced by 10 --
    /// the liquid that falls as snow, times 10 (0.1 in of it is 1 in; 1 mm is 1 cm) -- summed from the first served
    /// hour over the blend's hourly means. It is not yet on the day cards' statistic, as <c>cumulative_snowfall</c>
    /// is (the cards have no 10:1 amount). Rain stays rain: the phase gates of <c>snowfall</c> all apply, as on the
    /// <c>snowfall_total_10to1</c> map. Beside <c>cumulative_snowfall</c> it is the "Ourea ratio vs 10:1"
    /// comparison. <c>null</c> where the hour serves no snowfall.
    /// </summary>
    [JsonPropertyName("cumulative_snowfall_10to1")]
    public IReadOnlyList<double?> CumulativeSnowfall10To1 { get => field ?? []; init; } = [];

    /// <summary>
    /// The forecast's hourly wind speed, the hourly series' <c>ensemble.wind_speed.mean</c>, in the wind unit (mph
    /// / km/h / m/s as <c>units</c> says) at the wind precision (0.1), at every served hour. It is the forecast's
    /// own wind, not a member's, so it is served in every view, the plume view included (where the members' wind
    /// and <c>bands.wind_speed</c> are withheld).
    /// </summary>
    [JsonPropertyName("wind_speed")]
    public IReadOnlyList<double?> WindSpeed { get => field ?? []; init; } = [];

    /// <summary>
    /// The forecast's snowfall in the 6 hours ending at each 00/06/12/18Z hour, in the snowfall units (in / mm) at
    /// the amount precision: the rise of <c>cumulative_snowfall</c> over <c>(valid_time - 6 h, valid_time]</c>,
    /// computed here so no client differences the line. The 6-hour amounts therefore add up to the running line,
    /// and state the day cards' statistic as it does; they are not the sum of six hourly
    /// <c>ensemble.snowfall.mean</c>. Aligned to <c>valid_time</c> like every headline line and <c>null</c> at
    /// every hour that is not 00, 06, 12 or 18Z; also <c>null</c> where the window would open before the served
    /// window does (the first served hour's window opens an hour before it, as <c>bands.snowfall_6h</c>'s) and
    /// where any hour of the window serves no <c>cumulative_snowfall</c> (the hours of a last local date that has
    /// no day card, which is why the running line can end before <c>bands</c> does: nothing is extrapolated past
    /// it). The same 00/06/12/18Z steps <c>bands.snowfall_6h</c> is served on.
    /// </summary>
    [JsonPropertyName("snowfall_6h")]
    public IReadOnlyList<double?> Snowfall6Hours { get => field ?? []; init; } = [];
}
