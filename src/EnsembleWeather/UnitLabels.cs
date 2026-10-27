// SPDX-License-Identifier: BSD-2-Clause

namespace EnsembleWeather;

/// <summary>
/// The unit of each kind of quantity in a response, for its unit system. Values are never converted client-side:
/// to get other units, ask the server for them (<see cref="ForecastOptions.Units"/>).
/// </summary>
public sealed class UnitLabels
{
    private static readonly UnitLabels MetricLabels = new("°C", "mm", "m/s", "m");
    private static readonly UnitLabels ImperialLabels = new("°F", "in", "mph", "ft");
    private static readonly UnitLabels KelvinLabels = new("K", "mm", "m/s", "m");

    private UnitLabels(string temperature, string amount, string speed, string length)
    {
        Temperature = temperature;
        Amount = amount;
        Speed = speed;
        Length = length;
    }

    /// <summary>Gets the temperature unit (temperature, dewpoint, feels like): °C, °F or K.</summary>
    public string Temperature { get; }

    /// <summary>Gets the amount unit (precipitation, snowfall): mm or in.</summary>
    public string Amount { get; }

    /// <summary>Gets the speed unit (wind speed, gusts): m/s or mph.</summary>
    public string Speed { get; }

    /// <summary>Gets the length unit (snow level, elevations): m or ft.</summary>
    public string Length { get; }

    /// <summary>Gets the labels for a unit system.</summary>
    /// <param name="units">The unit system.</param>
    /// <returns>The labels (metric's for a unit system this SDK does not know).</returns>
    public static UnitLabels For(UnitSystem units)
        => units == UnitSystem.Imperial ? ImperialLabels : units == UnitSystem.Kelvin ? KelvinLabels : MetricLabels;
}
