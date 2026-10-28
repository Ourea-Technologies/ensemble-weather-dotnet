// SPDX-License-Identifier: BSD-2-Clause

using System.Text.Json.Serialization;

namespace EnsembleWeather.Serialization;

/// <summary>
/// The API's error body, <c>{"error": "&lt;CODE&gt;", "message": "...", "request_id": "...", "doc_url": "..."}</c>
/// (with <c>param</c> on a 400 about one parameter, <c>nearest_location</c> on some <c>PRO_REQUIRED</c>s).
/// </summary>
internal sealed class ErrorBody
{
    [JsonPropertyName("error")]
    public string? Error { get; init; }

    [JsonPropertyName("message")]
    public string? Message { get; init; }

    [JsonPropertyName("request_id")]
    public string? RequestId { get; init; }

    [JsonPropertyName("param")]
    public string? Param { get; init; }

    [JsonPropertyName("doc_url")]
    public string? DocUrl { get; init; }

    [JsonPropertyName("nearest_location")]
    public NearestLocation? NearestLocation { get; init; }
}
