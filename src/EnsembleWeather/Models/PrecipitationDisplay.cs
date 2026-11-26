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

    /// <summary>
    /// The day's mean snowfall, the served <c>total_snowfall.mean</c>, in the request's snow units. Present on
    /// every day that serves snowfall (that mean above 0), whatever its <c>kind</c> and whatever its chance: a day
    /// with <c>kind</c> <c>none</c> (under a 10 % chance of a wet day) still carries the small mean its few wet
    /// members leave, which may round to 0 at the served precision. On a <c>rain_snow_mix</c> day it is the snow
    /// beside the liquid <c>amount</c>, and on a <c>snow</c> day it equals <c>amount</c>. Absent on a day with no
    /// snowfall, never null. Its range is <c>total_snowfall.p10</c> to <c>total_snowfall.p90</c>. That field is
    /// still served; no headline reads it.
    /// </summary>
    [JsonPropertyName("snow_amount")]
    public double? SnowAmount { get; init; }
}
