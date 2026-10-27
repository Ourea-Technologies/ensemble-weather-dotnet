// SPDX-License-Identifier: BSD-2-Clause

using System;

namespace EnsembleWeather;

/// <summary>
/// The server refused the request as malformed (<c>BAD_REQUEST</c>, <c>INVALID_COORDINATES</c>, <c>INVALID_ELEVATION</c>, <c>INVALID_MAX_HOURS</c>, <c>ELEVATION_REQUIRED</c>, <c>AMBIGUOUS</c>, <c>FEELS_LIKE_UNAVAILABLE</c>, or HTTP 400).
/// </summary>
public sealed class InvalidRequestException : EnsembleWeatherException
{
    /// <summary>Initializes a new instance.</summary>
    public InvalidRequestException()
    {
    }

    /// <summary>Initializes a new instance with a message.</summary>
    /// <param name="message">The message.</param>
    public InvalidRequestException(string message)
        : base(message)
    {
    }

    /// <summary>Initializes a new instance with a message and the exception that caused it.</summary>
    /// <param name="message">The message.</param>
    /// <param name="innerException">The cause.</param>
    public InvalidRequestException(string message, Exception? innerException)
        : base(message, innerException)
    {
    }

    internal InvalidRequestException(ApiErrorDetails details, Exception? innerException = null)
        : base(details, innerException)
    {
    }
}
