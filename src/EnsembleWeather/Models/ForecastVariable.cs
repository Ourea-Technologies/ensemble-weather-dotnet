// SPDX-License-Identifier: BSD-2-Clause

using System;
using System.Text.Json.Serialization;

namespace EnsembleWeather;

/// <summary>
/// A forecast variable a probability term or window total can ask about.
/// </summary>
/// <remarks>
/// An extensible enumeration: a value this SDK version does not know (the API adds values over time) still
/// parses, keeps its raw string in <see cref="Value"/>, and reports <see cref="IsKnown"/> as <see langword="false"/>.
/// Compare with the static members, for example <c>value == ForecastVariable.Temperature</c>.
/// </remarks>
[JsonConverter(typeof(Serialization.ForecastVariableConverter))]
public readonly struct ForecastVariable : IEquatable<ForecastVariable>
{
    private static readonly string[] Known = ["temperature", "wind_speed", "snow_level", "precipitation", "snowfall"];
    private readonly string? _value;

    /// <summary>Initializes a value from its wire string.</summary>
    /// <param name="value">The wire string, for example <c>"temperature"</c>.</param>
    /// <exception cref="ArgumentNullException"><paramref name="value"/> is <see langword="null"/>.</exception>
    public ForecastVariable(string value)
    {
        _value = value ?? throw new ArgumentNullException(nameof(value));
    }

    /// <summary><c>temperature</c>.</summary>
    public static ForecastVariable Temperature { get; } = new("temperature");

    /// <summary><c>wind_speed</c>.</summary>
    public static ForecastVariable WindSpeed { get; } = new("wind_speed");

    /// <summary><c>snow_level</c>.</summary>
    public static ForecastVariable SnowLevel { get; } = new("snow_level");

    /// <summary><c>precipitation</c>.</summary>
    public static ForecastVariable Precipitation { get; } = new("precipitation");

    /// <summary><c>snowfall</c>.</summary>
    public static ForecastVariable Snowfall { get; } = new("snowfall");

    /// <summary>Gets the wire string, exactly as the API sent it.</summary>
    public string Value => _value ?? string.Empty;

    /// <summary>Gets whether this SDK version knows the value.</summary>
    public bool IsKnown => Array.IndexOf(Known, Value) >= 0;

    /// <summary>Compares two values by their wire string.</summary>
    /// <param name="left">The first value.</param>
    /// <param name="right">The second value.</param>
    /// <returns><see langword="true"/> when the wire strings are equal.</returns>
    public static bool operator ==(ForecastVariable left, ForecastVariable right) => left.Equals(right);

    /// <summary>Compares two values by their wire string.</summary>
    /// <param name="left">The first value.</param>
    /// <param name="right">The second value.</param>
    /// <returns><see langword="true"/> when the wire strings differ.</returns>
    public static bool operator !=(ForecastVariable left, ForecastVariable right) => !left.Equals(right);

    /// <inheritdoc/>
    public bool Equals(ForecastVariable other) => string.Equals(Value, other.Value, StringComparison.Ordinal);

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is ForecastVariable other && Equals(other);

    /// <inheritdoc/>
    public override int GetHashCode() => StringComparer.Ordinal.GetHashCode(Value);

    /// <summary>Returns the wire string.</summary>
    /// <returns>The wire string.</returns>
    public override string ToString() => Value;
}
