// SPDX-License-Identifier: BSD-2-Clause

using System.Threading;
using System.Threading.Tasks;

namespace EnsembleWeather;

/// <summary>
/// A store for revalidatable responses: the SDK's local <c>ETag</c> cache. The default is an in-memory
/// <see cref="MemoryResponseCache"/>; implement this to share entries across clients or processes.
/// </summary>
/// <remarks>
/// Keys are opaque strings built from the full request URL and a SHA-256 hash of the API key (never the key itself),
/// so keyless and keyed entries never mix. Implementations must be thread-safe.
/// </remarks>
public interface IResponseCache
{
    /// <summary>Gets the entry stored under a key.</summary>
    /// <param name="key">The cache key.</param>
    /// <param name="cancellationToken">Cancels the lookup.</param>
    /// <returns>The entry, or <see langword="null"/> when there is none.</returns>
    ValueTask<CachedResponse?> GetAsync(string key, CancellationToken cancellationToken = default);

    /// <summary>Stores (or replaces) the entry under a key.</summary>
    /// <param name="key">The cache key.</param>
    /// <param name="response">The entry.</param>
    /// <param name="cancellationToken">Cancels the store.</param>
    /// <returns>A task that completes when stored.</returns>
    ValueTask SetAsync(string key, CachedResponse response, CancellationToken cancellationToken = default);

    /// <summary>Removes the entry under a key, if any.</summary>
    /// <param name="key">The cache key.</param>
    /// <param name="cancellationToken">Cancels the removal.</param>
    /// <returns>A task that completes when removed.</returns>
    ValueTask RemoveAsync(string key, CancellationToken cancellationToken = default);
}
