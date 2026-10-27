// SPDX-License-Identifier: BSD-2-Clause

using System.Text.Json.Serialization;

namespace EnsembleWeather;

/// <summary>
/// Served as a daily row's <c>temperature_climatology</c> (it was <c>climatology</c>, which did not name its
/// variable). Seasonal temperature normal for this day at the forecast point (all-hours mean and standard
/// deviation over a ±15-day window around the date), resolved the way verification resolves a station's
/// reference: the nearest climatology station's own normal when one lies within 60 km, else the ERA5 0.25° cell
/// the point lies in. Units follow the request's <c>units</c> parameter (°C metric, °F imperial). Omitted only
/// outside the climatology grid or before the normals are loaded. <c>std</c> includes the diurnal range, so it is
/// a wide, climate-scale spread — useful for bounding a forecast against the local climate, not for tight day-to-
/// day comparison.
/// </summary>
public sealed class DayClimatology
{
    /// <summary>
    /// All-hours seasonal mean temperature.
    /// </summary>
    [JsonPropertyName("mean")]
    public double Mean { get; init; }

    /// <summary>
    /// Seasonal standard deviation (includes the diurnal range).
    /// </summary>
    [JsonPropertyName("std")]
    public double StandardDeviation { get; init; }
}
