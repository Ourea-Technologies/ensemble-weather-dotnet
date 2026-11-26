// SPDX-License-Identifier: BSD-2-Clause

using System;
using System.Text.Json.Serialization;

namespace EnsembleWeather;

/// <summary>
/// A mountain pass's official road-status level, least to most restrictive.
/// </summary>
/// <remarks>
/// An extensible enumeration: a value this SDK version does not know (the API adds values over time) still
/// parses, keeps its raw string in <see cref="Value"/>, and reports <see cref="IsKnown"/> as <see langword="false"/>.
/// Compare with the static members, for example <c>value == PassStatusLevel.Advisory</c>.
/// </remarks>
[JsonConverter(typeof(Serialization.PassStatusLevelConverter))]
public readonly struct PassStatusLevel : IEquatable<PassStatusLevel>
{
    private static readonly string[] Known = ["advisory", "chains", "closed", "none", "r1", "r2", "r3", "traction"];
    private readonly string? _value;

    /// <summary>Initializes a value from its wire string.</summary>
    /// <param name="value">The wire string, for example <c>"advisory"</c>.</param>
    /// <exception cref="ArgumentNullException"><paramref name="value"/> is <see langword="null"/>.</exception>
    public PassStatusLevel(string value)
    {
        _value = value ?? throw new ArgumentNullException(nameof(value));
    }

    /// <summary><c>advisory</c>.</summary>
    public static PassStatusLevel Advisory { get; } = new("advisory");

    /// <summary><c>chains</c>.</summary>
    public static PassStatusLevel Chains { get; } = new("chains");

    /// <summary><c>closed</c>.</summary>
    public static PassStatusLevel Closed { get; } = new("closed");

    /// <summary><c>none</c>.</summary>
    public static PassStatusLevel None { get; } = new("none");

    /// <summary><c>r1</c>.</summary>
    public static PassStatusLevel R1 { get; } = new("r1");

    /// <summary><c>r2</c>.</summary>
    public static PassStatusLevel R2 { get; } = new("r2");

    /// <summary><c>r3</c>.</summary>
    public static PassStatusLevel R3 { get; } = new("r3");

    /// <summary><c>traction</c>.</summary>
    public static PassStatusLevel Traction { get; } = new("traction");

    /// <summary>Gets the wire string, exactly as the API sent it.</summary>
    public string Value => _value ?? string.Empty;

    /// <summary>Gets whether this SDK version knows the value.</summary>
    public bool IsKnown => Array.IndexOf(Known, Value) >= 0;

    /// <summary>Compares two values by their wire string.</summary>
    /// <param name="left">The first value.</param>
    /// <param name="right">The second value.</param>
    /// <returns><see langword="true"/> when the wire strings are equal.</returns>
    public static bool operator ==(PassStatusLevel left, PassStatusLevel right) => left.Equals(right);

    /// <summary>Compares two values by their wire string.</summary>
    /// <param name="left">The first value.</param>
    /// <param name="right">The second value.</param>
    /// <returns><see langword="true"/> when the wire strings differ.</returns>
    public static bool operator !=(PassStatusLevel left, PassStatusLevel right) => !left.Equals(right);

    /// <inheritdoc/>
    public bool Equals(PassStatusLevel other) => string.Equals(Value, other.Value, StringComparison.Ordinal);

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is PassStatusLevel other && Equals(other);

    /// <inheritdoc/>
    public override int GetHashCode() => StringComparer.Ordinal.GetHashCode(Value);

    /// <summary>Returns the wire string.</summary>
    /// <returns>The wire string.</returns>
    public override string ToString() => Value;
}
