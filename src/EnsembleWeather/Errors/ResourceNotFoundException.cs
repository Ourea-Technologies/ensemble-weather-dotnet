// SPDX-License-Identifier: BSD-2-Clause

using System;

namespace EnsembleWeather;

/// <summary>
/// The location, run or resource does not exist (<c>LOCATION_NOT_FOUND</c>, <c>MODEL_RUN_NOT_FOUND</c>, <c>NOT_FOUND</c>, <c>GONE</c>, or HTTP 404/410).
/// </summary>
public sealed class ResourceNotFoundException : EnsembleWeatherException
{
    /// <summary>Initializes a new instance.</summary>
    public ResourceNotFoundException()
    {
    }

    /// <summary>Initializes a new instance with a message.</summary>
    /// <param name="message">The message.</param>
    public ResourceNotFoundException(string message)
        : base(message)
    {
    }

    /// <summary>Initializes a new instance with a message and the exception that caused it.</summary>
    /// <param name="message">The message.</param>
    /// <param name="innerException">The cause.</param>
    public ResourceNotFoundException(string message, Exception? innerException)
        : base(message, innerException)
    {
    }

    internal ResourceNotFoundException(ApiErrorDetails details, Exception? innerException = null)
        : base(details, innerException)
    {
    }
}
