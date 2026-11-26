// SPDX-License-Identifier: BSD-2-Clause

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace EnsembleWeather;

/// <summary>
/// A short plain-language summary of what is coming at a place, written by the server from templates filled with
/// served values (never free text from a model). Every number in <c>short</c> and <c>long</c> is one of the served
/// values in <c>events[].values</c>, rounded for reading (snow in whole inches at 1 inch and above, one decimal
/// below; whole centimetres in metric). Snow is always a window's headline total (the forecast's
/// <c>snow_outlook.next_*</c>, the sum of the hourly means) with its served p10-p90 range, written "up to N" when
/// p10 is under a trace. Likelihood words: "likely" at a served chance of 60% or more, "possible" at 30-59%. Times
/// are to the hour in the place's own zone (<c>location.timezone</c>). The wording of one place is fixed for the
/// whole run: a stable hash of (place, elevation, run, event kind) picks among phrasings that state the same facts,
/// so the favorites row, the page, the web and the apps show the same text, and it changes when a new run arrives.
/// </summary>
public sealed class PlaceSummary
{
    /// <summary>
    /// One sentence: the most important thing at the place, by priority (an NWS alert, precipitation starting or
    /// stopping soon, snow coming, wind or a large temperature change, then the day's headline). The favorites row,
    /// widgets and notification previews show it.
    /// </summary>
    [JsonPropertyName("short")]
    [SuppressMessage("Naming", "CA1720:Identifier contains type name", Justification = "The wire name (short, as opposed to long); renamed only to avoid a stutter.")]
    public string Short { get; init; } = string.Empty;

    /// <summary>
    /// One to three sentences for the location page, each from a different kind of event. <c>long[0]</c> is always
    /// <c>short</c>.
    /// </summary>
    [JsonPropertyName("long")]
    [SuppressMessage("Naming", "CA1720:Identifier contains type name", Justification = "The wire name (long, as opposed to short); renamed only to avoid a stutter.")]
    public IReadOnlyList<string> Long { get; init; } = [];

    /// <summary>
    /// The event behind each sentence of <c>long</c>, in the same order (<c>events[i]</c> wrote <c>long[i]</c>), so
    /// a client can style a sentence (an alert chip, a snow icon) without parsing text.
    /// </summary>
    [JsonPropertyName("events")]
    public IReadOnlyList<SummaryEvent> Events { get; init; } = [];

    /// <summary>
    /// The served elevation the sentences describe: its display name where the catalog has one, else its name.
    /// Omitted for a place with no named elevations.
    /// </summary>
    [JsonPropertyName("elevation_name")]
    public string? ElevationName { get; init; }

    /// <summary>
    /// The height the sentences describe, in the response's length units (m, ft).
    /// </summary>
    [JsonPropertyName("elevation")]
    public double Elevation { get; init; }

    /// <summary>
    /// The init of the forecast run the sentences were written from.
    /// </summary>
    [JsonPropertyName("run")]
    public DateTimeOffset Run { get; init; }

    /// <summary>
    /// When the sentences were written (the request's now); the "updated" stamp.
    /// </summary>
    [JsonPropertyName("generated_at")]
    public DateTimeOffset GeneratedAt { get; init; }
}
