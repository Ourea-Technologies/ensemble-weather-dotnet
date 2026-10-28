// SPDX-License-Identifier: BSD-2-Clause

using System;
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
}
