// SPDX-License-Identifier: BSD-2-Clause

using System;
using System.Text.Json.Serialization;

namespace EnsembleWeather;

/// <summary>
/// A machine-readable weather condition code, hourly or daily.
/// </summary>
/// <remarks>
/// An extensible enumeration: a value this SDK version does not know (the API adds values over time) still
/// parses, keeps its raw string in <see cref="Value"/>, and reports <see cref="IsKnown"/> as <see langword="false"/>.
/// Compare with the static members, for example <c>value == Condition.ChanceFreezingRain</c>.
/// </remarks>
[JsonConverter(typeof(Serialization.ConditionConverter))]
public readonly struct Condition : IEquatable<Condition>
{
    private static readonly string[] Known = ["chance_freezing_rain", "chance_rain_snow_mix", "chance_showers", "chance_snow", "clear", "cloudy", "freezing_rain", "freezing_rain_likely", "heavy_rain", "heavy_snow", "light_rain", "light_snow", "mostly_clear", "mostly_cloudy", "mostly_sunny", "partly_cloudy", "rain", "rain_snow_mix", "rain_snow_mix_likely", "showers_likely", "slight_chance_freezing_rain", "slight_chance_rain_snow_mix", "slight_chance_showers", "slight_chance_snow", "snow", "snow_likely", "sunny"];
    private readonly string? _value;

    /// <summary>Initializes a value from its wire string.</summary>
    /// <param name="value">The wire string, for example <c>"chance_freezing_rain"</c>.</param>
    /// <exception cref="ArgumentNullException"><paramref name="value"/> is <see langword="null"/>.</exception>
    public Condition(string value)
    {
        _value = value ?? throw new ArgumentNullException(nameof(value));
    }

    /// <summary><c>chance_freezing_rain</c>.</summary>
    public static Condition ChanceFreezingRain { get; } = new("chance_freezing_rain");

    /// <summary><c>chance_rain_snow_mix</c>.</summary>
    public static Condition ChanceRainSnowMix { get; } = new("chance_rain_snow_mix");

    /// <summary><c>chance_showers</c>.</summary>
    public static Condition ChanceShowers { get; } = new("chance_showers");

    /// <summary><c>chance_snow</c>.</summary>
    public static Condition ChanceSnow { get; } = new("chance_snow");

    /// <summary><c>clear</c>.</summary>
    public static Condition Clear { get; } = new("clear");

    /// <summary><c>cloudy</c>.</summary>
    public static Condition Cloudy { get; } = new("cloudy");

    /// <summary><c>freezing_rain</c>.</summary>
    public static Condition FreezingRain { get; } = new("freezing_rain");

    /// <summary><c>freezing_rain_likely</c>.</summary>
    public static Condition FreezingRainLikely { get; } = new("freezing_rain_likely");

    /// <summary><c>heavy_rain</c>.</summary>
    public static Condition HeavyRain { get; } = new("heavy_rain");

    /// <summary><c>heavy_snow</c>.</summary>
    public static Condition HeavySnow { get; } = new("heavy_snow");

    /// <summary><c>light_rain</c>.</summary>
    public static Condition LightRain { get; } = new("light_rain");

    /// <summary><c>light_snow</c>.</summary>
    public static Condition LightSnow { get; } = new("light_snow");

    /// <summary><c>mostly_clear</c>.</summary>
    public static Condition MostlyClear { get; } = new("mostly_clear");

    /// <summary><c>mostly_cloudy</c>.</summary>
    public static Condition MostlyCloudy { get; } = new("mostly_cloudy");

    /// <summary><c>mostly_sunny</c>.</summary>
    public static Condition MostlySunny { get; } = new("mostly_sunny");

    /// <summary><c>partly_cloudy</c>.</summary>
    public static Condition PartlyCloudy { get; } = new("partly_cloudy");

    /// <summary><c>rain</c>.</summary>
    public static Condition Rain { get; } = new("rain");

    /// <summary><c>rain_snow_mix</c>.</summary>
    public static Condition RainSnowMix { get; } = new("rain_snow_mix");

    /// <summary><c>rain_snow_mix_likely</c>.</summary>
    public static Condition RainSnowMixLikely { get; } = new("rain_snow_mix_likely");

    /// <summary><c>showers_likely</c>.</summary>
    public static Condition ShowersLikely { get; } = new("showers_likely");

    /// <summary><c>slight_chance_freezing_rain</c>.</summary>
    public static Condition SlightChanceFreezingRain { get; } = new("slight_chance_freezing_rain");

    /// <summary><c>slight_chance_rain_snow_mix</c>.</summary>
    public static Condition SlightChanceRainSnowMix { get; } = new("slight_chance_rain_snow_mix");

    /// <summary><c>slight_chance_showers</c>.</summary>
    public static Condition SlightChanceShowers { get; } = new("slight_chance_showers");

    /// <summary><c>slight_chance_snow</c>.</summary>
    public static Condition SlightChanceSnow { get; } = new("slight_chance_snow");

    /// <summary><c>snow</c>.</summary>
    public static Condition Snow { get; } = new("snow");

    /// <summary><c>snow_likely</c>.</summary>
    public static Condition SnowLikely { get; } = new("snow_likely");

    /// <summary><c>sunny</c>.</summary>
    public static Condition Sunny { get; } = new("sunny");

    /// <summary>Gets the wire string, exactly as the API sent it.</summary>
    public string Value => _value ?? string.Empty;

    /// <summary>Gets whether this SDK version knows the value.</summary>
    public bool IsKnown => Array.IndexOf(Known, Value) >= 0;

    /// <summary>Compares two values by their wire string.</summary>
    /// <param name="left">The first value.</param>
    /// <param name="right">The second value.</param>
    /// <returns><see langword="true"/> when the wire strings are equal.</returns>
    public static bool operator ==(Condition left, Condition right) => left.Equals(right);

    /// <summary>Compares two values by their wire string.</summary>
    /// <param name="left">The first value.</param>
    /// <param name="right">The second value.</param>
    /// <returns><see langword="true"/> when the wire strings differ.</returns>
    public static bool operator !=(Condition left, Condition right) => !left.Equals(right);

    /// <inheritdoc/>
    public bool Equals(Condition other) => string.Equals(Value, other.Value, StringComparison.Ordinal);

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is Condition other && Equals(other);

    /// <inheritdoc/>
    public override int GetHashCode() => StringComparer.Ordinal.GetHashCode(Value);

    /// <summary>Returns the wire string.</summary>
    /// <returns>The wire string.</returns>
    public override string ToString() => Value;
}
