// SPDX-License-Identifier: BSD-2-Clause

using System.Text.Json.Serialization;

namespace EnsembleWeather;

/// <summary>
/// The base of every object an operation returns: the response body plus <see cref="Metadata"/> about the HTTP
/// exchange that produced it (rate-limit state, cache revalidation, request id, deprecation).
/// </summary>
public abstract class ApiResult
{
    /// <summary>
    /// Gets what the SDK learned from the HTTP response: rate-limit state, whether a cached body was revalidated
    /// with a <c>304</c>, the request id and any deprecation notice. Not part of the JSON body.
    /// </summary>
    [JsonIgnore]
    public ResponseMetadata Metadata { get; internal set; } = ResponseMetadata.None;
}
