// SPDX-License-Identifier: BSD-2-Clause

using System;
using System.Text.Json.Serialization;

namespace EnsembleWeather;

/// <summary>
/// "Right now" as one flat object: a view of ONE served hourly point, never a second forecast. <c>time</c> names
/// the hour (the served hour nearest the request time, within one hour of it — the window in which the nowcast
/// blend's time taper is at full weight); every value is copied from that hour, in the request's <c>units</c>:
/// </summary>
public sealed class CurrentConditions
{
    /// <summary>
    /// <c>valid_time</c> of the hour the block is read from.
    /// </summary>
    [JsonPropertyName("time")]
    public DateTimeOffset Time { get; init; }

    /// <summary>
    /// °C (metric), °F (imperial), K (kelvin).
    /// </summary>
    [JsonPropertyName("temperature")]
    public double Temperature { get; init; }

    /// <summary>
    /// Same unit as <c>temperature</c>.
    /// </summary>
    [JsonPropertyName("feels_like")]
    public double? FeelsLike { get; init; }

    /// <summary>
    /// (JSON <c>condition</c>) The hour's stable condition code, as <c>TimeSeriesPoint.condition</c> (<c>partly_cloudy</c>). Treat unknown
    /// codes as additive.
    /// </summary>
    [JsonPropertyName("condition")]
    public string Code { get; init; } = string.Empty;

    /// <summary>
    /// (JSON <c>condition_text</c>) The display text for <c>condition</c>, as <c>TimeSeriesPoint.condition_text</c>.
    /// </summary>
    [JsonPropertyName("condition_text")]
    public string Text { get; init; } = string.Empty;

    /// <summary>
    /// (JSON <c>condition_icon</c>) The condition icon name.
    /// </summary>
    [JsonPropertyName("condition_icon")]
    public ConditionIcon Icon { get; init; }

    /// <summary>
    /// M/s (metric, kelvin) or mph (imperial).
    /// </summary>
    [JsonPropertyName("wind_speed")]
    public double WindSpeed { get; init; }

    /// <summary>
    /// Same unit as <c>wind_speed</c>. Omitted where the hour serves no gust.
    /// </summary>
    [JsonPropertyName("wind_gust")]
    public double? WindGust { get; init; }

    /// <summary>
    /// Degrees clockwise from north, direction the wind blows FROM.
    /// </summary>
    [JsonPropertyName("wind_direction")]
    public double WindDirection { get; init; }

    /// <summary>
    /// Relative humidity, percent (0–100).
    /// </summary>
    [JsonPropertyName("relative_humidity")]
    public double? RelativeHumidity { get; init; }

    /// <summary>
    /// Same unit as <c>temperature</c>.
    /// </summary>
    [JsonPropertyName("dewpoint")]
    public double? Dewpoint { get; init; }

    /// <summary>
    /// Probability (0–1) of at least a 0.1 mm trace in the hour.
    /// </summary>
    [JsonPropertyName("precipitation_probability")]
    public double? PrecipitationProbability { get; init; }

    /// <summary>
    /// <c>nowcast</c> — at least one live observation (surface station, MRMS radar, GOES cloud mask) carried weight
    /// at this hour. <c>model</c> — none did; the hour is the calibrated forecast alone.
    /// </summary>
    [JsonPropertyName("source")]
    public CurrentConditionsSource Source { get; init; }

    /// <summary>
    /// The surface station whose reading anchored temperature, dewpoint and wind at this hour. Omitted when none did.
    /// </summary>
    [JsonPropertyName("station")]
    public ObservingStation? Station { get; init; }

    /// <summary>
    /// The hour in progress's <c>measured</c>, the hour <c>condition</c> and <c>precipitation_probability</c> are
    /// read from: the radar's published accumulation for that clock hour so far is served above 0 (above 0.00 mm
    /// and 0.000 in), so <c>condition</c> states the precipitation falling beside a chance that can be small. Show
    /// "Measured" in place of the chance. Always present. Additive; Cap'n Proto <c>CurrentConditions.measured</c>.
    /// </summary>
    [JsonPropertyName("measured")]
    public bool Measured { get; init; }

    /// <summary>
    /// Cloud cover, percent (0-100), the mean of the hour in progress's <c>ensemble.cloud_cover</c>: the hour
    /// <c>condition</c> is read from, so the number and the label describe one hour. The label is the most likely
    /// sky class of that hour's distribution, so on a wide distribution it can read "Sunny" beside a mean near 50.
    /// Omitted where the hour serves no cloud cover. Additive; Cap'n Proto <c>CurrentConditions.cloudCover</c>.
    /// </summary>
    [JsonPropertyName("cloud_cover")]
    public double? CloudCover { get; init; }
}
