// SPDX-License-Identifier: BSD-2-Clause

using System.Collections.Generic;

namespace EnsembleWeather;

/// <summary>Options for current conditions at a location or coordinates.</summary>
public sealed class CurrentConditionsOptions
{
    /// <summary>Gets or sets the unit system; <see langword="null"/> for the client's.</summary>
    public UnitSystem? Units { get; set; }

    /// <summary>Gets the observation sources to include (for example <c>snotel</c>, <c>ski_resort</c>).</summary>
    public IList<string> Sources { get; } = new List<string>();

    /// <summary>Gets the observation sources to exclude.</summary>
    public IList<string> ExcludeSources { get; } = new List<string>();

    /// <summary>
    /// Gets or sets whether to add <c>nowcast_hours</c>, the obs-blended band to overlay on a cached
    /// <c>nowcast=false</c> forecast.
    /// </summary>
    public bool IncludeNowcastHours { get; set; }
}
