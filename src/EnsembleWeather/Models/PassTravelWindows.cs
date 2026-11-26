// SPDX-License-Identifier: BSD-2-Clause

using System.Text.Json.Serialization;

namespace EnsembleWeather;

/// <summary>
/// The quietest and the busiest stretch of weather at the pass in the next <c>horizon_hours</c>, by one fixed rule
/// on the hours above: an hour has weather when it is snowing (<c>snowfall</c> at least 0.04 in, 1 mm),
/// <c>freezing_on_wet_road</c>, or <c>gust_over_40mph</c>. <c>least_weather</c> is the longest run of hours without
/// weather (earliest on a tie); <c>most_weather</c> the run of hours with weather holding the most snowfall, then
/// the most freezing hours, then the highest gust. A description of the weather, never advice on whether to travel.
/// </summary>
public sealed class PassTravelWindows
{
    /// <summary>
    /// The horizon's length in hours, 24.
    /// </summary>
    [JsonPropertyName("horizon_hours")]
    public int HorizonHours { get; init; }

    /// <summary>
    /// Null when every hour has weather.
    /// </summary>
    [JsonPropertyName("least_weather")]
    public PassTravelWindow? LeastWeather { get; init; }

    /// <summary>
    /// Null when no hour has weather.
    /// </summary>
    [JsonPropertyName("most_weather")]
    public PassTravelWindow? MostWeather { get; init; }
}
