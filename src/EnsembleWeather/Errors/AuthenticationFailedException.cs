// SPDX-License-Identifier: BSD-2-Clause

using System;

namespace EnsembleWeather;

/// <summary>
/// The API key was refused (<c>INVALID_API_KEY</c>, <c>UNAUTHORIZED</c>, or HTTP 401). Send a valid key, or none for anonymous Free access.
/// </summary>
public sealed class AuthenticationFailedException : EnsembleWeatherException
{
    /// <summary>Initializes a new instance.</summary>
    public AuthenticationFailedException()
    {
    }

    /// <summary>Initializes a new instance with a message.</summary>
    /// <param name="message">The message.</param>
    public AuthenticationFailedException(string message)
        : base(message)
    {
    }

    /// <summary>Initializes a new instance with a message and the exception that caused it.</summary>
    /// <param name="message">The message.</param>
    /// <param name="innerException">The cause.</param>
    public AuthenticationFailedException(string message, Exception? innerException)
        : base(message, innerException)
    {
    }

    internal AuthenticationFailedException(ApiErrorDetails details, Exception? innerException = null)
        : base(details, innerException)
    {
    }
}
