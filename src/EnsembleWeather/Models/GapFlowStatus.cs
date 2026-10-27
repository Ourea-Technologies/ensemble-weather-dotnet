// SPDX-License-Identifier: BSD-2-Clause

using System;
using System.Text.Json.Serialization;

namespace EnsembleWeather;

/// <summary>
/// The strength of Columbia Gorge gap flow.
/// </summary>
/// <remarks>
/// An extensible enumeration: a value this SDK version does not know (the API adds values over time) still
/// parses, keeps its raw string in <see cref="Value"/>, and reports <see cref="IsKnown"/> as <see langword="false"/>.
/// Compare with the static members, for example <c>value == GapFlowStatus.Extreme</c>.
/// </remarks>
[JsonConverter(typeof(Serialization.GapFlowStatusConverter))]
public readonly struct GapFlowStatus : IEquatable<GapFlowStatus>
{
    private static readonly string[] Known = ["extreme", "moderate", "none", "strong"];
    private readonly string? _value;

    /// <summary>Initializes a value from its wire string.</summary>
    /// <param name="value">The wire string, for example <c>"extreme"</c>.</param>
    /// <exception cref="ArgumentNullException"><paramref name="value"/> is <see langword="null"/>.</exception>
    public GapFlowStatus(string value)
    {
        _value = value ?? throw new ArgumentNullException(nameof(value));
    }

    /// <summary><c>extreme</c>.</summary>
    public static GapFlowStatus Extreme { get; } = new("extreme");

    /// <summary><c>moderate</c>.</summary>
    public static GapFlowStatus Moderate { get; } = new("moderate");

    /// <summary><c>none</c>.</summary>
    public static GapFlowStatus None { get; } = new("none");

    /// <summary><c>strong</c>.</summary>
    public static GapFlowStatus Strong { get; } = new("strong");

    /// <summary>Gets the wire string, exactly as the API sent it.</summary>
    public string Value => _value ?? string.Empty;

    /// <summary>Gets whether this SDK version knows the value.</summary>
    public bool IsKnown => Array.IndexOf(Known, Value) >= 0;

    /// <summary>Compares two values by their wire string.</summary>
    /// <param name="left">The first value.</param>
    /// <param name="right">The second value.</param>
    /// <returns><see langword="true"/> when the wire strings are equal.</returns>
    public static bool operator ==(GapFlowStatus left, GapFlowStatus right) => left.Equals(right);

    /// <summary>Compares two values by their wire string.</summary>
    /// <param name="left">The first value.</param>
    /// <param name="right">The second value.</param>
    /// <returns><see langword="true"/> when the wire strings differ.</returns>
    public static bool operator !=(GapFlowStatus left, GapFlowStatus right) => !left.Equals(right);

    /// <inheritdoc/>
    public bool Equals(GapFlowStatus other) => string.Equals(Value, other.Value, StringComparison.Ordinal);

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is GapFlowStatus other && Equals(other);

    /// <inheritdoc/>
    public override int GetHashCode() => StringComparer.Ordinal.GetHashCode(Value);

    /// <summary>Returns the wire string.</summary>
    /// <returns>The wire string.</returns>
    public override string ToString() => Value;
}
