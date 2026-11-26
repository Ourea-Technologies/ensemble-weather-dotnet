// SPDX-License-Identifier: BSD-2-Clause

using System.Text.Json.Serialization;

namespace EnsembleWeather;

/// <summary>
/// The day's climate normal at the forecast point: the mean, over the day of year +/-15 days of every year of the
/// station's record, of the very quantities this row states -- the local CALENDAR date's maximum hourly temperature
/// (<c>high</c>, the quantity <c>high</c> is), its minimum (<c>low</c>) and its precipitation TOTAL
/// (<c>precipitation</c>, dry days included; a daily total, never an hourly amount). Units follow the request's
/// <c>units</c> (°C / mm metric, °F / in imperial), converted as the row's own values are; <c>distance_km</c> is
/// always km. Where it comes from. The nearest climatology station within 60 km of the point and within 300 m of
/// its elevation (a station far above or below the point describes another climate, and no lapse rate carries a
/// normal across a valley inversion) that holds calendar-day normals (<c>source: station</c>, today a SNOTEL
/// station's own hourly record, at least ten years of complete local days); else the ERA5 0.25° cell the point lies
/// in (<c>source: era5_cell</c>). The ERA5 normals are sampled at four hours a day and state no calendar-day
/// maximum, minimum or total, so no cell answers today. No approximation: where neither has a normal the whole
/// object is absent, and an absent member (<c>precipitation</c> in a window too dry to fit) is absent rather than
/// estimated. The source is credited in <c>attribution</c>.
/// </summary>
public sealed class DayNormal
{
    /// <summary>
    /// Mean local-calendar-day maximum temperature.
    /// </summary>
    [JsonPropertyName("high")]
    public double? High { get; init; }

    /// <summary>
    /// Mean local-calendar-day minimum temperature.
    /// </summary>
    [JsonPropertyName("low")]
    public double? Low { get; init; }

    /// <summary>
    /// Mean local-calendar-day precipitation total, dry days included.
    /// </summary>
    [JsonPropertyName("precipitation")]
    public double? Precipitation { get; init; }

    /// <summary>
    /// A climatology station's own record, or the ERA5 cell of the point. One of <c>station</c>, <c>era5_cell</c>.
    /// </summary>
    [JsonPropertyName("source")]
    public string Source { get; init; } = string.Empty;

    /// <summary>
    /// Great-circle distance from the point to the station (or the cell centre), km.
    /// </summary>
    [JsonPropertyName("distance_km")]
    public double DistanceKm { get; init; }
}
