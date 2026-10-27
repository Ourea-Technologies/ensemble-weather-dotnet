// SPDX-License-Identifier: BSD-2-Clause

using System;

namespace EnsembleWeather;

/// <summary>
/// The rate-limit state a response reported in its <c>RateLimit-Limit</c>, <c>RateLimit-Remaining</c>,
/// <c>RateLimit-Reset</c> and <c>RateLimit-Policy</c> headers. Read it rather than hard-coding limits: they differ by
/// tier and change over time.
/// </summary>
public sealed class RateLimitState
{
    internal RateLimitState(long? limit, long? remaining, TimeSpan? reset, string? policy)
    {
        Limit = limit;
        Remaining = remaining;
        Reset = reset;
        Policy = policy;
    }

    /// <summary>Gets the size of the budget this response counted against.</summary>
    public long? Limit { get; }

    /// <summary>Gets the requests left before the binding limit refuses.</summary>
    public long? Remaining { get; }

    /// <summary>Gets the time until the binding limit is whole again.</summary>
    public TimeSpan? Reset { get; }

    /// <summary>Gets the caller's budgets, for example <c>60;w=60, 10000;w=86400</c>.</summary>
    public string? Policy { get; }
}
