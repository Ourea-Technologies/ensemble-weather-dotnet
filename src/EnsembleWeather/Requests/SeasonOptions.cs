// SPDX-License-Identifier: BSD-2-Clause

namespace EnsembleWeather;

/// <summary>Options for <see cref="OureaClient.GetSeasonAsync(string, SeasonOptions, System.Threading.CancellationToken)"/>.</summary>
public sealed class SeasonOptions
{
    /// <summary>
    /// Gets or sets the water year (October to September), named by the year it ends in (2027 or later);
    /// <see langword="null"/> for the current one.
    /// </summary>
    public int? WaterYear { get; set; }

    /// <summary>Gets or sets whether to include the day-by-day record (<see cref="SeasonResponse.Daily"/>).</summary>
    public bool IncludeDaily { get; set; }

    /// <summary>Gets or sets the unit system; <see langword="null"/> for the client's.</summary>
    public UnitSystem? Units { get; set; }
}
