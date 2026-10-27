// SPDX-License-Identifier: BSD-2-Clause

using System;
using System.Text.Json.Serialization;

namespace EnsembleWeather;

/// <summary>
/// A phase of the moon.
/// </summary>
/// <remarks>
/// An extensible enumeration: a value this SDK version does not know (the API adds values over time) still
/// parses, keeps its raw string in <see cref="Value"/>, and reports <see cref="IsKnown"/> as <see langword="false"/>.
/// Compare with the static members, for example <c>value == MoonPhase.FirstQuarter</c>.
/// </remarks>
[JsonConverter(typeof(Serialization.MoonPhaseConverter))]
public readonly struct MoonPhase : IEquatable<MoonPhase>
{
    private static readonly string[] Known = ["first_quarter", "full_moon", "last_quarter", "new_moon", "waning_crescent", "waning_gibbous", "waxing_crescent", "waxing_gibbous"];
    private readonly string? _value;

    /// <summary>Initializes a value from its wire string.</summary>
    /// <param name="value">The wire string, for example <c>"first_quarter"</c>.</param>
    /// <exception cref="ArgumentNullException"><paramref name="value"/> is <see langword="null"/>.</exception>
    public MoonPhase(string value)
    {
        _value = value ?? throw new ArgumentNullException(nameof(value));
    }

    /// <summary><c>first_quarter</c>.</summary>
    public static MoonPhase FirstQuarter { get; } = new("first_quarter");

    /// <summary><c>full_moon</c>.</summary>
    public static MoonPhase FullMoon { get; } = new("full_moon");

    /// <summary><c>last_quarter</c>.</summary>
    public static MoonPhase LastQuarter { get; } = new("last_quarter");

    /// <summary><c>new_moon</c>.</summary>
    public static MoonPhase NewMoon { get; } = new("new_moon");

    /// <summary><c>waning_crescent</c>.</summary>
    public static MoonPhase WaningCrescent { get; } = new("waning_crescent");

    /// <summary><c>waning_gibbous</c>.</summary>
    public static MoonPhase WaningGibbous { get; } = new("waning_gibbous");

    /// <summary><c>waxing_crescent</c>.</summary>
    public static MoonPhase WaxingCrescent { get; } = new("waxing_crescent");

    /// <summary><c>waxing_gibbous</c>.</summary>
    public static MoonPhase WaxingGibbous { get; } = new("waxing_gibbous");

    /// <summary>Gets the wire string, exactly as the API sent it.</summary>
    public string Value => _value ?? string.Empty;

    /// <summary>Gets whether this SDK version knows the value.</summary>
    public bool IsKnown => Array.IndexOf(Known, Value) >= 0;

    /// <summary>Compares two values by their wire string.</summary>
    /// <param name="left">The first value.</param>
    /// <param name="right">The second value.</param>
    /// <returns><see langword="true"/> when the wire strings are equal.</returns>
    public static bool operator ==(MoonPhase left, MoonPhase right) => left.Equals(right);

    /// <summary>Compares two values by their wire string.</summary>
    /// <param name="left">The first value.</param>
    /// <param name="right">The second value.</param>
    /// <returns><see langword="true"/> when the wire strings differ.</returns>
    public static bool operator !=(MoonPhase left, MoonPhase right) => !left.Equals(right);

    /// <inheritdoc/>
    public bool Equals(MoonPhase other) => string.Equals(Value, other.Value, StringComparison.Ordinal);

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is MoonPhase other && Equals(other);

    /// <inheritdoc/>
    public override int GetHashCode() => StringComparer.Ordinal.GetHashCode(Value);

    /// <summary>Returns the wire string.</summary>
    /// <returns>The wire string.</returns>
    public override string ToString() => Value;
}
