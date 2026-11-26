// SPDX-License-Identifier: BSD-2-Clause

using System;
using System.Globalization;

namespace EnsembleWeather;

/// <summary>
/// A point in decimal degrees (WGS84), as the <c>near</c> filter of <see cref="ExploreOptions"/> and
/// <see cref="SearchOptions"/> takes it. It is sent as <c>lat,lon</c> with a literal comma, each number in its
/// shortest round-trip form.
/// </summary>
public readonly struct GeoPoint : IEquatable<GeoPoint>
{
    /// <summary>Initializes a point.</summary>
    /// <param name="latitude">The latitude, -90 to 90.</param>
    /// <param name="longitude">The longitude, -180 to 180, west negative.</param>
    public GeoPoint(double latitude, double longitude)
    {
        Latitude = latitude;
        Longitude = longitude;
    }

    /// <summary>Gets the latitude, decimal degrees.</summary>
    public double Latitude { get; }

    /// <summary>Gets the longitude, decimal degrees, west negative.</summary>
    public double Longitude { get; }

    /// <summary>Compares two points by their coordinates.</summary>
    /// <param name="left">The first point.</param>
    /// <param name="right">The second point.</param>
    /// <returns><see langword="true"/> when both coordinates are equal.</returns>
    public static bool operator ==(GeoPoint left, GeoPoint right) => left.Equals(right);

    /// <summary>Compares two points by their coordinates.</summary>
    /// <param name="left">The first point.</param>
    /// <param name="right">The second point.</param>
    /// <returns><see langword="true"/> when a coordinate differs.</returns>
    public static bool operator !=(GeoPoint left, GeoPoint right) => !left.Equals(right);

    /// <inheritdoc/>
    public bool Equals(GeoPoint other) => Latitude.Equals(other.Latitude) && Longitude.Equals(other.Longitude);

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is GeoPoint other && Equals(other);

    /// <inheritdoc/>
    public override int GetHashCode() => (Latitude, Longitude).GetHashCode();

    /// <summary>Returns the point as the API's <c>lat,lon</c>.</summary>
    /// <returns>The point, for example <c>45.33,-121.66</c>.</returns>
    public override string ToString()
        => Latitude.ToString("R", CultureInfo.InvariantCulture) + "," + Longitude.ToString("R", CultureInfo.InvariantCulture);
}
