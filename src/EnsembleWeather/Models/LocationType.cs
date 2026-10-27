// SPDX-License-Identifier: BSD-2-Clause

using System;
using System.Text.Json.Serialization;

namespace EnsembleWeather;

/// <summary>
/// The category of a location.
/// </summary>
/// <remarks>
/// An extensible enumeration: a value this SDK version does not know (the API adds values over time) still
/// parses, keeps its raw string in <see cref="Value"/>, and reports <see cref="IsKnown"/> as <see langword="false"/>.
/// Compare with the static members, for example <c>value == LocationType.Airport</c>.
/// </remarks>
[JsonConverter(typeof(Serialization.LocationTypeConverter))]
public readonly struct LocationType : IEquatable<LocationType>
{
    private static readonly string[] Known = ["airport", "backcountry", "beach_coast", "campground", "city_town", "climbing_area", "coastal_point", "event_venue", "fire_lookout", "fishing_river_access", "golf_course", "hot_spring", "lake", "mountain_pass", "national_park", "observation_station", "other", "park", "recreation_area", "reservoir_lake", "scenic_tourist", "ski_area", "state_park", "summit", "trailhead", "volcano", "waterfall", "wine_agritourism"];
    private readonly string? _value;

    /// <summary>Initializes a value from its wire string.</summary>
    /// <param name="value">The wire string, for example <c>"airport"</c>.</param>
    /// <exception cref="ArgumentNullException"><paramref name="value"/> is <see langword="null"/>.</exception>
    public LocationType(string value)
    {
        _value = value ?? throw new ArgumentNullException(nameof(value));
    }

    /// <summary><c>airport</c>.</summary>
    public static LocationType Airport { get; } = new("airport");

    /// <summary><c>backcountry</c>.</summary>
    public static LocationType Backcountry { get; } = new("backcountry");

    /// <summary><c>beach_coast</c>.</summary>
    public static LocationType BeachCoast { get; } = new("beach_coast");

    /// <summary><c>campground</c>.</summary>
    public static LocationType Campground { get; } = new("campground");

    /// <summary><c>city_town</c>.</summary>
    public static LocationType CityTown { get; } = new("city_town");

    /// <summary><c>climbing_area</c>.</summary>
    public static LocationType ClimbingArea { get; } = new("climbing_area");

    /// <summary><c>coastal_point</c>.</summary>
    public static LocationType CoastalPoint { get; } = new("coastal_point");

    /// <summary><c>event_venue</c>.</summary>
    public static LocationType EventVenue { get; } = new("event_venue");

    /// <summary><c>fire_lookout</c>.</summary>
    public static LocationType FireLookout { get; } = new("fire_lookout");

    /// <summary><c>fishing_river_access</c>.</summary>
    public static LocationType FishingRiverAccess { get; } = new("fishing_river_access");

    /// <summary><c>golf_course</c>.</summary>
    public static LocationType GolfCourse { get; } = new("golf_course");

    /// <summary><c>hot_spring</c>.</summary>
    public static LocationType HotSpring { get; } = new("hot_spring");

    /// <summary><c>lake</c>.</summary>
    public static LocationType Lake { get; } = new("lake");

    /// <summary><c>mountain_pass</c>.</summary>
    public static LocationType MountainPass { get; } = new("mountain_pass");

    /// <summary><c>national_park</c>.</summary>
    public static LocationType NationalPark { get; } = new("national_park");

    /// <summary><c>observation_station</c>.</summary>
    public static LocationType ObservationStation { get; } = new("observation_station");

    /// <summary><c>other</c>.</summary>
    public static LocationType Other { get; } = new("other");

    /// <summary><c>park</c>.</summary>
    public static LocationType Park { get; } = new("park");

    /// <summary><c>recreation_area</c>.</summary>
    public static LocationType RecreationArea { get; } = new("recreation_area");

    /// <summary><c>reservoir_lake</c>.</summary>
    public static LocationType ReservoirLake { get; } = new("reservoir_lake");

    /// <summary><c>scenic_tourist</c>.</summary>
    public static LocationType ScenicTourist { get; } = new("scenic_tourist");

    /// <summary><c>ski_area</c>.</summary>
    public static LocationType SkiArea { get; } = new("ski_area");

    /// <summary><c>state_park</c>.</summary>
    public static LocationType StatePark { get; } = new("state_park");

    /// <summary><c>summit</c>.</summary>
    public static LocationType Summit { get; } = new("summit");

    /// <summary><c>trailhead</c>.</summary>
    public static LocationType Trailhead { get; } = new("trailhead");

    /// <summary><c>volcano</c>.</summary>
    public static LocationType Volcano { get; } = new("volcano");

    /// <summary><c>waterfall</c>.</summary>
    public static LocationType Waterfall { get; } = new("waterfall");

    /// <summary><c>wine_agritourism</c>.</summary>
    public static LocationType WineAgritourism { get; } = new("wine_agritourism");

    /// <summary>Gets the wire string, exactly as the API sent it.</summary>
    public string Value => _value ?? string.Empty;

    /// <summary>Gets whether this SDK version knows the value.</summary>
    public bool IsKnown => Array.IndexOf(Known, Value) >= 0;

    /// <summary>Compares two values by their wire string.</summary>
    /// <param name="left">The first value.</param>
    /// <param name="right">The second value.</param>
    /// <returns><see langword="true"/> when the wire strings are equal.</returns>
    public static bool operator ==(LocationType left, LocationType right) => left.Equals(right);

    /// <summary>Compares two values by their wire string.</summary>
    /// <param name="left">The first value.</param>
    /// <param name="right">The second value.</param>
    /// <returns><see langword="true"/> when the wire strings differ.</returns>
    public static bool operator !=(LocationType left, LocationType right) => !left.Equals(right);

    /// <inheritdoc/>
    public bool Equals(LocationType other) => string.Equals(Value, other.Value, StringComparison.Ordinal);

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is LocationType other && Equals(other);

    /// <inheritdoc/>
    public override int GetHashCode() => StringComparer.Ordinal.GetHashCode(Value);

    /// <summary>Returns the wire string.</summary>
    /// <returns>The wire string.</returns>
    public override string ToString() => Value;
}
