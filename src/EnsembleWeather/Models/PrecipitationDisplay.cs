// SPDX-License-Identifier: BSD-2-Clause

using System.Text.Json.Serialization;

namespace EnsembleWeather;

/// <summary>
/// A day's precipitation headline, chosen by the server so every client says the same thing: <c>kind</c>
/// <c>none</c> when the chance of a wet day (<c>precipitation_occurrence_probability</c>) is under 15 %, else the day's
/// <c>precipitation_type</c>; <c>chance</c> that chance; <c>amount</c> the median amount if wet
/// (<c>total_snowfall_if_wet.p50</c> for snow, <c>total_precipitation_if_wet.p50</c> otherwise).
/// </summary>
public sealed class PrecipitationDisplay
{
    /// <summary>
    /// The kind.
    /// </summary>
    [JsonPropertyName("kind")]
    public PrecipitationKind Kind { get; init; }

    /// <summary>
    /// The chance of a wet day, 0..1.
    /// </summary>
    [JsonPropertyName("chance")]
    public double Chance { get; init; }

    /// <summary>
    /// The median amount if wet, in the request's units.
    /// </summary>
    [JsonPropertyName("amount")]
    public double? Amount { get; init; }
}
