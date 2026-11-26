// SPDX-License-Identifier: BSD-2-Clause

using System;
using System.Text.Json.Serialization;

namespace EnsembleWeather;

/// <summary>
/// One DOT chain-control or closure point, as the DOT publishes it.
/// </summary>
public sealed class PassStatusSegment
{
    /// <summary>
    /// The DOT's own id for the point.
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; init; } = string.Empty;

    /// <summary>
    /// The road, for example <c>I-90</c>.
    /// </summary>
    [JsonPropertyName("route")]
    public string Route { get; init; } = string.Empty;

    /// <summary>
    /// The direction the point controls, as the DOT names it.
    /// </summary>
    [JsonPropertyName("direction")]
    public string Direction { get; init; } = string.Empty;

    /// <summary>
    /// The DOT's name for the point (its trailing field padding removed).
    /// </summary>
    [JsonPropertyName("location")]
    public string Location { get; init; } = string.Empty;

    /// <summary>
    /// (JSON <c>status</c>) The DOT's status code, verbatim.
    /// </summary>
    [JsonPropertyName("status")]
    public string State { get; init; } = string.Empty;

    /// <summary>
    /// The DOT's own restriction, as an enum, for ordering and colour only: the words a client shows are the DOT's
    /// <c>status</c> and <c>status_text</c>. Caltrans's codes map one to one: R-0 (<c>none</c>, "No chain controls
    /// are in effect at this time."), R-1, R-2, R-3 and ROAD CLOSED (<c>closed</c>). WSDOT, CDOT and ODOT publish
    /// words, not codes, read conservatively: a closure is <c>closed</c>; chains required or Colorado's Chain Law
    /// is <c>chains</c>; traction tires required or Colorado's Traction Law is <c>traction</c>; "No restrictions"
    /// is <c>none</c>; any other restriction or advice, or wording not recognised, is <c>advisory</c> (WSDOT's
    /// "Traction Tires Advised, Oversize Vehicles Prohibited."). Levels order least to most restrictive within one
    /// DOT; a pass has one DOT. A client should treat a level it does not know as <c>advisory</c>.
    /// </summary>
    [JsonPropertyName("level")]
    public PassStatusLevel Level { get; init; }

    /// <summary>
    /// (JSON <c>status_text</c>) The DOT's own description of the status, verbatim.
    /// </summary>
    [JsonPropertyName("status_text")]
    public string Text { get; init; } = string.Empty;

    /// <summary>
    /// When the DOT set this status.
    /// </summary>
    [JsonPropertyName("updated_at")]
    public DateTimeOffset UpdatedAt { get; init; }

    /// <summary>
    /// The latitude, decimal degrees.
    /// </summary>
    [JsonPropertyName("lat")]
    public double Latitude { get; init; }

    /// <summary>
    /// The longitude, decimal degrees.
    /// </summary>
    [JsonPropertyName("lon")]
    public double Longitude { get; init; }

    /// <summary>
    /// The DOT's own report of the road surface there, verbatim, where it gives one beside the restriction (WSDOT,
    /// ODOT). Omitted otherwise.
    /// </summary>
    [JsonPropertyName("road_condition")]
    public string? RoadCondition { get; init; }

    /// <summary>
    /// The DOT's own report of the weather there, verbatim (WSDOT, ODOT). Omitted otherwise. Not a forecast of
    /// Ourea's.
    /// </summary>
    [JsonPropertyName("weather_condition")]
    public string? WeatherCondition { get; init; }
}
