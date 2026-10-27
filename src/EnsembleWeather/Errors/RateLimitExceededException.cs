// SPDX-License-Identifier: BSD-2-Clause

using System;

namespace EnsembleWeather;

/// <summary>
/// A rate budget refused the request (<c>TOO_MANY_REQUESTS</c>, HTTP 429). <see cref="EnsembleWeatherException.RetryAfter"/> says how long to wait; the SDK already retried when the wait was 60 s or less.
/// </summary>
public sealed class RateLimitExceededException : EnsembleWeatherException
{
    /// <summary>Initializes a new instance.</summary>
    public RateLimitExceededException()
    {
    }

    /// <summary>Initializes a new instance with a message.</summary>
    /// <param name="message">The message.</param>
    public RateLimitExceededException(string message)
        : base(message)
    {
    }

    /// <summary>Initializes a new instance with a message and the exception that caused it.</summary>
    /// <param name="message">The message.</param>
    /// <param name="innerException">The cause.</param>
    public RateLimitExceededException(string message, Exception? innerException)
        : base(message, innerException)
    {
    }

    internal RateLimitExceededException(ApiErrorDetails details, Exception? innerException = null)
        : base(details, innerException)
    {
    }
}
