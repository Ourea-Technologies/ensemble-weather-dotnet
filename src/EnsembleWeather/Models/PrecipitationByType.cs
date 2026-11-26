// SPDX-License-Identifier: BSD-2-Clause

using System.Text.Json.Serialization;

namespace EnsembleWeather;

/// <summary>
/// This hour's headline precipitation (<c>ensemble.precipitation.mean</c>) split by phase, in the precipitation
/// units -- <c>rain + snow + rain_snow_mix + freezing_rain</c> is the hour's mean (to the served precision), so a
/// stacked bar is the headline bar. The shares come from the members (<c>members=true</c>): each member value votes
/// its precipitation, spread evenly over the hours its <c>precipitation_hours</c> span, as rain (none of it falls
/// as snow), snow (all of it does) or rain/snow mix (part does) -- its share being its served snowfall over
/// precipitation x SLR, the snowfall rule's melting-layer and surface shares, so the split agrees with the members'
/// snowfall. A liquid member whose drops freeze on contact -- fully melted aloft over a sub-freezing surface, the
/// rule <c>precipitation_type</c> calls freezing rain, applied to the member's own column -- votes
/// <c>freezing_rain</c>, not <c>rain</c> (before it was counted in <c>rain</c>), so an hour whose
/// <c>precipitation_type</c> is <c>freezing_rain</c> draws its icing members as freezing rain. Ice pellets are not
/// split out: <c>precipitation_type</c> has no ice-pellet type (a partly melted drop over a frozen surface is
/// <c>rain_snow_mix</c> in both). Served with <c>members=true</c> at every hour some member's precipitation covers;
/// absent where no member's does, and where every covering member is dry under a wet headline (no phase to
/// apportion). A dry hour serves zeros.
/// </summary>
public sealed class PrecipitationByType
{
    /// <summary>
    /// Rain.
    /// </summary>
    [JsonPropertyName("rain")]
    public double Rain { get; init; }

    /// <summary>
    /// The liquid that falls as snow (not snow depth; that is <c>ensemble.snowfall</c>).
    /// </summary>
    [JsonPropertyName("snow")]
    public double Snow { get; init; }

    /// <summary>
    /// Rain and snow mixed.
    /// </summary>
    [JsonPropertyName("rain_snow_mix")]
    public double RainSnowMix { get; init; }

    /// <summary>
    /// -- liquid that freezes on contact (counted in <c>rain</c> before).
    /// </summary>
    [JsonPropertyName("freezing_rain")]
    public double FreezingRain { get; init; }
}
