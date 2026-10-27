// SPDX-License-Identifier: BSD-2-Clause

using System.Text.Json.Serialization;

namespace EnsembleWeather.Serialization;

/// <summary>The API's error body, <c>{"error": "&lt;CODE&gt;", "message": "..."}</c>.</summary>
internal sealed class ErrorBody
{
    [JsonPropertyName("error")]
    public string? Error { get; init; }

    [JsonPropertyName("message")]
    public string? Message { get; init; }
}
