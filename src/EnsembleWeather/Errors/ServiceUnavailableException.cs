// SPDX-License-Identifier: BSD-2-Clause

using System;

namespace EnsembleWeather;

/// <summary>
/// The service could not answer (<c>FORECAST_UNAVAILABLE</c>, <c>TIMEOUT</c>, <c>INTERNAL_ERROR</c>, <c>DATABASE_ERROR</c>, <c>AUTH_UNAVAILABLE</c>, or HTTP 5xx). 502, 503 and 504 were retried first.
/// </summary>
public sealed class ServiceUnavailableException : EnsembleWeatherException
{
    /// <summary>Initializes a new instance.</summary>
    public ServiceUnavailableException()
    {
    }

    /// <summary>Initializes a new instance with a message.</summary>
    /// <param name="message">The message.</param>
    public ServiceUnavailableException(string message)
        : base(message)
    {
    }

    /// <summary>Initializes a new instance with a message and the exception that caused it.</summary>
    /// <param name="message">The message.</param>
    /// <param name="innerException">The cause.</param>
    public ServiceUnavailableException(string message, Exception? innerException)
        : base(message, innerException)
    {
    }

    internal ServiceUnavailableException(ApiErrorDetails details, Exception? innerException = null)
        : base(details, innerException)
    {
    }
}
