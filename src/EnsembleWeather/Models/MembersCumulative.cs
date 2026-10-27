// SPDX-License-Identifier: BSD-2-Clause

using System.Text.Json.Serialization;

namespace EnsembleWeather;

/// <summary>
/// The distribution of the members' running totals at this hour -- for each quantity, percentiles over every
/// member's <c>cumulative_precipitation</c> / <c>cumulative_snowfall</c> here. Computed from the cumulative
/// distribution, NEVER by summing hourly percentiles (quantiles are not additive: the sum of hourly p90s is far
/// above the p90 of the total). Present at an hour where at least two members carry a running total -- in
/// practice the 6-hourly synoptic hours (00/06/12/18Z), since between them only RRFS's single member has a value;
/// absent at the other hours. A quantity fewer than two members carry is omitted. The members are equally
/// weighted: this is the member spread, not the Ourea forecast -- the headline total is the served
/// <c>snow_outlook</c> (the calibrated, skill-weighted blend's mean; a served running headline total is to come).
/// Clients draw these served values and never compute percentiles or totals themselves.
/// </summary>
public sealed class MembersCumulative
{
    /// <summary>
    /// Precipitation.
    /// </summary>
    [JsonPropertyName("precipitation")]
    public Quantiles? Precipitation { get; init; }

    /// <summary>
    /// Snowfall.
    /// </summary>
    [JsonPropertyName("snowfall")]
    public Quantiles? Snowfall { get; init; }
}
