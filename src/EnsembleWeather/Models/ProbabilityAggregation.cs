// SPDX-License-Identifier: BSD-2-Clause

using System;
using System.Text.Json.Serialization;

namespace EnsembleWeather;

/// <summary>
/// How a probability term aggregates its window.
/// </summary>
/// <remarks>
/// An extensible enumeration: a value this SDK version does not know (the API adds values over time) still
/// parses, keeps its raw string in <see cref="Value"/>, and reports <see cref="IsKnown"/> as <see langword="false"/>.
/// Compare with the static members, for example <c>value == ProbabilityAggregation.Total</c>.
/// </remarks>
[JsonConverter(typeof(Serialization.ProbabilityAggregationConverter))]
public readonly struct ProbabilityAggregation : IEquatable<ProbabilityAggregation>
{
    private static readonly string[] Known = ["total", "any", "all"];
    private readonly string? _value;

    /// <summary>Initializes a value from its wire string.</summary>
    /// <param name="value">The wire string, for example <c>"total"</c>.</param>
    /// <exception cref="ArgumentNullException"><paramref name="value"/> is <see langword="null"/>.</exception>
    public ProbabilityAggregation(string value)
    {
        _value = value ?? throw new ArgumentNullException(nameof(value));
    }

    /// <summary>The window total (precipitation and snowfall only).</summary>
    public static ProbabilityAggregation Total { get; } = new("total");

    /// <summary>At any hour of the window (the default for temperature, wind speed and snow level).</summary>
    public static ProbabilityAggregation Any { get; } = new("any");

    /// <summary>At every hour of the window.</summary>
    public static ProbabilityAggregation All { get; } = new("all");

    /// <summary>Gets the wire string, exactly as the API sent it.</summary>
    public string Value => _value ?? string.Empty;

    /// <summary>Gets whether this SDK version knows the value.</summary>
    public bool IsKnown => Array.IndexOf(Known, Value) >= 0;

    /// <summary>Compares two values by their wire string.</summary>
    /// <param name="left">The first value.</param>
    /// <param name="right">The second value.</param>
    /// <returns><see langword="true"/> when the wire strings are equal.</returns>
    public static bool operator ==(ProbabilityAggregation left, ProbabilityAggregation right) => left.Equals(right);

    /// <summary>Compares two values by their wire string.</summary>
    /// <param name="left">The first value.</param>
    /// <param name="right">The second value.</param>
    /// <returns><see langword="true"/> when the wire strings differ.</returns>
    public static bool operator !=(ProbabilityAggregation left, ProbabilityAggregation right) => !left.Equals(right);

    /// <inheritdoc/>
    public bool Equals(ProbabilityAggregation other) => string.Equals(Value, other.Value, StringComparison.Ordinal);

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is ProbabilityAggregation other && Equals(other);

    /// <inheritdoc/>
    public override int GetHashCode() => StringComparer.Ordinal.GetHashCode(Value);

    /// <summary>Returns the wire string.</summary>
    /// <returns>The wire string.</returns>
    public override string ToString() => Value;
}
