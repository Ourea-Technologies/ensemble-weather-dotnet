// SPDX-License-Identifier: BSD-2-Clause

using System;
using System.Text.Json.Serialization;

namespace EnsembleWeather;

/// <summary>
/// The kind of a data source.
/// </summary>
/// <remarks>
/// An extensible enumeration: a value this SDK version does not know (the API adds values over time) still
/// parses, keeps its raw string in <see cref="Value"/>, and reports <see cref="IsKnown"/> as <see langword="false"/>.
/// Compare with the static members, for example <c>value == AttributionSourceKind.Alerts</c>.
/// </remarks>
[JsonConverter(typeof(Serialization.AttributionSourceKindConverter))]
public readonly struct AttributionSourceKind : IEquatable<AttributionSourceKind>
{
    private static readonly string[] Known = ["alerts", "basemap", "climatology", "model", "observation", "peer", "places", "terrain"];
    private readonly string? _value;

    /// <summary>Initializes a value from its wire string.</summary>
    /// <param name="value">The wire string, for example <c>"alerts"</c>.</param>
    /// <exception cref="ArgumentNullException"><paramref name="value"/> is <see langword="null"/>.</exception>
    public AttributionSourceKind(string value)
    {
        _value = value ?? throw new ArgumentNullException(nameof(value));
    }

    /// <summary><c>alerts</c>.</summary>
    public static AttributionSourceKind Alerts { get; } = new("alerts");

    /// <summary><c>basemap</c>.</summary>
    public static AttributionSourceKind Basemap { get; } = new("basemap");

    /// <summary><c>climatology</c>.</summary>
    public static AttributionSourceKind Climatology { get; } = new("climatology");

    /// <summary><c>model</c>.</summary>
    public static AttributionSourceKind Model { get; } = new("model");

    /// <summary><c>observation</c>.</summary>
    public static AttributionSourceKind Observation { get; } = new("observation");

    /// <summary><c>peer</c>.</summary>
    public static AttributionSourceKind Peer { get; } = new("peer");

    /// <summary><c>places</c>.</summary>
    public static AttributionSourceKind Places { get; } = new("places");

    /// <summary><c>terrain</c>.</summary>
    public static AttributionSourceKind Terrain { get; } = new("terrain");

    /// <summary>Gets the wire string, exactly as the API sent it.</summary>
    public string Value => _value ?? string.Empty;

    /// <summary>Gets whether this SDK version knows the value.</summary>
    public bool IsKnown => Array.IndexOf(Known, Value) >= 0;

    /// <summary>Compares two values by their wire string.</summary>
    /// <param name="left">The first value.</param>
    /// <param name="right">The second value.</param>
    /// <returns><see langword="true"/> when the wire strings are equal.</returns>
    public static bool operator ==(AttributionSourceKind left, AttributionSourceKind right) => left.Equals(right);

    /// <summary>Compares two values by their wire string.</summary>
    /// <param name="left">The first value.</param>
    /// <param name="right">The second value.</param>
    /// <returns><see langword="true"/> when the wire strings differ.</returns>
    public static bool operator !=(AttributionSourceKind left, AttributionSourceKind right) => !left.Equals(right);

    /// <inheritdoc/>
    public bool Equals(AttributionSourceKind other) => string.Equals(Value, other.Value, StringComparison.Ordinal);

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is AttributionSourceKind other && Equals(other);

    /// <inheritdoc/>
    public override int GetHashCode() => StringComparer.Ordinal.GetHashCode(Value);

    /// <summary>Returns the wire string.</summary>
    /// <returns>The wire string.</returns>
    public override string ToString() => Value;
}
