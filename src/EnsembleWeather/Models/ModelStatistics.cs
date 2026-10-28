// SPDX-License-Identifier: BSD-2-Clause

using System.Text.Json.Serialization;

namespace EnsembleWeather;

/// <summary>
/// One model's statistics at one hour.
/// </summary>
public sealed class ModelStatistics
{
    /// <summary>
    /// The model id, for example &lt;c&gt;gefs&lt;/c&gt;.
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; init; } = string.Empty;

    /// <summary>
    /// Absent when this model published nothing for the hour; never 0. The global ensembles publish on a 6-hourly
    /// step, so at five lead hours out of six a contributing model has nothing to report.
    /// </summary>
    [JsonPropertyName("temperature")]
    public Quantiles? Temperature { get; init; }

    /// <summary>
    /// Always absent for <c>aigefs</c>, which publishes no 2 m humidity and no surface pressure: the blended
    /// dewpoint, relative humidity and feels-like are pooled from the models that publish one, their weights
    /// renormalised over them.
    /// </summary>
    [JsonPropertyName("dewpoint")]
    public Quantiles? Dewpoint { get; init; }

    /// <summary>
    /// Absent when this model published nothing for the hour; never 0.
    /// </summary>
    [JsonPropertyName("precipitation")]
    public Quantiles? Precipitation { get; init; }

    /// <summary>
    /// Absent when this model published nothing for the hour; never 0.
    /// </summary>
    [JsonPropertyName("wind_speed")]
    public Quantiles? WindSpeed { get; init; }

    /// <summary>
    /// The model's own wind direction as a circular distribution; absent when this model published nothing for the
    /// hour.
    /// </summary>
    [JsonPropertyName("wind_direction")]
    public CircularStatistics? WindDirection { get; init; }

    /// <summary>
    /// Snow level, in the request's length unit.
    /// </summary>
    [JsonPropertyName("snow_level")]
    public Quantiles? SnowLevel { get; init; }

    /// <summary>
    /// Snow-to-liquid ratio.
    /// </summary>
    [JsonPropertyName("slr")]
    public Quantiles? SnowToLiquidRatio { get; init; }

    /// <summary>
    /// Snowfall.
    /// </summary>
    [JsonPropertyName("snowfall")]
    public Quantiles? Snowfall { get; init; }

    /// <summary>
    /// <c>True</c> when this model did not publish at this lead and the stats above were interpolated from its two
    /// bracketing native steps, so the blend pools the same set of contributors at every lead. Omitted (absent) when
    /// the model published at this hour, which is the ordinary case — treat absence as <c>false</c>. The global
    /// ensembles publish 6-hourly and the convection-allowing model is hourly, so at five lead hours out of six the
    /// 6-hourly models' rows carry this flag. NOTE — <c>models=true</c> and <c>members=true</c> deliberately disagree
    /// here, and the disagreement is the honest answer to two different questions. A model's per-hour STATS can be
    /// interpolated to a lead it did not publish at, because a distribution between two distributions is a
    /// distribution. Its per-hour MEMBERS cannot: member 7 at lead 3 is a specific draw from a specific run, and
    /// interpolating member 7's two neighbouring values would invent a trajectory that ensemble never produced and
    /// would break the member-to-member correlation every spaghetti plot and every ECC reordering depends on. So the
    /// same hour serves <c>model_stats.aifs.temperature</c> with <c>interpolated: true</c> and
    /// <c>members.aifs[*].temperature = null</c>. The stats are the blend's input; the members are the raw draws.
    /// </summary>
    [JsonPropertyName("interpolated")]
    public bool? Interpolated { get; init; }
}
