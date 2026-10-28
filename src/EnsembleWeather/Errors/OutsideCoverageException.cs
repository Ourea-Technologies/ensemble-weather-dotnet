// SPDX-License-Identifier: BSD-2-Clause

using System;

namespace EnsembleWeather;

/// <summary>
/// The point is outside the forecast coverage (<c>OUTSIDE_COVERAGE</c>, a 404): the contiguous United States, over land.
/// </summary>
public sealed class OutsideCoverageException : EnsembleWeatherException
{
    /// <summary>Initializes a new instance.</summary>
    public OutsideCoverageException()
    {
    }

    /// <summary>Initializes a new instance with a message.</summary>
    /// <param name="message">The message.</param>
    public OutsideCoverageException(string message)
        : base(message)
    {
    }

    /// <summary>Initializes a new instance with a message and the exception that caused it.</summary>
    /// <param name="message">The message.</param>
    /// <param name="innerException">The cause.</param>
    public OutsideCoverageException(string message, Exception? innerException)
        : base(message, innerException)
    {
    }

    internal OutsideCoverageException(ApiErrorDetails details, Exception? innerException = null)
        : base(details, innerException)
    {
    }
}
