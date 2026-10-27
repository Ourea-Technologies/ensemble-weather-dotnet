// SPDX-License-Identifier: BSD-2-Clause

using System;
using System.Globalization;

namespace EnsembleWeather;

/// <summary>
/// A window of the forecast, in hours from the first served hour (the current hour, unless the request asks for
/// <c>include_past</c>): hours 0 to 24 is "the next 24 hours". Windows reach at most 360 h.
/// </summary>
public readonly struct ForecastWindow : IEquatable<ForecastWindow>
{
    /// <summary>The longest window end the API serves, 360 h.</summary>
    public const int MaxHours = 360;

    private readonly int _end;

    private ForecastWindow(int startHours, int endHours)
    {
        if (startHours < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(startHours), "A window cannot start before hour 0.");
        }

        if (endHours <= startHours || endHours > MaxHours)
        {
            throw new ArgumentOutOfRangeException(
                nameof(endHours), $"A window must end after it starts and at most {MaxHours} h out.");
        }

        StartHours = startHours;
        _end = endHours;
    }

    /// <summary>Gets the default window, the next 24 hours.</summary>
    public static ForecastWindow Default { get; } = new(0, 24);

    /// <summary>Gets the window start, in hours from the first served hour.</summary>
    public int StartHours { get; }

    /// <summary>Gets the window end, in hours from the first served hour (24 for the default window).</summary>
    public int EndHours => _end == 0 ? 24 : _end;

    /// <summary>The next <paramref name="hours"/> hours: from hour 0 to <paramref name="hours"/>.</summary>
    /// <param name="hours">The window length, 1 to 360.</param>
    /// <returns>The window.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="hours"/> is outside 1..360.</exception>
    public static ForecastWindow Hours(int hours) => new(0, hours);

    /// <summary>The hours from <paramref name="startHours"/> to <paramref name="endHours"/>.</summary>
    /// <param name="startHours">The start, 0 or later.</param>
    /// <param name="endHours">The end, after the start and at most 360.</param>
    /// <returns>The window.</returns>
    /// <exception cref="ArgumentOutOfRangeException">The window is empty, reversed or past 360 h.</exception>
    public static ForecastWindow Between(int startHours, int endHours) => new(startHours, endHours);

    /// <summary>Compares two windows.</summary>
    /// <param name="left">The first window.</param>
    /// <param name="right">The second window.</param>
    /// <returns><see langword="true"/> when equal.</returns>
    public static bool operator ==(ForecastWindow left, ForecastWindow right) => left.Equals(right);

    /// <summary>Compares two windows.</summary>
    /// <param name="left">The first window.</param>
    /// <param name="right">The second window.</param>
    /// <returns><see langword="true"/> when different.</returns>
    public static bool operator !=(ForecastWindow left, ForecastWindow right) => !left.Equals(right);

    /// <inheritdoc/>
    public bool Equals(ForecastWindow other) => StartHours == other.StartHours && EndHours == other.EndHours;

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is ForecastWindow other && Equals(other);

    /// <inheritdoc/>
    public override int GetHashCode() => (StartHours, EndHours).GetHashCode();

    /// <summary>Formats the window as the API writes it: <c>24h</c>, or <c>12-36h</c> when it does not start at 0.</summary>
    /// <returns>The wire form.</returns>
    public override string ToString()
        => StartHours == 0
            ? EndHours.ToString(CultureInfo.InvariantCulture) + "h"
            : string.Format(CultureInfo.InvariantCulture, "{0}-{1}h", StartHours, EndHours);
}
