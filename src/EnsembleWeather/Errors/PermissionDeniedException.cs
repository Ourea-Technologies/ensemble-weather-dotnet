// SPDX-License-Identifier: BSD-2-Clause

using System;

namespace EnsembleWeather;

/// <summary>
/// The caller's tier does not allow the request (<c>PRO_REQUIRED</c>, or HTTP 403), for example a coordinate forecast without a Pro key.
/// </summary>
public sealed class PermissionDeniedException : EnsembleWeatherException
{
    /// <summary>Initializes a new instance.</summary>
    public PermissionDeniedException()
    {
    }

    /// <summary>Initializes a new instance with a message.</summary>
    /// <param name="message">The message.</param>
    public PermissionDeniedException(string message)
        : base(message)
    {
    }

    /// <summary>Initializes a new instance with a message and the exception that caused it.</summary>
    /// <param name="message">The message.</param>
    /// <param name="innerException">The cause.</param>
    public PermissionDeniedException(string message, Exception? innerException)
        : base(message, innerException)
    {
    }

    internal PermissionDeniedException(ApiErrorDetails details, Exception? innerException = null)
        : base(details, innerException)
    {
    }
}
