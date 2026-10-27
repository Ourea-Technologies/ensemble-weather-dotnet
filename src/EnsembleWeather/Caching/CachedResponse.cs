// SPDX-License-Identifier: BSD-2-Clause

using System;

namespace EnsembleWeather;

/// <summary>A stored response body with what the SDK needs to revalidate it.</summary>
public sealed class CachedResponse
{
    /// <summary>Initializes an entry.</summary>
    /// <param name="entityTag">The <c>ETag</c>, byte for byte as received (including any <c>W/</c>).</param>
    /// <param name="body">The response body (JSON).</param>
    /// <param name="storedAt">When the response was received or last revalidated.</param>
    /// <param name="maxAge">The response's <c>Cache-Control: max-age</c>, if any.</param>
    /// <exception cref="ArgumentNullException"><paramref name="body"/> is <see langword="null"/>.</exception>
    public CachedResponse(string? entityTag, byte[] body, DateTimeOffset storedAt, TimeSpan? maxAge)
    {
        EntityTag = entityTag;
        Body = body ?? throw new ArgumentNullException(nameof(body));
        StoredAt = storedAt;
        MaxAge = maxAge;
    }

    /// <summary>Gets the <c>ETag</c>, sent back as <c>If-None-Match</c> verbatim.</summary>
    public string? EntityTag { get; }

    /// <summary>Gets the stored body.</summary>
#pragma warning disable CA1819 // A cache entry is a byte buffer by nature; it is never mutated.
    public byte[] Body { get; }
#pragma warning restore CA1819

    /// <summary>Gets when the entry was stored or last revalidated.</summary>
    public DateTimeOffset StoredAt { get; }

    /// <summary>
    /// Gets the <c>max-age</c> the response allowed. The SDK is a private cache: it uses <c>max-age</c> (0 on the
    /// API, so every call revalidates) and ignores <c>s-maxage</c> and <c>stale-while-revalidate</c>.
    /// </summary>
    public TimeSpan? MaxAge { get; }

    /// <summary>Gets whether the entry may be served without revalidating at <paramref name="now"/>.</summary>
    /// <param name="now">The current time.</param>
    /// <returns><see langword="true"/> while the entry is younger than its <see cref="MaxAge"/>.</returns>
    public bool IsFresh(DateTimeOffset now) => MaxAge is TimeSpan age && age > TimeSpan.Zero && now - StoredAt < age;
}
