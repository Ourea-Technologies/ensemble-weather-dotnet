// SPDX-License-Identifier: BSD-2-Clause

using System;
using System.Globalization;

namespace EnsembleWeather;

/// <summary>
/// A pair of served percentiles, for example the 10th to 90th: "most members fall between these".
/// </summary>
public readonly struct QuantileRange : IEquatable<QuantileRange>
{
    /// <summary>Initializes a range.</summary>
    /// <param name="lowPercentile">The low percentile, for example 10.</param>
    /// <param name="low">The value at the low percentile.</param>
    /// <param name="highPercentile">The high percentile, for example 90.</param>
    /// <param name="high">The value at the high percentile.</param>
    public QuantileRange(int lowPercentile, double low, int highPercentile, double high)
    {
        LowPercentile = lowPercentile;
        Low = low;
        HighPercentile = highPercentile;
        High = high;
    }

    /// <summary>Gets the low percentile, for example 10.</summary>
    public int LowPercentile { get; }

    /// <summary>Gets the value at the low percentile.</summary>
    public double Low { get; }

    /// <summary>Gets the high percentile, for example 90.</summary>
    public int HighPercentile { get; }

    /// <summary>Gets the value at the high percentile.</summary>
    public double High { get; }

    /// <summary>Compares two ranges.</summary>
    /// <param name="left">The first range.</param>
    /// <param name="right">The second range.</param>
    /// <returns><see langword="true"/> when equal.</returns>
    public static bool operator ==(QuantileRange left, QuantileRange right) => left.Equals(right);

    /// <summary>Compares two ranges.</summary>
    /// <param name="left">The first range.</param>
    /// <param name="right">The second range.</param>
    /// <returns><see langword="true"/> when different.</returns>
    public static bool operator !=(QuantileRange left, QuantileRange right) => !left.Equals(right);

    /// <inheritdoc/>
    public bool Equals(QuantileRange other)
        => LowPercentile == other.LowPercentile && Low.Equals(other.Low)
            && HighPercentile == other.HighPercentile && High.Equals(other.High);

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is QuantileRange other && Equals(other);

    /// <inheritdoc/>
    public override int GetHashCode()
        => (LowPercentile, Low, HighPercentile, High).GetHashCode();

    /// <summary>Formats the range as <c>low–high</c> with the invariant culture.</summary>
    /// <returns>For example <c>-4–0</c>.</returns>
    public override string ToString()
        => string.Format(CultureInfo.InvariantCulture, "{0}–{1}", Low, High);
}
