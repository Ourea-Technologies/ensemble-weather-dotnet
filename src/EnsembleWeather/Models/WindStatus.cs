// SPDX-License-Identifier: BSD-2-Clause

using System;
using System.Text.Json.Serialization;

namespace EnsembleWeather;

/// <summary>
/// A day's wind class.
/// </summary>
/// <remarks>
/// An extensible enumeration: a value this SDK version does not know (the API adds values over time) still
/// parses, keeps its raw string in <see cref="Value"/>, and reports <see cref="IsKnown"/> as <see langword="false"/>.
/// Compare with the static members, for example <c>value == WindStatus.Calm</c>.
/// </remarks>
[JsonConverter(typeof(Serialization.WindStatusConverter))]
public readonly struct WindStatus : IEquatable<WindStatus>
{
    private static readonly string[] Known = ["calm", "extreme", "light", "moderate", "strong", "very_strong"];
    private readonly string? _value;

    /// <summary>Initializes a value from its wire string.</summary>
    /// <param name="value">The wire string, for example <c>"calm"</c>.</param>
    /// <exception cref="ArgumentNullException"><paramref name="value"/> is <see langword="null"/>.</exception>
    public WindStatus(string value)
    {
        _value = value ?? throw new ArgumentNullException(nameof(value));
    }

    /// <summary><c>calm</c>.</summary>
    public static WindStatus Calm { get; } = new("calm");

    /// <summary><c>extreme</c>.</summary>
    public static WindStatus Extreme { get; } = new("extreme");

    /// <summary><c>light</c>.</summary>
    public static WindStatus Light { get; } = new("light");

    /// <summary><c>moderate</c>.</summary>
    public static WindStatus Moderate { get; } = new("moderate");

    /// <summary><c>strong</c>.</summary>
    public static WindStatus Strong { get; } = new("strong");

    /// <summary><c>very_strong</c>.</summary>
    public static WindStatus VeryStrong { get; } = new("very_strong");

    /// <summary>Gets the wire string, exactly as the API sent it.</summary>
    public string Value => _value ?? string.Empty;

    /// <summary>Gets whether this SDK version knows the value.</summary>
    public bool IsKnown => Array.IndexOf(Known, Value) >= 0;

    /// <summary>Compares two values by their wire string.</summary>
    /// <param name="left">The first value.</param>
    /// <param name="right">The second value.</param>
    /// <returns><see langword="true"/> when the wire strings are equal.</returns>
    public static bool operator ==(WindStatus left, WindStatus right) => left.Equals(right);

    /// <summary>Compares two values by their wire string.</summary>
    /// <param name="left">The first value.</param>
    /// <param name="right">The second value.</param>
    /// <returns><see langword="true"/> when the wire strings differ.</returns>
    public static bool operator !=(WindStatus left, WindStatus right) => !left.Equals(right);

    /// <inheritdoc/>
    public bool Equals(WindStatus other) => string.Equals(Value, other.Value, StringComparison.Ordinal);

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is WindStatus other && Equals(other);

    /// <inheritdoc/>
    public override int GetHashCode() => StringComparer.Ordinal.GetHashCode(Value);

    /// <summary>Returns the wire string.</summary>
    /// <returns>The wire string.</returns>
    public override string ToString() => Value;
}
