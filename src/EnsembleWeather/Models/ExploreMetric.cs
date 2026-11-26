// SPDX-License-Identifier: BSD-2-Clause

using System;
using System.Text.Json.Serialization;

namespace EnsembleWeather;

/// <summary>
/// What <see cref="OureaClient.ExplorePageAsync"/> ranks places by, and a
/// search's metric sort (<see cref="SearchOptions.Sort"/>).
/// </summary>
/// <remarks>
/// An extensible enumeration: a value this SDK version does not know (the API adds values over time) still
/// parses, keeps its raw string in <see cref="Value"/>, and reports <see cref="IsKnown"/> as <see langword="false"/>.
/// Compare with the static members, for example <c>value == ExploreMetric.BaseDepth</c>.
/// </remarks>
[JsonConverter(typeof(Serialization.ExploreMetricConverter))]
public readonly struct ExploreMetric : IEquatable<ExploreMetric>
{
    private static readonly string[] Known = ["base_depth", "new_snow_24h", "precip_10d", "precip_14d", "precip_5d", "precip_today", "season_precip", "season_precip_pct_normal", "season_snow", "snow_10d", "snow_14d", "snow_5d", "snow_today"];
    private readonly string? _value;

    /// <summary>Initializes a value from its wire string.</summary>
    /// <param name="value">The wire string, for example <c>"base_depth"</c>.</param>
    /// <exception cref="ArgumentNullException"><paramref name="value"/> is <see langword="null"/>.</exception>
    public ExploreMetric(string value)
    {
        _value = value ?? throw new ArgumentNullException(nameof(value));
    }

    /// <summary><c>base_depth</c>.</summary>
    public static ExploreMetric BaseDepth { get; } = new("base_depth");

    /// <summary><c>new_snow_24h</c>.</summary>
    public static ExploreMetric NewSnow24h { get; } = new("new_snow_24h");

    /// <summary><c>precip_10d</c>.</summary>
    public static ExploreMetric Precip10d { get; } = new("precip_10d");

    /// <summary><c>precip_14d</c>.</summary>
    public static ExploreMetric Precip14d { get; } = new("precip_14d");

    /// <summary><c>precip_5d</c>.</summary>
    public static ExploreMetric Precip5d { get; } = new("precip_5d");

    /// <summary><c>precip_today</c>.</summary>
    public static ExploreMetric PrecipToday { get; } = new("precip_today");

    /// <summary><c>season_precip</c>.</summary>
    public static ExploreMetric SeasonPrecip { get; } = new("season_precip");

    /// <summary><c>season_precip_pct_normal</c>.</summary>
    public static ExploreMetric SeasonPrecipPctNormal { get; } = new("season_precip_pct_normal");

    /// <summary><c>season_snow</c>.</summary>
    public static ExploreMetric SeasonSnow { get; } = new("season_snow");

    /// <summary><c>snow_10d</c>.</summary>
    public static ExploreMetric Snow10d { get; } = new("snow_10d");

    /// <summary><c>snow_14d</c>.</summary>
    public static ExploreMetric Snow14d { get; } = new("snow_14d");

    /// <summary><c>snow_5d</c>.</summary>
    public static ExploreMetric Snow5d { get; } = new("snow_5d");

    /// <summary><c>snow_today</c>.</summary>
    public static ExploreMetric SnowToday { get; } = new("snow_today");

    /// <summary>Gets the wire string, exactly as the API sent it.</summary>
    public string Value => _value ?? string.Empty;

    /// <summary>Gets whether this SDK version knows the value.</summary>
    public bool IsKnown => Array.IndexOf(Known, Value) >= 0;

    /// <summary>Compares two values by their wire string.</summary>
    /// <param name="left">The first value.</param>
    /// <param name="right">The second value.</param>
    /// <returns><see langword="true"/> when the wire strings are equal.</returns>
    public static bool operator ==(ExploreMetric left, ExploreMetric right) => left.Equals(right);

    /// <summary>Compares two values by their wire string.</summary>
    /// <param name="left">The first value.</param>
    /// <param name="right">The second value.</param>
    /// <returns><see langword="true"/> when the wire strings differ.</returns>
    public static bool operator !=(ExploreMetric left, ExploreMetric right) => !left.Equals(right);

    /// <inheritdoc/>
    public bool Equals(ExploreMetric other) => string.Equals(Value, other.Value, StringComparison.Ordinal);

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is ExploreMetric other && Equals(other);

    /// <inheritdoc/>
    public override int GetHashCode() => StringComparer.Ordinal.GetHashCode(Value);

    /// <summary>Returns the wire string.</summary>
    /// <returns>The wire string.</returns>
    public override string ToString() => Value;
}
