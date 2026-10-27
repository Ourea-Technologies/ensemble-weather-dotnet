// SPDX-License-Identifier: BSD-2-Clause

using System.Text.Json.Serialization;

namespace EnsembleWeather;

/// <summary>
/// An hour's freezing level: below the surface, at a height, or above the top of the profile.
/// </summary>
/// <remarks>
/// The API sends one of three shapes, told apart by <see cref="Kind"/>. This type carries all three (the fields
/// that do not apply to a kind are <see langword="null"/>), so a kind added later still parses.
/// </remarks>
public sealed class FreezingLevel
{
    /// <summary>
    /// The kind: <c>below_surface</c>, <c>height</c> or <c>above_profile</c>.
    /// </summary>
    [JsonPropertyName("kind")]
    public string Kind { get; init; } = string.Empty;

    /// <summary>
    /// For <c>height</c>: the freezing level, in the request's length unit.
    /// </summary>
    [JsonPropertyName("height")]
    public double? Height { get; init; }

    /// <summary>
    /// For <c>height</c>: whether an above-freezing layer sits aloft over the level.
    /// </summary>
    [JsonPropertyName("above_freezing_layer_aloft")]
    public bool? AboveFreezingLayerAloft { get; init; }

    /// <summary>
    /// For <c>above_profile</c>: the top of the profile, in the request's length unit; the level is above it.
    /// </summary>
    [JsonPropertyName("top")]
    public double? Top { get; init; }
}
