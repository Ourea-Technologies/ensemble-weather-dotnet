// SPDX-License-Identifier: BSD-2-Clause

using System;
using System.Text.Json.Serialization;

namespace EnsembleWeather;

/// <summary>
/// Why a probability or a window total is <see langword="null"/>.
/// </summary>
/// <remarks>
/// An extensible enumeration: a value this SDK version does not know (the API adds values over time) still
/// parses, keeps its raw string in <see cref="Value"/>, and reports <see cref="IsKnown"/> as <see langword="false"/>.
/// Compare with the static members, for example <c>value == NullReason.WindowNotCovered</c>.
/// </remarks>
[JsonConverter(typeof(Serialization.NullReasonConverter))]
public readonly struct NullReason : IEquatable<NullReason>
{
    private static readonly string[] Known = ["window_not_covered", "too_few_members", "variable_unavailable"];
    private readonly string? _value;

    /// <summary>Initializes a value from its wire string.</summary>
    /// <param name="value">The wire string, for example <c>"window_not_covered"</c>.</param>
    /// <exception cref="ArgumentNullException"><paramref name="value"/> is <see langword="null"/>.</exception>
    public NullReason(string value)
    {
        _value = value ?? throw new ArgumentNullException(nameof(value));
    }

    /// <summary>The served series does not reach the window's end (ask a larger <c>MaxHours</c>).</summary>
    public static NullReason WindowNotCovered { get; } = new("window_not_covered");

    /// <summary>Fewer than two members cover the whole window.</summary>
    public static NullReason TooFewMembers { get; } = new("too_few_members");

    /// <summary>No member carries the variable through the window (for example with <c>hourly=false</c>).</summary>
    public static NullReason VariableUnavailable { get; } = new("variable_unavailable");

    /// <summary>Gets the wire string, exactly as the API sent it.</summary>
    public string Value => _value ?? string.Empty;

    /// <summary>Gets whether this SDK version knows the value.</summary>
    public bool IsKnown => Array.IndexOf(Known, Value) >= 0;

    /// <summary>Compares two values by their wire string.</summary>
    /// <param name="left">The first value.</param>
    /// <param name="right">The second value.</param>
    /// <returns><see langword="true"/> when the wire strings are equal.</returns>
    public static bool operator ==(NullReason left, NullReason right) => left.Equals(right);

    /// <summary>Compares two values by their wire string.</summary>
    /// <param name="left">The first value.</param>
    /// <param name="right">The second value.</param>
    /// <returns><see langword="true"/> when the wire strings differ.</returns>
    public static bool operator !=(NullReason left, NullReason right) => !left.Equals(right);

    /// <inheritdoc/>
    public bool Equals(NullReason other) => string.Equals(Value, other.Value, StringComparison.Ordinal);

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is NullReason other && Equals(other);

    /// <inheritdoc/>
    public override int GetHashCode() => StringComparer.Ordinal.GetHashCode(Value);

    /// <summary>Returns the wire string.</summary>
    /// <returns>The wire string.</returns>
    public override string ToString() => Value;
}
