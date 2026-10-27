// SPDX-License-Identifier: BSD-2-Clause

using System;
using System.Text.Json.Serialization;

namespace EnsembleWeather;

/// <summary>
/// An astronomical season.
/// </summary>
/// <remarks>
/// An extensible enumeration: a value this SDK version does not know (the API adds values over time) still
/// parses, keeps its raw string in <see cref="Value"/>, and reports <see cref="IsKnown"/> as <see langword="false"/>.
/// Compare with the static members, for example <c>value == AstronomicalSeason.Autumn</c>.
/// </remarks>
[JsonConverter(typeof(Serialization.AstronomicalSeasonConverter))]
public readonly struct AstronomicalSeason : IEquatable<AstronomicalSeason>
{
    private static readonly string[] Known = ["autumn", "spring", "summer", "winter"];
    private readonly string? _value;

    /// <summary>Initializes a value from its wire string.</summary>
    /// <param name="value">The wire string, for example <c>"autumn"</c>.</param>
    /// <exception cref="ArgumentNullException"><paramref name="value"/> is <see langword="null"/>.</exception>
    public AstronomicalSeason(string value)
    {
        _value = value ?? throw new ArgumentNullException(nameof(value));
    }

    /// <summary><c>autumn</c>.</summary>
    public static AstronomicalSeason Autumn { get; } = new("autumn");

    /// <summary><c>spring</c>.</summary>
    public static AstronomicalSeason Spring { get; } = new("spring");

    /// <summary><c>summer</c>.</summary>
    public static AstronomicalSeason Summer { get; } = new("summer");

    /// <summary><c>winter</c>.</summary>
    public static AstronomicalSeason Winter { get; } = new("winter");

    /// <summary>Gets the wire string, exactly as the API sent it.</summary>
    public string Value => _value ?? string.Empty;

    /// <summary>Gets whether this SDK version knows the value.</summary>
    public bool IsKnown => Array.IndexOf(Known, Value) >= 0;

    /// <summary>Compares two values by their wire string.</summary>
    /// <param name="left">The first value.</param>
    /// <param name="right">The second value.</param>
    /// <returns><see langword="true"/> when the wire strings are equal.</returns>
    public static bool operator ==(AstronomicalSeason left, AstronomicalSeason right) => left.Equals(right);

    /// <summary>Compares two values by their wire string.</summary>
    /// <param name="left">The first value.</param>
    /// <param name="right">The second value.</param>
    /// <returns><see langword="true"/> when the wire strings differ.</returns>
    public static bool operator !=(AstronomicalSeason left, AstronomicalSeason right) => !left.Equals(right);

    /// <inheritdoc/>
    public bool Equals(AstronomicalSeason other) => string.Equals(Value, other.Value, StringComparison.Ordinal);

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is AstronomicalSeason other && Equals(other);

    /// <inheritdoc/>
    public override int GetHashCode() => StringComparer.Ordinal.GetHashCode(Value);

    /// <summary>Returns the wire string.</summary>
    /// <returns>The wire string.</returns>
    public override string ToString() => Value;
}
