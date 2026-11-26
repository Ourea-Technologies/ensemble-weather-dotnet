// SPDX-License-Identifier: BSD-2-Clause

using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace EnsembleWeather;

/// <summary>
/// -- each recent run's daily snowfall at a location's point, by source.
/// </summary>
public sealed class SnowTrendResponse : ApiResult
{
    /// <summary>
    /// ONE location reference, the same wherever a place is named -- the forecast's and <c>/current</c>'s
    /// <c>location</c>, every list and <c>/nearby</c> row, search hits and favorites.
    /// </summary>
    [JsonPropertyName("location")]
    public LocationReference Location { get; init; } = null!;

    /// <summary>
    /// Unit system for response values. <c>kelvin</c> returns metric units with Kelvin temperatures.
    /// </summary>
    [JsonPropertyName("units")]
    public UnitSystem Units { get; init; }

    /// <summary>
    /// The point the grids are for.
    /// </summary>
    [JsonPropertyName("elevation")]
    public SnowTrendElevation Elevation { get; init; } = null!;

    /// <summary>
    /// How many days back runs are kept.
    /// </summary>
    [JsonPropertyName("retention_days")]
    public int RetentionDays { get; init; }

    /// <summary>
    /// The columns, location-local dates ascending.
    /// </summary>
    [JsonPropertyName("dates")]
    [JsonConverter(typeof(Serialization.CalendarDateListConverter))]
    public IReadOnlyList<DateTime> Dates { get => field ?? []; init; } = [];

    /// <summary>
    /// <c>ourea</c> first; with the <c>model_data</c> entitlement then <c>aifs</c>, <c>aigefs</c>, <c>gefs</c>,
    /// <c>ifs_ens</c> and <c>rrfs</c>. A client must tolerate a source it does not know.
    /// </summary>
    [JsonPropertyName("grids")]
    public IReadOnlyList<SnowTrendGrid> Grids { get => field ?? []; init; } = [];
}
