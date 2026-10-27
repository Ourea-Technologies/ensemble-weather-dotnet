// SPDX-License-Identifier: BSD-2-Clause

using System;
using System.Collections.Generic;
using System.Globalization;

namespace EnsembleWeather;

/// <summary>What an error response said, before it becomes an exception of its category.</summary>
internal sealed class ApiErrorDetails
{
    private static readonly Dictionary<string, Func<ApiErrorDetails, EnsembleWeatherException>> ByCode =
        new(StringComparer.Ordinal)
        {
            ["INVALID_API_KEY"] = d => new AuthenticationFailedException(d),
            ["UNAUTHORIZED"] = d => new AuthenticationFailedException(d),
            ["PRO_REQUIRED"] = d => new PermissionDeniedException(d),
            ["FORBIDDEN"] = d => new PermissionDeniedException(d),
            ["LOCATION_NOT_FOUND"] = d => new ResourceNotFoundException(d),
            ["MODEL_RUN_NOT_FOUND"] = d => new ResourceNotFoundException(d),
            ["NOT_FOUND"] = d => new ResourceNotFoundException(d),
            ["GONE"] = d => new ResourceNotFoundException(d),
            ["BAD_REQUEST"] = d => new InvalidRequestException(d),
            ["INVALID_COORDINATES"] = d => new InvalidRequestException(d),
            ["INVALID_ELEVATION"] = d => new InvalidRequestException(d),
            ["INVALID_MAX_HOURS"] = d => new InvalidRequestException(d),
            ["ELEVATION_REQUIRED"] = d => new InvalidRequestException(d),
            ["AMBIGUOUS"] = d => new InvalidRequestException(d),
            ["FEELS_LIKE_UNAVAILABLE"] = d => new InvalidRequestException(d),
            ["OUTSIDE_COVERAGE"] = d => new OutsideCoverageException(d),
            ["TOO_MANY_REQUESTS"] = d => new RateLimitExceededException(d),
            ["FORECAST_UNAVAILABLE"] = d => new ServiceUnavailableException(d),
            ["TIMEOUT"] = d => new ServiceUnavailableException(d),
            ["INTERNAL_ERROR"] = d => new ServiceUnavailableException(d),
            ["DATABASE_ERROR"] = d => new ServiceUnavailableException(d),
        };

    public int StatusCode { get; init; }

    public string? ErrorCode { get; init; }

    public string? Message { get; init; }

    public string? RequestId { get; init; }

    public TimeSpan? RetryAfter { get; init; }

    public RateLimitState? RateLimit { get; init; }

    /// <summary>Maps the error to its category: by code first, by status when the code is unknown.</summary>
    public EnsembleWeatherException ToException()
    {
        if (ErrorCode is not null && ByCode.TryGetValue(ErrorCode, out Func<ApiErrorDetails, EnsembleWeatherException>? create))
        {
            return create(this);
        }

        return StatusCode switch
        {
            400 => new InvalidRequestException(this),
            401 => new AuthenticationFailedException(this),
            403 => new PermissionDeniedException(this),
            404 or 410 => new ResourceNotFoundException(this),
            429 => new RateLimitExceededException(this),
            >= 500 and <= 599 => new ServiceUnavailableException(this),
            _ => new EnsembleWeatherException(this),
        };
    }

    public string Describe()
    {
        string text = string.IsNullOrEmpty(Message) ? "The Ourea API returned an error." : Message!;
        string code = ErrorCode is null ? string.Empty : ErrorCode + ", ";
        string request = RequestId is null ? string.Empty : ", request " + RequestId;
        return string.Format(CultureInfo.InvariantCulture, "{0} ({1}HTTP {2}{3})", text, code, StatusCode, request);
    }
}
