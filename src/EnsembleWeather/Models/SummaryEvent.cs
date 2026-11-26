// SPDX-License-Identifier: BSD-2-Clause

using System;
using System.Text.Json.Serialization;

namespace EnsembleWeather;

/// <summary>
/// One event the place summary was written from.
/// </summary>
public sealed class SummaryEvent
{
    /// <summary>
    /// What the sentence is about. Clients must ignore a kind they do not know: <c>phase_at_elevation</c> and
    /// <c>change_since_last_run</c> are planned (v1.1). One of <c>alert</c>, <c>precip_starting</c>,
    /// <c>precip_stopping</c>, <c>snow_coming</c>, <c>wind</c>, <c>temperature_change</c>, <c>day_headline</c>,
    /// <c>pass_snow_starts</c>, <c>pass_rain_to_snow</c>, <c>pass_freezing_on_wet_road</c>.
    /// </summary>
    [JsonPropertyName("kind")]
    public string Kind { get; init; } = string.Empty;

    /// <summary>
    /// The phrasing used, <c>&lt;template&gt;.&lt;variant&gt;</c> (<c>snow_coming.v1.3</c>), stable across releases
    /// so sentences can be counted, reviewed and later translated.
    /// </summary>
    [JsonPropertyName("template")]
    public string Template { get; init; } = string.Empty;

    /// <summary>
    /// The hour the event begins (precipitation starting or stopping, an alert's onset, a pass event).
    /// </summary>
    [JsonPropertyName("at")]
    public DateTimeOffset? At { get; init; }

    /// <summary>
    /// When the event ends, where the sentence says so (an alert's end, a snow window's end).
    /// </summary>
    [JsonPropertyName("until")]
    public DateTimeOffset? Until { get; init; }

    /// <summary>
    /// The <c>alerts[].id</c> an <c>alert</c> event states.
    /// </summary>
    [JsonPropertyName("alert_id")]
    public string? AlertId { get; init; }

    /// <summary>
    /// The served numbers the sentence states, unrounded, in the response's units.
    /// </summary>
    [JsonPropertyName("values")]
    public SummaryValues? Values { get; init; }
}
