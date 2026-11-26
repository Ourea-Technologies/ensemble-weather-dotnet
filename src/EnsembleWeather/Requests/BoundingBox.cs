// SPDX-License-Identifier: BSD-2-Clause

using System;
using System.Globalization;

namespace EnsembleWeather;

/// <summary>
/// A box in decimal degrees (WGS84), as RFC 7946's <c>bbox</c>: west, south, east, north. It is sent as
/// <c>west,south,east,north</c> with literal commas, each number in its shortest round-trip form.
/// </summary>
public readonly struct BoundingBox : IEquatable<BoundingBox>
{
    /// <summary>Initializes a box.</summary>
    /// <param name="west">The west edge (longitude, -180 to 180); at most <paramref name="east"/>.</param>
    /// <param name="south">The south edge (latitude, -90 to 90); at most <paramref name="north"/>.</param>
    /// <param name="east">The east edge (longitude, -180 to 180).</param>
    /// <param name="north">The north edge (latitude, -90 to 90).</param>
    public BoundingBox(double west, double south, double east, double north)
    {
        West = west;
        South = south;
        East = east;
        North = north;
    }

    /// <summary>Gets the west edge, a longitude.</summary>
    public double West { get; }

    /// <summary>Gets the south edge, a latitude.</summary>
    public double South { get; }

    /// <summary>Gets the east edge, a longitude.</summary>
    public double East { get; }

    /// <summary>Gets the north edge, a latitude.</summary>
    public double North { get; }

    /// <summary>Compares two boxes by their edges.</summary>
    /// <param name="left">The first box.</param>
    /// <param name="right">The second box.</param>
    /// <returns><see langword="true"/> when every edge is equal.</returns>
    public static bool operator ==(BoundingBox left, BoundingBox right) => left.Equals(right);

    /// <summary>Compares two boxes by their edges.</summary>
    /// <param name="left">The first box.</param>
    /// <param name="right">The second box.</param>
    /// <returns><see langword="true"/> when an edge differs.</returns>
    public static bool operator !=(BoundingBox left, BoundingBox right) => !left.Equals(right);

    /// <inheritdoc/>
    public bool Equals(BoundingBox other)
        => West.Equals(other.West) && South.Equals(other.South) && East.Equals(other.East) && North.Equals(other.North);

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is BoundingBox other && Equals(other);

    /// <inheritdoc/>
    public override int GetHashCode() => (West, South, East, North).GetHashCode();

    /// <summary>Returns the box as the API's <c>west,south,east,north</c>.</summary>
    /// <returns>The box, for example <c>-125,41,-116,49</c>.</returns>
    public override string ToString() => string.Join(
        ",",
        West.ToString("R", CultureInfo.InvariantCulture),
        South.ToString("R", CultureInfo.InvariantCulture),
        East.ToString("R", CultureInfo.InvariantCulture),
        North.ToString("R", CultureInfo.InvariantCulture));
}
