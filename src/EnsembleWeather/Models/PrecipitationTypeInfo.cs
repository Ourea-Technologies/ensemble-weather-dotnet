// SPDX-License-Identifier: BSD-2-Clause

using System.Text.Json.Serialization;

namespace EnsembleWeather;

/// <summary>
/// Predicted precipitation type derived from the mean snow level, station elevation, and the hour's served
/// (corrected) surface temperature and dewpoint, on the same rule, and the same temperature, the served
/// <c>snowfall</c> accumulates at. The snow level is the bottom of the melting layer, where falling snow has
/// fully melted; the rain/snow mix lies ABOVE it, from the snow level up to the all-snow height about 127 m
/// higher. Absent when the hour has no snow level: the phase cannot be placed.
/// </summary>
public sealed class PrecipitationTypeInfo
{
    /// <summary>
    /// The categorical reading of <c>snow_probability</c>: <c>snow</c> only at 1 (and at most 2 °C),
    /// <c>rain_snow_mix</c> strictly between 0 and 1, <c>rain</c> or <c>freezing_rain</c> at 0. <c>freezing_rain</c>
    /// is fully melted precipitation (the location at or below the snow level) falling onto a surface below 0 °C. The
    /// hourly <c>condition</c> states this type's phase: a Snow, Rain/Snow Mix or Freezing Rain label, or a liquid
    /// label for <c>rain</c>.
    /// </summary>
    [JsonPropertyName("type")]
    public PrecipitationKind Type { get; init; }

    /// <summary>
    /// Share of the hour's precipitation that falls as snow (0-1): the melting layer's snow fraction at the location
    /// (0 at and below the snow level, 1 from about 127 m above it, linear in melting energy between) times the
    /// surface snow share — all snow at or below 2 °C air temperature, none at or above 4 °C, and the surface wet-
    /// bulb fade (none at or above 2 °C wet-bulb). It is the share the served <c>snowfall</c> accumulates at, so a
    /// warm rain hour has 0 and an hour inside the mixed band carries its fraction. Conditional on precipitation; for
    /// the chance that snow accumulates at all, use <c>ensemble.snowfall.probability</c>.
    /// </summary>
    [JsonPropertyName("snow_probability")]
    public double SnowProbability { get; init; }

    /// <summary>
    /// <c>1 - snow_probability</c> (0-1).
    /// </summary>
    [JsonPropertyName("rain_probability")]
    public double RainProbability { get; init; }
}
