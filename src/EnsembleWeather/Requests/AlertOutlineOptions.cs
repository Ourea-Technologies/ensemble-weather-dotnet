// SPDX-License-Identifier: BSD-2-Clause

using System.Collections.Generic;

namespace EnsembleWeather;

/// <summary>Filters for <see cref="OureaClient.GetAlertOutlinesAsync"/>.</summary>
public sealed class AlertOutlineOptions
{
    /// <summary>
    /// Gets the alert classes to include (<c>class=</c>, sent as one sorted comma list); empty for every class.
    /// </summary>
    public IList<AlertClass> Classes { get; } = new List<AlertClass>();

    /// <summary>
    /// Gets or sets a box: only alerts whose bounding box touches it. Features are served whole, not clipped.
    /// </summary>
    public BoundingBox? BoundingBox { get; set; }
}
