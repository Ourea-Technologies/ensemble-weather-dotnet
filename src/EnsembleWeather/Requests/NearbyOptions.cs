// SPDX-License-Identifier: BSD-2-Clause

namespace EnsembleWeather;

/// <summary>Options for nearby-location lookups.</summary>
public sealed class NearbyOptions
{
    /// <summary>Gets or sets the number of neighbours wanted (1..20). The API's default is 3.</summary>
    public int? Limit { get; set; }

    /// <summary>Gets or sets the search radius in kilometres (up to 100). The API's default is 50.</summary>
    public double? RadiusKm { get; set; }

    /// <summary>Gets or sets the unit system for elevations; <see langword="null"/> for the client's.</summary>
    public UnitSystem? Units { get; set; }
}
