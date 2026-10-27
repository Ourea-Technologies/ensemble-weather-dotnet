// SPDX-License-Identifier: BSD-2-Clause

using System;
using System.Collections.Generic;

namespace EnsembleWeather;

/// <summary>
/// Options for a location or coordinate forecast. Every option left unset is omitted from the request, as is one
/// set to the API's default, so equal requests share one edge-cache entry.
/// </summary>
public sealed class ForecastOptions
{
    /// <summary>Gets or sets the unit system; <see langword="null"/> for the client's <see cref="EnsembleWeatherOptions.Units"/>.</summary>
    public UnitSystem? Units { get; set; }

    /// <summary>
    /// Gets or sets the hours of the hourly series past its first hour (6..360, clamped to the tier: Free 240, Pro
    /// 360). The API's default is 24.
    /// </summary>
    public int? MaxHours { get; set; }

    /// <summary>
    /// Gets or sets whether to serve the hourly series from the model run's init, past hours included, instead of
    /// from the current hour. The API's default is <see langword="false"/>.
    /// </summary>
    public bool? IncludePast { get; set; }

    /// <summary>Gets or sets whether to include per-model breakdowns (Pro). The API's default is <see langword="false"/>.</summary>
    public bool? Models { get; set; }

    /// <summary>
    /// Gets or sets whether to include individual member values (Free: the plume view; Pro: every variable). The
    /// API's default is <see langword="false"/>.
    /// </summary>
    public bool? Members { get; set; }

    /// <summary>
    /// Gets the named elevations to forecast too (ski areas; location forecasts only): <c>all</c>, or names from
    /// <c>location.elevations[].name</c>.
    /// </summary>
    public IList<string> Elevations { get; } = new List<string>();

    /// <summary>Gets or sets whether to include daily summaries. The API's default is <see langword="true"/>.</summary>
    public bool? Daily { get; set; }

    /// <summary>Gets or sets the number of daily rows (1..15; default: the tier's horizon).</summary>
    public int? MaxDays { get; set; }

    /// <summary>Gets or sets whether to include the hourly series. The API's default is <see langword="true"/>.</summary>
    public bool? Hourly { get; set; }

    /// <summary>Gets or sets a model run init time to pin (Pro), for historical queries.</summary>
    public DateTimeOffset? Run { get; set; }

    /// <summary>Gets the observation sources to allow in the nowcast blend (for example <c>snotel</c>).</summary>
    public IList<string> Sources { get; } = new List<string>();

    /// <summary>Gets the observation sources to exclude from the nowcast blend.</summary>
    public IList<string> ExcludeSources { get; } = new List<string>();

    /// <summary>
    /// Gets or sets whether the live nowcast layer is blended in. The API's default is <see langword="true"/>;
    /// <see langword="false"/> serves the pure, longer-cacheable base forecast.
    /// </summary>
    public bool? Nowcast { get; set; }

    /// <summary>
    /// Gets the probability questions to ask. The answers are in <see cref="ForecastResponse.Probabilities"/>, in
    /// this order (duplicates share one answer).
    /// </summary>
    public IList<ProbabilityTerm> Probabilities { get; } = new List<ProbabilityTerm>();

    /// <summary>
    /// Gets the window totals to ask for. The answers are in <see cref="ForecastResponse.WindowTotals"/>, in this
    /// order.
    /// </summary>
    public IList<WindowTotalTerm> WindowTotals { get; } = new List<WindowTotalTerm>();

    /// <summary>Gets the serve-path diagnostics to include (<c>provenance</c>, <c>ptype</c>).</summary>
    public IList<string> Diagnostics { get; } = new List<string>();

    /// <summary>Gets the "feels like" indices to serve (<c>nws</c>, the default; <c>utci</c> is not served yet).</summary>
    public IList<string> FeelsLike { get; } = new List<string>();
}
