// SPDX-License-Identifier: BSD-2-Clause

namespace EnsembleWeather;

/// <summary>Which of a place's points <see cref="OureaClient.ExplorePageAsync"/> ranks (<c>points=</c>).</summary>
public enum ExplorePoints
{
    /// <summary>Each place's own point (<c>primary</c>, the API's default).</summary>
    Primary,

    /// <summary>Each named elevation too (<c>all</c>): a ski area's base, mid and summit rank on their own.</summary>
    All,
}
