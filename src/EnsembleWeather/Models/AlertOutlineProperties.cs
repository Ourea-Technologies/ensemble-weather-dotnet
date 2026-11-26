// SPDX-License-Identifier: BSD-2-Clause

using System;
using System.Text.Json.Serialization;

namespace EnsembleWeather;

/// <summary>
/// The alert, under the names <c>ActiveAlert</c> uses for the same fields, and its <c>class</c>. The full text is
/// on the alerts routes. GeoJSON: every key is the wire's, unchanged (BEHAVIOR.md 3.0).
/// </summary>
public sealed class AlertOutlineProperties
{
    /// <summary>
    /// NWS alert identifier (urn:oid:…), as <c>ActiveAlert.id</c>.
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; init; } = string.Empty;

    /// <summary>
    /// The NWS event type, for example <c>Winter Storm Warning</c>.
    /// </summary>
    [JsonPropertyName("event")]
    public string Event { get; init; } = string.Empty;

    /// <summary>
    /// What an alert is about, as far as a map filters on it. Additive: ignore a value you do not know.
    /// </summary>
    [JsonPropertyName("class")]
    public AlertClass Class { get; init; }

    /// <summary>
    /// As <c>ActiveAlert.severity</c>. Additive, treat an unknown value as <c>unknown</c>.
    /// </summary>
    [JsonPropertyName("severity")]
    public string Severity { get; init; } = string.Empty;

    /// <summary>
    /// As <c>ActiveAlert.urgency</c>. One of <c>immediate</c>, <c>expected</c>, <c>future</c>, <c>past</c>,
    /// <c>unknown</c>.
    /// </summary>
    [JsonPropertyName("urgency")]
    public string Urgency { get; init; } = string.Empty;

    /// <summary>
    /// As <c>ActiveAlert.certainty</c>. One of <c>observed</c>, <c>likely</c>, <c>possible</c>, <c>unlikely</c>,
    /// <c>unknown</c>.
    /// </summary>
    [JsonPropertyName("certainty")]
    public string Certainty { get; init; } = string.Empty;

    /// <summary>
    /// The alert's headline.
    /// </summary>
    [JsonPropertyName("headline")]
    public string Headline { get; init; } = string.Empty;

    /// <summary>
    /// When the hazard begins. Omitted when NWS supplies none.
    /// </summary>
    [JsonPropertyName("onset")]
    public DateTimeOffset? Onset { get; init; }

    /// <summary>
    /// When NWS is due to re-issue this message; not the alert's end (see <c>ActiveAlert.expires</c>).
    /// </summary>
    [JsonPropertyName("expires")]
    public DateTimeOffset Expires { get; init; }

    /// <summary>
    /// When the hazard ends. Omitted when NWS supplies none (the alert then runs to <c>expires</c>).
    /// </summary>
    [JsonPropertyName("ends")]
    public DateTimeOffset? Ends { get; init; }

    /// <summary>
    /// The NWS office. Omitted when NWS supplies none.
    /// </summary>
    [JsonPropertyName("sender_name")]
    public string? SenderName { get; init; }

    /// <summary>
    /// The NWS forecast page of a point inside the outline, which lists the alert and links to its text: where
    /// "view on NWS" goes.
    /// </summary>
    [JsonPropertyName("source_url")]
    public string SourceUrl { get; init; } = string.Empty;
}
