// SPDX-License-Identifier: BSD-2-Clause

using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace EnsembleWeather;

/// <summary>
/// -- observed season-to-date precipitation at a location.
/// </summary>
public sealed class SeasonResponse : ApiResult
{
    /// <summary>
    /// ONE location reference, the same wherever a place is named -- the forecast's and <c>/current</c>'s
    /// <c>location</c>, every list and <c>/nearby</c> row, search hits and favorites.
    /// </summary>
    [JsonPropertyName("location")]
    public LocationReference Location { get; init; } = null!;

    /// <summary>
    /// The water year by its ending year.
    /// </summary>
    [JsonPropertyName("water_year")]
    public int WaterYear { get; init; }

    /// <summary>
    /// The first local day of the water year.
    /// </summary>
    [JsonPropertyName("starts")]
    [JsonConverter(typeof(Serialization.CalendarDateConverter))]
    public DateTime Starts { get; init; }

    /// <summary>
    /// The last local day with an observed row (the day before <c>starts</c> when there is none).
    /// </summary>
    [JsonPropertyName("through")]
    [JsonConverter(typeof(Serialization.CalendarDateConverter))]
    public DateTime Through { get; init; }

    /// <summary>
    /// When the newest day row was computed; null with no rows.
    /// </summary>
    [JsonPropertyName("as_of")]
    public DateTimeOffset? AsOf { get; init; }

    /// <summary>
    /// Unit system for response values. <c>kelvin</c> returns metric units with Kelvin temperatures.
    /// </summary>
    [JsonPropertyName("units")]
    public UnitSystem Units { get; init; }

    /// <summary>
    /// Null until the location has an observed day.
    /// </summary>
    [JsonPropertyName("precipitation")]
    public SeasonTotal? Precipitation { get; init; }

    /// <summary>
    /// Always null for now; season snowfall is not served yet.
    /// </summary>
    [JsonPropertyName("snowfall")]
    public SeasonTotal? Snowfall { get; init; }

    /// <summary>
    /// The nearest SNOTEL or SCAN gauge within 25 km and 300 m with rows this water year, or null.
    /// </summary>
    [JsonPropertyName("station")]
    public SeasonStation? Station { get; init; }

    /// <summary>
    /// Only with <c>include=daily</c>. One row per observed local day, oldest first.
    /// </summary>
    [JsonPropertyName("daily")]
    public IReadOnlyList<SeasonDay> Daily { get; init; } = [];

    /// <summary>
    /// The data sources THIS response drew on, and what their licences ask us to say. <c>sources</c> names exactly
    /// the contributors: the blend's models that moved a served number (a model read but pooled at zero weight is
    /// absent), the climatology behind the daily normal (<c>era5</c>, <c>snotel</c>), PRISM where a
    /// precipitation-elevation carry was applied, the DEM for a coordinate forecast whose elevation came from it
    /// (<c>usgs_3dep</c>), and the observation networks of the nowcast and the nearest sensors. <c>credit</c> is
    /// one line per provider (ECMWF-derived output says it was modified, as its CC BY 4.0 terms require); carry it
    /// with any redistributed output. The full notices are at <c>url</c>.
    /// </summary>
    [JsonPropertyName("attribution")]
    public ResponseAttribution Attribution { get; init; } = null!;
}
