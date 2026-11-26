// SPDX-License-Identifier: BSD-2-Clause

namespace EnsembleWeather;

/// <summary>Options for <see cref="OureaClient.GetSnowTrendAsync(string, SnowTrendOptions, System.Threading.CancellationToken)"/>.</summary>
public sealed class SnowTrendOptions
{
    /// <summary>
    /// Gets or sets one of the location's named elevations (<c>location.elevations[].name</c>);
    /// <see langword="null"/> for its primary.
    /// </summary>
    public string? Elevation { get; set; }

    /// <summary>Gets or sets the unit system; <see langword="null"/> for the client's.</summary>
    public UnitSystem? Units { get; set; }
}
