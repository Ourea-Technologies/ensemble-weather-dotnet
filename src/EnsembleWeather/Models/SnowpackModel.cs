// SPDX-License-Identifier: BSD-2-Clause

using System.Text.Json.Serialization;

namespace EnsembleWeather;

/// <summary>
/// The snowpack model that made a run.
/// </summary>
public sealed class SnowpackModel
{
    /// <summary>
    /// The name.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; init; } = string.Empty;

    /// <summary>
    /// Who makes the model.
    /// </summary>
    [JsonPropertyName("by")]
    public string By { get; init; } = string.Empty;

    /// <summary>
    /// The model's licence.
    /// </summary>
    [JsonPropertyName("license")]
    public string License { get; init; } = string.Empty;

    /// <summary>
    /// The build that ran: the commits of Ourea's SNOWPACK and MeteoIO source it was built from, as <c>snowpack
    /// &lt;commit&gt; meteoio &lt;commit&gt;</c>. Absent on a run whose build carried no such record.
    /// </summary>
    [JsonPropertyName("version")]
    public string? Version { get; init; }
}
