// SPDX-License-Identifier: BSD-2-Clause

using System.Collections.Concurrent;

namespace EnsembleWeather;

/// <summary>Remembers which operations have reported a deprecation, so each reports once per process.</summary>
internal sealed class DeprecationRegistry
{
    private readonly ConcurrentDictionary<string, bool> _seen = new();

    public static DeprecationRegistry Process { get; } = new();

    public bool FirstTime(string operation) => _seen.TryAdd(operation, true);
}
