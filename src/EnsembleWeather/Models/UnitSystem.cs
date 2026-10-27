// SPDX-License-Identifier: BSD-2-Clause

using System;
using System.Text.Json.Serialization;

namespace EnsembleWeather;

/// <summary>
/// A unit system: <c>metric</c> (°C, mm, m/s, m), <c>imperial</c> (°F, in, mph, ft) or <c>kelvin</c> (metric with temperatures in K).
/// </summary>
/// <remarks>
/// An extensible enumeration: a value this SDK version does not know (the API adds values over time) still
/// parses, keeps its raw string in <see cref="Value"/>, and reports <see cref="IsKnown"/> as <see langword="false"/>.
/// Compare with the static members, for example <c>value == UnitSystem.Metric</c>.
/// </remarks>
[JsonConverter(typeof(Serialization.UnitSystemConverter))]
public readonly struct UnitSystem : IEquatable<UnitSystem>
{
    private static readonly string[] Known = ["metric", "imperial", "kelvin"];
    private readonly string? _value;

    /// <summary>Initializes a value from its wire string.</summary>
    /// <param name="value">The wire string, for example <c>"metric"</c>.</param>
    /// <exception cref="ArgumentNullException"><paramref name="value"/> is <see langword="null"/>.</exception>
    public UnitSystem(string value)
    {
        _value = value ?? throw new ArgumentNullException(nameof(value));
    }

    /// <summary>Metric: °C, mm, m/s, m (the default).</summary>
    public static UnitSystem Metric { get; } = new("metric");

    /// <summary>Imperial: °F, in, mph, ft.</summary>
    public static UnitSystem Imperial { get; } = new("imperial");

    /// <summary>Metric, with temperatures in kelvin.</summary>
    public static UnitSystem Kelvin { get; } = new("kelvin");

    /// <summary>Gets the wire string, exactly as the API sent it.</summary>
    public string Value => _value ?? string.Empty;

    /// <summary>Gets whether this SDK version knows the value.</summary>
    public bool IsKnown => Array.IndexOf(Known, Value) >= 0;

    /// <summary>Compares two values by their wire string.</summary>
    /// <param name="left">The first value.</param>
    /// <param name="right">The second value.</param>
    /// <returns><see langword="true"/> when the wire strings are equal.</returns>
    public static bool operator ==(UnitSystem left, UnitSystem right) => left.Equals(right);

    /// <summary>Compares two values by their wire string.</summary>
    /// <param name="left">The first value.</param>
    /// <param name="right">The second value.</param>
    /// <returns><see langword="true"/> when the wire strings differ.</returns>
    public static bool operator !=(UnitSystem left, UnitSystem right) => !left.Equals(right);

    /// <inheritdoc/>
    public bool Equals(UnitSystem other) => string.Equals(Value, other.Value, StringComparison.Ordinal);

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is UnitSystem other && Equals(other);

    /// <inheritdoc/>
    public override int GetHashCode() => StringComparer.Ordinal.GetHashCode(Value);

    /// <summary>Returns the wire string.</summary>
    /// <returns>The wire string.</returns>
    public override string ToString() => Value;
}
