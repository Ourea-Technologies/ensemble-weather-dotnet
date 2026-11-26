// SPDX-License-Identifier: BSD-2-Clause

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace EnsembleWeather;

/// <summary>
/// The snowpack forecast of a location's spots.
/// </summary>
public sealed class SnowpackResponse : ApiResult
{
    /// <summary>
    /// Always <c>true</c> while the product is a beta; fields may be added.
    /// </summary>
    [JsonPropertyName("beta")]
    public bool Beta { get; init; }

    /// <summary>
    /// ONE location reference, the same wherever a place is named -- the forecast's and <c>/current</c>'s
    /// <c>location</c>, every list and <c>/nearby</c> row, search hits and favorites.
    /// </summary>
    [JsonPropertyName("location")]
    public LocationReference Location { get; init; } = null!;

    /// <summary>
    /// Unit system for response values. <c>kelvin</c> returns metric units with Kelvin temperatures.
    /// </summary>
    [JsonPropertyName("units")]
    public UnitSystem Units { get; init; }

    /// <summary>
    /// The newest snowpack run among the spots served. Absent when <c>spots</c> is empty.
    /// </summary>
    [JsonPropertyName("model_run")]
    public SnowpackRun? ModelRun { get; init; }

    /// <summary>
    /// The newest run's blend cycle is more than 24 h old.
    /// </summary>
    [JsonPropertyName("stale")]
    public bool Stale { get; init; }

    /// <summary>
    /// The location's own point first, then its named elevations in their display order.
    /// </summary>
    [JsonPropertyName("spots")]
    public IReadOnlyList<SnowpackSpot> Spots { get; init; } = [];

    /// <summary>
    /// What the snowpack codes mean: code -> text.
    /// </summary>
    [JsonPropertyName("definitions")]
    public SnowpackDefinitions Definitions { get; init; } = null!;
}
