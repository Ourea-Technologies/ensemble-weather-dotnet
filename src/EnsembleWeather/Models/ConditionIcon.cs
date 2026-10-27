// SPDX-License-Identifier: BSD-2-Clause

using System;
using System.Text.Json.Serialization;

namespace EnsembleWeather;

/// <summary>
/// An icon hint for a condition (night forms are prefixed <c>night_</c>).
/// </summary>
/// <remarks>
/// An extensible enumeration: a value this SDK version does not know (the API adds values over time) still
/// parses, keeps its raw string in <see cref="Value"/>, and reports <see cref="IsKnown"/> as <see langword="false"/>.
/// Compare with the static members, for example <c>value == ConditionIcon.ChanceRain</c>.
/// </remarks>
[JsonConverter(typeof(Serialization.ConditionIconConverter))]
public readonly struct ConditionIcon : IEquatable<ConditionIcon>
{
    private static readonly string[] Known = ["chance_rain", "chance_showers", "cloudy", "freezing_rain", "heavy_rain", "light_rain", "mostly_cloudy", "mostly_sunny", "night_chance_showers", "night_clear", "night_cloudy", "night_freezing_rain", "night_heavy_rain", "night_light_rain", "night_moon_chance_showers", "night_moon_showers_likely", "night_mostly_clear", "night_mostly_cloudy", "night_partly_cloudy", "night_rain", "night_rain_snow_mix", "night_showers_likely", "night_snow", "partly_cloudy", "rain", "rain_likely", "rain_snow_mix", "showers", "showers_likely", "snow", "sun_chance_showers", "sun_showers", "sun_showers_likely", "sunny"];
    private readonly string? _value;

    /// <summary>Initializes a value from its wire string.</summary>
    /// <param name="value">The wire string, for example <c>"chance_rain"</c>.</param>
    /// <exception cref="ArgumentNullException"><paramref name="value"/> is <see langword="null"/>.</exception>
    public ConditionIcon(string value)
    {
        _value = value ?? throw new ArgumentNullException(nameof(value));
    }

    /// <summary><c>chance_rain</c>.</summary>
    public static ConditionIcon ChanceRain { get; } = new("chance_rain");

    /// <summary><c>chance_showers</c>.</summary>
    public static ConditionIcon ChanceShowers { get; } = new("chance_showers");

    /// <summary><c>cloudy</c>.</summary>
    public static ConditionIcon Cloudy { get; } = new("cloudy");

    /// <summary><c>freezing_rain</c>.</summary>
    public static ConditionIcon FreezingRain { get; } = new("freezing_rain");

    /// <summary><c>heavy_rain</c>.</summary>
    public static ConditionIcon HeavyRain { get; } = new("heavy_rain");

    /// <summary><c>light_rain</c>.</summary>
    public static ConditionIcon LightRain { get; } = new("light_rain");

    /// <summary><c>mostly_cloudy</c>.</summary>
    public static ConditionIcon MostlyCloudy { get; } = new("mostly_cloudy");

    /// <summary><c>mostly_sunny</c>.</summary>
    public static ConditionIcon MostlySunny { get; } = new("mostly_sunny");

    /// <summary><c>night_chance_showers</c>.</summary>
    public static ConditionIcon NightChanceShowers { get; } = new("night_chance_showers");

    /// <summary><c>night_clear</c>.</summary>
    public static ConditionIcon NightClear { get; } = new("night_clear");

    /// <summary><c>night_cloudy</c>.</summary>
    public static ConditionIcon NightCloudy { get; } = new("night_cloudy");

    /// <summary><c>night_freezing_rain</c>.</summary>
    public static ConditionIcon NightFreezingRain { get; } = new("night_freezing_rain");

    /// <summary><c>night_heavy_rain</c>.</summary>
    public static ConditionIcon NightHeavyRain { get; } = new("night_heavy_rain");

    /// <summary><c>night_light_rain</c>.</summary>
    public static ConditionIcon NightLightRain { get; } = new("night_light_rain");

    /// <summary><c>night_moon_chance_showers</c>.</summary>
    public static ConditionIcon NightMoonChanceShowers { get; } = new("night_moon_chance_showers");

    /// <summary><c>night_moon_showers_likely</c>.</summary>
    public static ConditionIcon NightMoonShowersLikely { get; } = new("night_moon_showers_likely");

    /// <summary><c>night_mostly_clear</c>.</summary>
    public static ConditionIcon NightMostlyClear { get; } = new("night_mostly_clear");

    /// <summary><c>night_mostly_cloudy</c>.</summary>
    public static ConditionIcon NightMostlyCloudy { get; } = new("night_mostly_cloudy");

    /// <summary><c>night_partly_cloudy</c>.</summary>
    public static ConditionIcon NightPartlyCloudy { get; } = new("night_partly_cloudy");

    /// <summary><c>night_rain</c>.</summary>
    public static ConditionIcon NightRain { get; } = new("night_rain");

    /// <summary><c>night_rain_snow_mix</c>.</summary>
    public static ConditionIcon NightRainSnowMix { get; } = new("night_rain_snow_mix");

    /// <summary><c>night_showers_likely</c>.</summary>
    public static ConditionIcon NightShowersLikely { get; } = new("night_showers_likely");

    /// <summary><c>night_snow</c>.</summary>
    public static ConditionIcon NightSnow { get; } = new("night_snow");

    /// <summary><c>partly_cloudy</c>.</summary>
    public static ConditionIcon PartlyCloudy { get; } = new("partly_cloudy");

    /// <summary><c>rain</c>.</summary>
    public static ConditionIcon Rain { get; } = new("rain");

    /// <summary><c>rain_likely</c>.</summary>
    public static ConditionIcon RainLikely { get; } = new("rain_likely");

    /// <summary><c>rain_snow_mix</c>.</summary>
    public static ConditionIcon RainSnowMix { get; } = new("rain_snow_mix");

    /// <summary><c>showers</c>.</summary>
    public static ConditionIcon Showers { get; } = new("showers");

    /// <summary><c>showers_likely</c>.</summary>
    public static ConditionIcon ShowersLikely { get; } = new("showers_likely");

    /// <summary><c>snow</c>.</summary>
    public static ConditionIcon Snow { get; } = new("snow");

    /// <summary><c>sun_chance_showers</c>.</summary>
    public static ConditionIcon SunChanceShowers { get; } = new("sun_chance_showers");

    /// <summary><c>sun_showers</c>.</summary>
    public static ConditionIcon SunShowers { get; } = new("sun_showers");

    /// <summary><c>sun_showers_likely</c>.</summary>
    public static ConditionIcon SunShowersLikely { get; } = new("sun_showers_likely");

    /// <summary><c>sunny</c>.</summary>
    public static ConditionIcon Sunny { get; } = new("sunny");

    /// <summary>Gets the wire string, exactly as the API sent it.</summary>
    public string Value => _value ?? string.Empty;

    /// <summary>Gets whether this SDK version knows the value.</summary>
    public bool IsKnown => Array.IndexOf(Known, Value) >= 0;

    /// <summary>Compares two values by their wire string.</summary>
    /// <param name="left">The first value.</param>
    /// <param name="right">The second value.</param>
    /// <returns><see langword="true"/> when the wire strings are equal.</returns>
    public static bool operator ==(ConditionIcon left, ConditionIcon right) => left.Equals(right);

    /// <summary>Compares two values by their wire string.</summary>
    /// <param name="left">The first value.</param>
    /// <param name="right">The second value.</param>
    /// <returns><see langword="true"/> when the wire strings differ.</returns>
    public static bool operator !=(ConditionIcon left, ConditionIcon right) => !left.Equals(right);

    /// <inheritdoc/>
    public bool Equals(ConditionIcon other) => string.Equals(Value, other.Value, StringComparison.Ordinal);

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is ConditionIcon other && Equals(other);

    /// <inheritdoc/>
    public override int GetHashCode() => StringComparer.Ordinal.GetHashCode(Value);

    /// <summary>Returns the wire string.</summary>
    /// <returns>The wire string.</returns>
    public override string ToString() => Value;
}
