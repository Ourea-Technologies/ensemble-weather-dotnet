// SPDX-License-Identifier: BSD-2-Clause

using System;
using System.Text.Json.Serialization;

namespace EnsembleWeather;

/// <summary>
/// A precipitation kind.
/// </summary>
/// <remarks>
/// An extensible enumeration: a value this SDK version does not know (the API adds values over time) still
/// parses, keeps its raw string in <see cref="Value"/>, and reports <see cref="IsKnown"/> as <see langword="false"/>.
/// Compare with the static members, for example <c>value == PrecipitationKind.FreezingRain</c>.
/// </remarks>
[JsonConverter(typeof(Serialization.PrecipitationKindConverter))]
public readonly struct PrecipitationKind : IEquatable<PrecipitationKind>
{
    private static readonly string[] Known = ["freezing_rain", "none", "rain", "rain_snow_mix", "snow"];
    private readonly string? _value;

    /// <summary>Initializes a value from its wire string.</summary>
    /// <param name="value">The wire string, for example <c>"freezing_rain"</c>.</param>
    /// <exception cref="ArgumentNullException"><paramref name="value"/> is <see langword="null"/>.</exception>
    public PrecipitationKind(string value)
    {
        _value = value ?? throw new ArgumentNullException(nameof(value));
    }

    /// <summary><c>freezing_rain</c>.</summary>
    public static PrecipitationKind FreezingRain { get; } = new("freezing_rain");

    /// <summary><c>none</c>.</summary>
    public static PrecipitationKind None { get; } = new("none");

    /// <summary><c>rain</c>.</summary>
    public static PrecipitationKind Rain { get; } = new("rain");

    /// <summary><c>rain_snow_mix</c>.</summary>
    public static PrecipitationKind RainSnowMix { get; } = new("rain_snow_mix");

    /// <summary><c>snow</c>.</summary>
    public static PrecipitationKind Snow { get; } = new("snow");

    /// <summary>Gets the wire string, exactly as the API sent it.</summary>
    public string Value => _value ?? string.Empty;

    /// <summary>Gets whether this SDK version knows the value.</summary>
    public bool IsKnown => Array.IndexOf(Known, Value) >= 0;

    /// <summary>Compares two values by their wire string.</summary>
    /// <param name="left">The first value.</param>
    /// <param name="right">The second value.</param>
    /// <returns><see langword="true"/> when the wire strings are equal.</returns>
    public static bool operator ==(PrecipitationKind left, PrecipitationKind right) => left.Equals(right);

    /// <summary>Compares two values by their wire string.</summary>
    /// <param name="left">The first value.</param>
    /// <param name="right">The second value.</param>
    /// <returns><see langword="true"/> when the wire strings differ.</returns>
    public static bool operator !=(PrecipitationKind left, PrecipitationKind right) => !left.Equals(right);

    /// <inheritdoc/>
    public bool Equals(PrecipitationKind other) => string.Equals(Value, other.Value, StringComparison.Ordinal);

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is PrecipitationKind other && Equals(other);

    /// <inheritdoc/>
    public override int GetHashCode() => StringComparer.Ordinal.GetHashCode(Value);

    /// <summary>Returns the wire string.</summary>
    /// <returns>The wire string.</returns>
    public override string ToString() => Value;
}
