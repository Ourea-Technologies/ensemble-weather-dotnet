// SPDX-License-Identifier: BSD-2-Clause

namespace EnsembleWeather;

/// <summary>The unit a caller states a probability threshold in.</summary>
public enum ThresholdUnit
{
    /// <summary>Millimetres (precipitation, snowfall).</summary>
    Millimeters,

    /// <summary>Inches (precipitation, snowfall); 1 in = 25.4 mm exactly.</summary>
    Inches,

    /// <summary>Degrees Celsius (temperature).</summary>
    Celsius,

    /// <summary>Degrees Fahrenheit (temperature); °F = °C × 9/5 + 32.</summary>
    Fahrenheit,

    /// <summary>Kelvin (temperature); K = °C + 273.15.</summary>
    Kelvin,

    /// <summary>Metres (snow level).</summary>
    Meters,

    /// <summary>Feet (snow level); 1 ft = 0.3048 m exactly.</summary>
    Feet,

    /// <summary>Metres per second (wind speed).</summary>
    MetersPerSecond,

    /// <summary>Miles per hour (wind speed); 1 mph = 0.44704 m/s exactly.</summary>
    MilesPerHour,
}
