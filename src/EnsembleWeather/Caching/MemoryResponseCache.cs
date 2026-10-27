// SPDX-License-Identifier: BSD-2-Clause

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace EnsembleWeather;

/// <summary>
/// The default <see cref="IResponseCache"/>: a thread-safe, in-memory LRU bounded by entry count and total body
/// bytes (256 entries and 32 MiB unless set otherwise).
/// </summary>
public sealed class MemoryResponseCache : IResponseCache
{
    /// <summary>The default entry bound.</summary>
    public const int DefaultMaxEntries = 256;

    /// <summary>The default byte bound, 32 MiB.</summary>
    public const long DefaultMaxBytes = 32L * 1024 * 1024;

    private readonly object _gate = new();
    private readonly Dictionary<string, LinkedListNode<KeyValuePair<string, CachedResponse>>> _map = new(StringComparer.Ordinal);
    private readonly LinkedList<KeyValuePair<string, CachedResponse>> _order = new();
    private long _bytes;

    /// <summary>Initializes a cache with the given bounds.</summary>
    /// <param name="maxEntries">The most entries kept.</param>
    /// <param name="maxBytes">The most body bytes kept, in all.</param>
    /// <exception cref="ArgumentOutOfRangeException">A bound is not positive.</exception>
    public MemoryResponseCache(int maxEntries = DefaultMaxEntries, long maxBytes = DefaultMaxBytes)
    {
        if (maxEntries <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(maxEntries), "The entry bound must be positive.");
        }

        if (maxBytes <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(maxBytes), "The byte bound must be positive.");
        }

        MaxEntries = maxEntries;
        MaxBytes = maxBytes;
    }

    /// <summary>Gets the most entries kept.</summary>
    public int MaxEntries { get; }

    /// <summary>Gets the most body bytes kept.</summary>
    public long MaxBytes { get; }

    /// <summary>Gets the number of entries now stored.</summary>
    public int Count
    {
        get
        {
            lock (_gate)
            {
                return _map.Count;
            }
        }
    }

    /// <inheritdoc/>
    public ValueTask<CachedResponse?> GetAsync(string key, CancellationToken cancellationToken = default)
    {
        lock (_gate)
        {
            if (!_map.TryGetValue(key, out LinkedListNode<KeyValuePair<string, CachedResponse>>? node))
            {
                return new ValueTask<CachedResponse?>((CachedResponse?)null);
            }

            _order.Remove(node);
            _order.AddFirst(node);
            return new ValueTask<CachedResponse?>(node.Value.Value);
        }
    }

    /// <inheritdoc/>
    public ValueTask SetAsync(string key, CachedResponse response, CancellationToken cancellationToken = default)
    {
        if (response is null)
        {
            throw new ArgumentNullException(nameof(response));
        }

        lock (_gate)
        {
            RemoveLocked(key);
            if (response.Body.LongLength > MaxBytes)
            {
                return default;
            }

            _map[key] = _order.AddFirst(new KeyValuePair<string, CachedResponse>(key, response));
            _bytes += response.Body.LongLength;
            while (_map.Count > MaxEntries || _bytes > MaxBytes)
            {
                RemoveLocked(_order.Last!.Value.Key);
            }
        }

        return default;
    }

    /// <inheritdoc/>
    public ValueTask RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        lock (_gate)
        {
            RemoveLocked(key);
        }

        return default;
    }

    private void RemoveLocked(string key)
    {
        if (_map.TryGetValue(key, out LinkedListNode<KeyValuePair<string, CachedResponse>>? node))
        {
            _order.Remove(node);
            _map.Remove(key);
            _bytes -= node.Value.Value.Body.LongLength;
        }
    }
}
