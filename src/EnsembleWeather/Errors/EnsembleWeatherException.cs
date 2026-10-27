// SPDX-License-Identifier: BSD-2-Clause

using System;

namespace EnsembleWeather;

/// <summary>
/// The base of every error the API or the transport produces. Catch a subclass for one category (for example
/// <see cref="OutsideCoverageException"/>), or this type for all of them.
/// </summary>
/// <remarks>
/// <para>
/// An API error carries the HTTP <see cref="StatusCode"/>, the API's <see cref="ErrorCode"/> (kept verbatim, even
/// when this SDK does not know it) and message, and the <see cref="RequestId"/> to quote to support. The category is
/// chosen by code first and by status when the code is unknown or the body is not JSON. An error the SDK cannot
/// categorise is an <see cref="EnsembleWeatherException"/> itself.
/// </para>
/// <para>
/// Usage errors (an invalid probability term, an unserved quantile pair) are found before anything is sent and are
/// thrown as <see cref="ArgumentException"/>, as .NET does for invalid arguments.
/// </para>
/// <para>The API key never appears in a message or in <see cref="Exception.ToString"/>.</para>
/// </remarks>
public class EnsembleWeatherException : Exception
{
    /// <summary>Initializes a new instance.</summary>
    public EnsembleWeatherException()
    {
    }

    /// <summary>Initializes a new instance with a message.</summary>
    /// <param name="message">The message.</param>
    public EnsembleWeatherException(string message)
        : base(message)
    {
    }

    /// <summary>Initializes a new instance with a message and the exception that caused it.</summary>
    /// <param name="message">The message.</param>
    /// <param name="innerException">The cause.</param>
    public EnsembleWeatherException(string message, Exception? innerException)
        : base(message, innerException)
    {
    }

    internal EnsembleWeatherException(ApiErrorDetails details, Exception? innerException = null)
        : base(details.Describe(), innerException)
    {
        StatusCode = details.StatusCode;
        ErrorCode = details.ErrorCode;
        ApiMessage = details.Message;
        RequestId = details.RequestId;
        RetryAfter = details.RetryAfter;
        RateLimit = details.RateLimit;
    }

    /// <summary>Gets the HTTP status, or <see langword="null"/> when no response arrived.</summary>
    public int? StatusCode { get; }

    /// <summary>
    /// Gets the API's error code (for example <c>OUTSIDE_COVERAGE</c>), verbatim; <see langword="null"/> when the
    /// body was not the API's JSON error.
    /// </summary>
    public string? ErrorCode { get; }

    /// <summary>Gets the API's own message, when the body carried one.</summary>
    public string? ApiMessage { get; }

    /// <summary>Gets the response's <c>X-Request-Id</c>, when it carried one.</summary>
    public string? RequestId { get; }

    /// <summary>
    /// Gets the server's <c>Retry-After</c> (on a <c>429</c>, and on a <c>503</c> that sends one). When it is over
    /// 60 s the SDK does not wait it out and throws with it set.
    /// </summary>
    public TimeSpan? RetryAfter { get; }

    /// <summary>Gets the rate-limit state the error response carried, when it carried one.</summary>
    public RateLimitState? RateLimit { get; }
}
