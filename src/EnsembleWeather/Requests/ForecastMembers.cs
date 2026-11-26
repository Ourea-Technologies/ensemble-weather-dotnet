// SPDX-License-Identifier: BSD-2-Clause

using System;

namespace EnsembleWeather;

/// <summary>
/// What a forecast carries of the individual ensemble members (<c>members=</c>): nothing (<see cref="Off"/>, the
/// API's default), each hour's members (<see cref="On"/>), or the plume view (<see cref="Plume"/>).
/// </summary>
/// <remarks>
/// A <see cref="bool"/> converts implicitly, so <c>options.Members = true</c> asks for the hourly members. With
/// <see cref="Plume"/> the members come back as columns in <see cref="ForecastData.Plume"/> (and
/// <see cref="ElevationForecast.Plume"/>), and <see cref="ForecastData.Timeseries"/> is empty.
/// <see cref="ForecastResponse.MembersView"/> names the member set the tier carries, not the layout.
/// </remarks>
public readonly struct ForecastMembers : IEquatable<ForecastMembers>
{
    private readonly string? _value;

    private ForecastMembers(string value)
    {
        _value = value;
    }

    /// <summary>Gets no members (<c>members=false</c>, the API's default; never sent).</summary>
    public static ForecastMembers Off { get; } = new("false");

    /// <summary>Gets each hour's members (<c>members=true</c>).</summary>
    public static ForecastMembers On { get; } = new("true");

    /// <summary>Gets the plume view (<c>members=plume</c>): the members as columns, with bands and a headline.</summary>
    public static ForecastMembers Plume { get; } = new("plume");

    /// <summary>Gets the wire value: <c>false</c>, <c>true</c> or <c>plume</c>.</summary>
    public string Value => _value ?? "false";

    /// <summary>Converts a boolean: <see langword="true"/> is <see cref="On"/>, <see langword="false"/> is <see cref="Off"/>.</summary>
    /// <param name="value">Whether to include the hourly members.</param>
    public static implicit operator ForecastMembers(bool value) => value ? On : Off;

    /// <summary>Reads <c>true</c>, <c>false</c> or <c>plume</c>.</summary>
    /// <param name="value">The value.</param>
    /// <returns>The member choice.</returns>
    /// <exception cref="ArgumentException"><paramref name="value"/> is not one of the three.</exception>
    public static ForecastMembers Parse(string value) => value switch
    {
        "true" => On,
        "false" => Off,
        "plume" => Plume,
        _ => throw new ArgumentException($"members takes true, false or \"plume\", not '{value}'.", nameof(value)),
    };

    /// <summary>Converts a boolean (the named alternative to the implicit conversion).</summary>
    /// <param name="value">Whether to include the hourly members.</param>
    /// <returns><see cref="On"/> or <see cref="Off"/>.</returns>
    public static ForecastMembers FromBoolean(bool value) => value;

    /// <summary>Compares two choices.</summary>
    /// <param name="left">The first choice.</param>
    /// <param name="right">The second choice.</param>
    /// <returns><see langword="true"/> when they are the same.</returns>
    public static bool operator ==(ForecastMembers left, ForecastMembers right) => left.Equals(right);

    /// <summary>Compares two choices.</summary>
    /// <param name="left">The first choice.</param>
    /// <param name="right">The second choice.</param>
    /// <returns><see langword="true"/> when they differ.</returns>
    public static bool operator !=(ForecastMembers left, ForecastMembers right) => !left.Equals(right);

    /// <inheritdoc/>
    public bool Equals(ForecastMembers other) => string.Equals(Value, other.Value, StringComparison.Ordinal);

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is ForecastMembers other && Equals(other);

    /// <inheritdoc/>
    public override int GetHashCode() => StringComparer.Ordinal.GetHashCode(Value);

    /// <summary>Returns the wire value.</summary>
    /// <returns><c>false</c>, <c>true</c> or <c>plume</c>.</returns>
    public override string ToString() => Value;
}
