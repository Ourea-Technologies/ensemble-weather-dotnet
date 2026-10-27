// SPDX-License-Identifier: BSD-2-Clause

namespace EnsembleWeather;

/// <summary>
/// What the SDK learned from the HTTP response behind a result.
/// </summary>
public sealed class ResponseMetadata
{
    internal static readonly ResponseMetadata None = new(200, null, null, false, null, null);

    internal ResponseMetadata(
        int statusCode,
        RateLimitState? rateLimit,
        string? requestId,
        bool revalidated,
        string? entityTag,
        DeprecationNotice? deprecation)
    {
        StatusCode = statusCode;
        RateLimit = rateLimit;
        RequestId = requestId;
        Revalidated = revalidated;
        EntityTag = entityTag;
        Deprecation = deprecation;
    }

    /// <summary>
    /// Gets the HTTP status of the final response: <c>200</c>, or <c>304</c> when the cached body was revalidated.
    /// </summary>
    public int StatusCode { get; }

    /// <summary>
    /// Gets the rate-limit state the response carried, or <see langword="null"/> when it carried none (a response a
    /// shared cache may store, such as an anonymous forecast, carries no <c>RateLimit-*</c> headers).
    /// </summary>
    public RateLimitState? RateLimit { get; }

    /// <summary>Gets the <c>X-Request-Id</c> the server assigned, when it sent one.</summary>
    public string? RequestId { get; }

    /// <summary>
    /// Gets whether the body came from the SDK's cache after the server answered <c>304 Not Modified</c> to the
    /// stored <c>ETag</c>.
    /// </summary>
    public bool Revalidated { get; }

    /// <summary>Gets the representation's <c>ETag</c>, byte for byte as the server sent it.</summary>
    public string? EntityTag { get; }

    /// <summary>
    /// Gets the deprecation notice (<c>Deprecation</c>, <c>Sunset</c>, <c>Link rel="deprecation"</c>) when the
    /// response carried one.
    /// </summary>
    public DeprecationNotice? Deprecation { get; }
}
