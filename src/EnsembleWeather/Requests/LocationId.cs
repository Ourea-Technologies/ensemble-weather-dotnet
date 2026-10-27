// SPDX-License-Identifier: BSD-2-Clause

using System;

namespace EnsembleWeather;

/// <summary>
/// A named location: the pair (state, slug), for example <c>or/mt-hood-meadows</c>.
/// </summary>
/// <remarks>
/// A two-letter state code is lowercased (the canonical form); a state name passes through and the server redirects
/// it. Both parts are sent as percent-encoded path segments.
/// </remarks>
public readonly struct LocationId : IEquatable<LocationId>
{
    /// <summary>Initializes a location id.</summary>
    /// <param name="state">The USPS state code (<c>or</c>) or state name.</param>
    /// <param name="slug">The location's catalog slug (<c>mt-hood-meadows</c>).</param>
    /// <exception cref="ArgumentException">A part is null or empty.</exception>
    public LocationId(string state, string slug)
    {
        if (string.IsNullOrEmpty(state))
        {
            throw new ArgumentException("A location needs a state.", nameof(state));
        }

        if (string.IsNullOrEmpty(slug))
        {
            throw new ArgumentException("A location needs a slug.", nameof(slug));
        }

        State = state;
        Slug = slug;
    }

    /// <summary>Gets the state, as given.</summary>
    public string State { get; }

    /// <summary>Gets the slug, as given.</summary>
    public string Slug { get; }

    /// <summary>Parses the combined form <c>state/slug</c>, splitting on its one <c>/</c>.</summary>
    /// <param name="location">For example <c>or/mt-hood-meadows</c>.</param>
    /// <returns>The location id.</returns>
    /// <exception cref="ArgumentException"><paramref name="location"/> does not contain exactly one <c>/</c>.</exception>
    public static LocationId Parse(string location)
    {
        if (location is null)
        {
            throw new ArgumentNullException(nameof(location));
        }

        int slash = location.IndexOf('/');
        if (slash <= 0 || slash == location.Length - 1 || location.IndexOf('/', slash + 1) >= 0)
        {
            throw new ArgumentException($"Expected 'state/slug' (for example 'or/mt-hood-meadows'), got '{location}'.", nameof(location));
        }

        return new LocationId(location.Substring(0, slash), location.Substring(slash + 1));
    }

    /// <summary>Compares two ids.</summary>
    /// <param name="left">The first id.</param>
    /// <param name="right">The second id.</param>
    /// <returns><see langword="true"/> when equal.</returns>
    public static bool operator ==(LocationId left, LocationId right) => left.Equals(right);

    /// <summary>Compares two ids.</summary>
    /// <param name="left">The first id.</param>
    /// <param name="right">The second id.</param>
    /// <returns><see langword="true"/> when different.</returns>
    public static bool operator !=(LocationId left, LocationId right) => !left.Equals(right);

    /// <inheritdoc/>
    public bool Equals(LocationId other)
        => string.Equals(CanonicalState, other.CanonicalState, StringComparison.Ordinal)
            && string.Equals(Slug, other.Slug, StringComparison.Ordinal);

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is LocationId other && Equals(other);

    /// <inheritdoc/>
    public override int GetHashCode() => (CanonicalState, Slug).GetHashCode();

    /// <summary>Formats the id as <c>state/slug</c>.</summary>
    /// <returns>The combined form.</returns>
    public override string ToString() => CanonicalState + "/" + Slug;

    /// <summary>The state as sent: a two-letter code lowercased, anything else as given.</summary>
    internal string CanonicalState
    {
        get
        {
            string state = State ?? string.Empty;
            return state.Length == 2 && IsAsciiLetter(state[0]) && IsAsciiLetter(state[1])
                ? state.ToLowerInvariant()
                : state;
        }
    }

    private static bool IsAsciiLetter(char c) => (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z');
}
