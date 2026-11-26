// SPDX-License-Identifier: BSD-2-Clause

using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace EnsembleWeather;

/// <summary>
/// Compact active NWS alert (headline carries the user text).
/// </summary>
public sealed class ActiveAlert
{
    /// <summary>
    /// NWS alert identifier (urn:oid:…).
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; init; } = string.Empty;

    /// <summary>
    /// The NWS event type, for example "Winter Storm Warning".
    /// </summary>
    [JsonPropertyName("event")]
    public string Event { get; init; } = string.Empty;

    /// <summary>
    /// The NWS severity, snake_case: <c>extreme</c>, <c>severe</c>, <c>moderate</c>, <c>minor</c> or <c>unknown</c>
    /// (treat any other value as <c>unknown</c>).
    /// </summary>
    [JsonPropertyName("severity")]
    public string Severity { get; init; } = string.Empty;

    /// <summary>
    /// The NWS urgency, snake_case: <c>immediate</c>, <c>expected</c>, <c>future</c>, <c>past</c> or <c>unknown</c>.
    /// </summary>
    [JsonPropertyName("urgency")]
    public string Urgency { get; init; } = string.Empty;

    /// <summary>
    /// The NWS certainty, snake_case: <c>observed</c>, <c>likely</c>, <c>possible</c>, <c>unlikely</c> or
    /// <c>unknown</c>.
    /// </summary>
    [JsonPropertyName("certainty")]
    public string Certainty { get; init; } = string.Empty;

    /// <summary>
    /// The headline text.
    /// </summary>
    [JsonPropertyName("headline")]
    public string Headline { get; init; } = string.Empty;

    /// <summary>
    /// Omitted when NWS supplies none.
    /// </summary>
    [JsonPropertyName("onset")]
    public DateTimeOffset? Onset { get; init; }

    /// <summary>
    /// When the alert expires.
    /// </summary>
    [JsonPropertyName("expires")]
    public DateTimeOffset Expires { get; init; }

    /// <summary>
    /// Omitted when NWS supplies none.
    /// </summary>
    [JsonPropertyName("ends")]
    public DateTimeOffset? Ends { get; init; }

    /// <summary>
    /// When NWS issued this message.
    /// </summary>
    [JsonPropertyName("sent")]
    public DateTimeOffset? Sent { get; init; }

    /// <summary>
    /// The CAP message type, snake_case. Not a sign of a new event: NWS sends messages typed <c>alert</c> that
    /// continue or end an existing one. Use <c>event_key</c>. Additive: treat an unknown value as absent.
    /// </summary>
    [JsonPropertyName("message_type")]
    public string? MessageType { get; init; }

    /// <summary>
    /// The ids of the messages this one replaces. Omitted when there are none.
    /// </summary>
    [JsonPropertyName("references")]
    public IReadOnlyList<string> References { get => field ?? []; init; } = [];

    /// <summary>
    /// The issuing office, as NWS names it.
    /// </summary>
    [JsonPropertyName("sender_name")]
    public string? SenderName { get; init; }

    /// <summary>
    /// The places this message covers, as NWS words them.
    /// </summary>
    [JsonPropertyName("area_desc")]
    public string? AreaDesc { get; init; }

    /// <summary>
    /// The full NWS text, unedited.
    /// </summary>
    [JsonPropertyName("description")]
    public string? Description { get; init; }

    /// <summary>
    /// What NWS says to do, unedited. Omitted when NWS gives none.
    /// </summary>
    [JsonPropertyName("instruction")]
    public string? Instruction { get; init; }

    /// <summary>
    /// The NWS forecast page for this location.
    /// </summary>
    [JsonPropertyName("source_url")]
    public string? SourceUrl { get; init; }

    /// <summary>
    /// The stable identity of this event. Every message in one event has the same key, across NWS Updates and new
    /// <c>id</c>s, so to notify once per event, dedupe on this and nothing else. Treat it as opaque. Omitted only
    /// on a message stored before this field existed.
    /// </summary>
    [JsonPropertyName("event_key")]
    public string? EventKey { get; init; }

    /// <summary>
    /// The VTEC action of this message, lower case. <c>can</c>, <c>exp</c> and <c>upg</c> mean this message ends
    /// the event. Omitted when NWS sent no VTEC. Additive: treat an unknown value as absent.
    /// </summary>
    [JsonPropertyName("vtec_action")]
    public string? VtecAction { get; init; }
}
