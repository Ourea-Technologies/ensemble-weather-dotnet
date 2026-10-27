// SPDX-License-Identifier: BSD-2-Clause

using System;

namespace EnsembleWeather;

/// <summary>
/// No response arrived: DNS, TLS, a reset connection or the per-attempt timeout. Connection failures other than TLS verification were retried first.
/// </summary>
public sealed class ServiceConnectionException : EnsembleWeatherException
{
    /// <summary>Initializes a new instance.</summary>
    public ServiceConnectionException()
    {
    }

    /// <summary>Initializes a new instance with a message.</summary>
    /// <param name="message">The message.</param>
    public ServiceConnectionException(string message)
        : base(message)
    {
    }

    /// <summary>Initializes a new instance with a message and the exception that caused it.</summary>
    /// <param name="message">The message.</param>
    /// <param name="innerException">The cause.</param>
    public ServiceConnectionException(string message, Exception? innerException)
        : base(message, innerException)
    {
    }

    internal ServiceConnectionException(ApiErrorDetails details, Exception? innerException = null)
        : base(details, innerException)
    {
    }
}
