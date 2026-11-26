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
    /// The one location reference.
    /// </summary>
    [JsonPropertyName("location")]
    public LocationReference Location { get; init; } = null!;

    /// <summary>
    /// (JSON <c>alert_count</c>) The number of active alerts.
    /// </summary>
    [JsonPropertyName("alert_count")]
    public int Count { get; init; }

    /// <summary>
    /// Snapshot timestamp (see /current alerts_as_of).
    /// </summary>
    [JsonPropertyName("as_of")]
    public DateTimeOffset AsOf { get; init; }

    /// <summary>
    /// (JSON <c>alerts</c>) The active alerts.
    /// </summary>
    [JsonPropertyName("alerts")]
    public IReadOnlyList<ActiveAlert> Items { get => field ?? []; init; } = [];

    /// <summary>
    /// The data sources this response drew on and the credit they require.
    /// </summary>
    [JsonPropertyName("attribution")]
    public ResponseAttribution Attribution { get; init; } = null!;
}
