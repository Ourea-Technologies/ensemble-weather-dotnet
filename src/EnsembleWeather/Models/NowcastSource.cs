// SPDX-License-Identifier: BSD-2-Clause

using System;
using System.Text.Json.Serialization;

namespace EnsembleWeather;

/// <summary>
/// A live nowcast source.
/// </summary>
/// <remarks>
/// An extensible enumeration: a value this SDK version does not know (the API adds values over time) still
/// parses, keeps its raw string in <see cref="Value"/>, and reports <see cref="IsKnown"/> as <see langword="false"/>.
/// Compare with the static members, for example <c>value == NowcastSource.Cloud</c>.
/// </remarks>
[JsonConverter(typeof(Serialization.NowcastSourceConverter))]
public readonly struct NowcastSource : IEquatable<NowcastSource>
{
    private static readonly string[] Known = ["cloud", "nearest_sensors", "observation", "radar"];
    private readonly string? _value;

    /// <summary>Initializes a value from its wire string.</summary>
    /// <param name="value">The wire string, for example <c>"cloud"</c>.</param>
    /// <exception cref="ArgumentNullException"><paramref name="value"/> is <see langword="null"/>.</exception>
    public NowcastSource(string value)
    {
        _value = value ?? throw new ArgumentNullException(nameof(value));
    }

    /// <summary><c>cloud</c>.</summary>
    public static NowcastSource Cloud { get; } = new("cloud");

    /// <summary><c>nearest_sensors</c>.</summary>
    public static NowcastSource NearestSensors { get; } = new("nearest_sensors");

    /// <summary><c>observation</c>.</summary>
    public static NowcastSource Observation { get; } = new("observation");

    /// <summary><c>radar</c>.</summary>
    public static NowcastSource Radar { get; } = new("radar");

    /// <summary>Gets the wire string, exactly as the API sent it.</summary>
    public string Value => _value ?? string.Empty;

    /// <summary>Gets whether this SDK version knows the value.</summary>
    public bool IsKnown => Array.IndexOf(Known, Value) >= 0;

    /// <summary>Compares two values by their wire string.</summary>
    /// <param name="left">The first value.</param>
    /// <param name="right">The second value.</param>
    /// <returns><see langword="true"/> when the wire strings are equal.</returns>
    public static bool operator ==(NowcastSource left, NowcastSource right) => left.Equals(right);

    /// <summary>Compares two values by their wire string.</summary>
    /// <param name="left">The first value.</param>
    /// <param name="right">The second value.</param>
    /// <returns><see langword="true"/> when the wire strings differ.</returns>
    public static bool operator !=(NowcastSource left, NowcastSource right) => !left.Equals(right);

    /// <inheritdoc/>
    public bool Equals(NowcastSource other) => string.Equals(Value, other.Value, StringComparison.Ordinal);

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is NowcastSource other && Equals(other);

    /// <inheritdoc/>
    public override int GetHashCode() => StringComparer.Ordinal.GetHashCode(Value);

    /// <summary>Returns the wire string.</summary>
    /// <returns>The wire string.</returns>
    public override string ToString() => Value;
}
