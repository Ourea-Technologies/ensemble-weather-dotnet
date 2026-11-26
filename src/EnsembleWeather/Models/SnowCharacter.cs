// SPDX-License-Identifier: BSD-2-Clause

using System;
using System.Text.Json.Serialization;

namespace EnsembleWeather;

/// <summary>
/// The snow's character, in five classes (wetness first, then density): <c>wet</c> (sticky), <c>dense</c> (heavy),
/// <c>average</c>, <c>dry</c> (powder) and <c>very_dry</c> (cold smoke).
/// </summary>
/// <remarks>
/// An extensible enumeration: a value this SDK version does not know (the API adds values over time) still
/// parses, keeps its raw string in <see cref="Value"/>, and reports <see cref="IsKnown"/> as <see langword="false"/>.
/// Compare with the static members, for example <c>value == SnowCharacter.Average</c>.
/// </remarks>
[JsonConverter(typeof(Serialization.SnowCharacterConverter))]
public readonly struct SnowCharacter : IEquatable<SnowCharacter>
{
    private static readonly string[] Known = ["average", "dense", "dry", "very_dry", "wet"];
    private readonly string? _value;

    /// <summary>Initializes a value from its wire string.</summary>
    /// <param name="value">The wire string, for example <c>"average"</c>.</param>
    /// <exception cref="ArgumentNullException"><paramref name="value"/> is <see langword="null"/>.</exception>
    public SnowCharacter(string value)
    {
        _value = value ?? throw new ArgumentNullException(nameof(value));
    }

    /// <summary><c>average</c>.</summary>
    public static SnowCharacter Average { get; } = new("average");

    /// <summary><c>dense</c>.</summary>
    public static SnowCharacter Dense { get; } = new("dense");

    /// <summary><c>dry</c>.</summary>
    public static SnowCharacter Dry { get; } = new("dry");

    /// <summary><c>very_dry</c>.</summary>
    public static SnowCharacter VeryDry { get; } = new("very_dry");

    /// <summary><c>wet</c>.</summary>
    public static SnowCharacter Wet { get; } = new("wet");

    /// <summary>Gets the wire string, exactly as the API sent it.</summary>
    public string Value => _value ?? string.Empty;

    /// <summary>Gets whether this SDK version knows the value.</summary>
    public bool IsKnown => Array.IndexOf(Known, Value) >= 0;

    /// <summary>Compares two values by their wire string.</summary>
    /// <param name="left">The first value.</param>
    /// <param name="right">The second value.</param>
    /// <returns><see langword="true"/> when the wire strings are equal.</returns>
    public static bool operator ==(SnowCharacter left, SnowCharacter right) => left.Equals(right);

    /// <summary>Compares two values by their wire string.</summary>
    /// <param name="left">The first value.</param>
    /// <param name="right">The second value.</param>
    /// <returns><see langword="true"/> when the wire strings differ.</returns>
    public static bool operator !=(SnowCharacter left, SnowCharacter right) => !left.Equals(right);

    /// <inheritdoc/>
    public bool Equals(SnowCharacter other) => string.Equals(Value, other.Value, StringComparison.Ordinal);

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is SnowCharacter other && Equals(other);

    /// <inheritdoc/>
    public override int GetHashCode() => StringComparer.Ordinal.GetHashCode(Value);

    /// <summary>Returns the wire string.</summary>
    /// <returns>The wire string.</returns>
    public override string ToString() => Value;
}
