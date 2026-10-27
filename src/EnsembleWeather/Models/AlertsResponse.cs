// SPDX-License-Identifier: BSD-2-Clause

using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace EnsembleWeather;

/// <summary>
/// The active NWS alerts for a location or point.
/// </summary>
public sealed class AlertsResponse : ApiResult
{
    /// <summary>
    /// (D-4) -- the one location reference (it was the display name).
    /// </summary>
    [JsonPropertyName("location")]
    public LocationReference Location { get; init; } = null!;

    /// <summary>
    /// The number of active alerts.
    /// </summary>
    [JsonPropertyName("alert_count")]
    public int AlertCount { get; init; }

    /// <summary>
    /// Snapshot timestamp (see /current alerts_as_of).
    /// </summary>
    [JsonPropertyName("as_of")]
    public DateTimeOffset AsOf { get; init; }

    /// <summary>
    /// The active alerts.
    /// </summary>
    [JsonPropertyName("alerts")]
    public IReadOnlyList<ActiveAlert> Alerts { get; init; } = [];

    /// <summary>
    /// The data sources this response drew on and the credit they require.
    /// </summary>
    [JsonPropertyName("attribution")]
    public ResponseAttribution Attribution { get; init; } = null!;
}
