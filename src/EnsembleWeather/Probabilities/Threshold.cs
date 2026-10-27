// SPDX-License-Identifier: BSD-2-Clause

using System;
using System.Globalization;

namespace EnsembleWeather;

/// <summary>
/// A probability term's threshold: a number, optionally with the unit it is stated in.
/// </summary>
/// <remarks>
/// A threshold with a unit (for example <c>Threshold.Inches(6)</c>) is converted to the request's unit system before
/// it is sent, with exact factors, then rounded to the variable's served precision exactly as the server rounds
/// (half away from zero). Converting the caller's input is the only arithmetic the SDK does: it never converts a
/// response value. A threshold without a unit (<see cref="InRequestUnits"/>) is taken as already in the request's
/// units.
/// </remarks>
public readonly struct Threshold : IEquatable<Threshold>
{
    /// <summary>Initializes a threshold.</summary>
    /// <param name="value">The number.</param>
    /// <param name="unit">The unit it is stated in, or <see langword="null"/> for the request's units.</param>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="value"/> is not finite.</exception>
    public Threshold(double value, ThresholdUnit? unit)
    {
        if (double.IsNaN(value) || double.IsInfinity(value))
        {
            throw new ArgumentOutOfRangeException(nameof(value), "A threshold must be a finite number.");
        }

        Value = value;
        Unit = unit;
    }

    /// <summary>Gets the number, in <see cref="Unit"/>.</summary>
    public double Value { get; }

    /// <summary>Gets the unit, or <see langword="null"/> when the value is in the request's units.</summary>
    public ThresholdUnit? Unit { get; }

    /// <summary>A threshold in the request's own units (no conversion).</summary>
    /// <param name="value">The number.</param>
    /// <returns>The threshold.</returns>
    public static Threshold InRequestUnits(double value) => new(value, null);

    /// <summary>A precipitation or snowfall threshold in millimetres.</summary>
    /// <param name="value">The amount.</param>
    /// <returns>The threshold.</returns>
    public static Threshold Millimeters(double value) => new(value, ThresholdUnit.Millimeters);

    /// <summary>A precipitation or snowfall threshold in centimetres (stated as millimetres × 10).</summary>
    /// <param name="value">The amount.</param>
    /// <returns>The threshold.</returns>
    public static Threshold Centimeters(double value) => new(value * 10, ThresholdUnit.Millimeters);

    /// <summary>A precipitation or snowfall threshold in inches.</summary>
    /// <param name="value">The amount.</param>
    /// <returns>The threshold.</returns>
    public static Threshold Inches(double value) => new(value, ThresholdUnit.Inches);

    /// <summary>A temperature threshold in degrees Celsius.</summary>
    /// <param name="value">The temperature.</param>
    /// <returns>The threshold.</returns>
    public static Threshold Celsius(double value) => new(value, ThresholdUnit.Celsius);

    /// <summary>A temperature threshold in degrees Fahrenheit.</summary>
    /// <param name="value">The temperature.</param>
    /// <returns>The threshold.</returns>
    public static Threshold Fahrenheit(double value) => new(value, ThresholdUnit.Fahrenheit);

    /// <summary>A temperature threshold in kelvin.</summary>
    /// <param name="value">The temperature.</param>
    /// <returns>The threshold.</returns>
    public static Threshold Kelvin(double value) => new(value, ThresholdUnit.Kelvin);

    /// <summary>A snow-level threshold in metres.</summary>
    /// <param name="value">The height.</param>
    /// <returns>The threshold.</returns>
    public static Threshold Meters(double value) => new(value, ThresholdUnit.Meters);

    /// <summary>A snow-level threshold in feet.</summary>
    /// <param name="value">The height.</param>
    /// <returns>The threshold.</returns>
    public static Threshold Feet(double value) => new(value, ThresholdUnit.Feet);

    /// <summary>A wind-speed threshold in metres per second.</summary>
    /// <param name="value">The speed.</param>
    /// <returns>The threshold.</returns>
    public static Threshold MetersPerSecond(double value) => new(value, ThresholdUnit.MetersPerSecond);

    /// <summary>A wind-speed threshold in miles per hour.</summary>
    /// <param name="value">The speed.</param>
    /// <returns>The threshold.</returns>
    public static Threshold MilesPerHour(double value) => new(value, ThresholdUnit.MilesPerHour);

    /// <summary>Compares two thresholds.</summary>
    /// <param name="left">The first threshold.</param>
    /// <param name="right">The second threshold.</param>
    /// <returns><see langword="true"/> when equal.</returns>
    public static bool operator ==(Threshold left, Threshold right) => left.Equals(right);

    /// <summary>Compares two thresholds.</summary>
    /// <param name="left">The first threshold.</param>
    /// <param name="right">The second threshold.</param>
    /// <returns><see langword="true"/> when different.</returns>
    public static bool operator !=(Threshold left, Threshold right) => !left.Equals(right);

    /// <inheritdoc/>
    public bool Equals(Threshold other) => Value.Equals(other.Value) && Unit == other.Unit;

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is Threshold other && Equals(other);

    /// <inheritdoc/>
    public override int GetHashCode() => (Value, Unit).GetHashCode();

    /// <summary>Formats the threshold, for example <c>6 Inches</c>.</summary>
    /// <returns>The text.</returns>
    public override string ToString()
        => Value.ToString(CultureInfo.InvariantCulture) + (Unit is ThresholdUnit u ? " " + u : string.Empty);
}
