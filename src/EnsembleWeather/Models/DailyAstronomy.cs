// SPDX-License-Identifier: BSD-2-Clause

using System;
using System.Text.Json.Serialization;

namespace EnsembleWeather;

/// <summary>
/// The day's sun and moon, grouped (they were flat keys on the daily row, beside a zodiac sign that is no longer
/// served).
/// </summary>
public sealed class DailyAstronomy
{
    /// <summary>
    /// Sunrise time at the forecast location, ISO-8601 UTC. Computed with the NOAA Solar Calculator algorithm
    /// including atmospheric refraction (-34′), solar semi-diameter (-16′), and elevation-induced horizon dip.
    /// Accurate to within ~1 minute of USNO reference values.
    /// </summary>
    [JsonPropertyName("sunrise")]
    public DateTimeOffset? Sunrise { get; init; }

    /// <summary>
    /// Sunset time at the forecast location, ISO-8601 UTC. For western- hemisphere locations in late-afternoon UTC,
    /// this often falls on the UTC calendar day *after* the local forecast date.
    /// </summary>
    [JsonPropertyName("sunset")]
    public DateTimeOffset? Sunset { get; init; }

    /// <summary>
    /// Eight-way moon phase for the date.
    /// </summary>
    [JsonPropertyName("moon_phase")]
    public MoonPhase MoonPhase { get; init; }

    /// <summary>
    /// Fraction of the moon's disc that's illuminated (0.0 = new, 1.0 = full). More precise than the named phase —
    /// useful if the UI wants to render a crescent icon scaled to the exact illumination.
    /// </summary>
    [JsonPropertyName("moon_illumination")]
    public double MoonIllumination { get; init; }

    /// <summary>
    /// Astronomical season by equinox/solstice (not meteorological). Hemisphere-aware — a Southern-hemisphere
    /// location returns <c>summer</c> in January.
    /// </summary>
    [JsonPropertyName("astronomical_season")]
    public AstronomicalSeason AstronomicalSeason { get; init; }
}
