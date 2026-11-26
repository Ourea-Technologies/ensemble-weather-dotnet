// SPDX-License-Identifier: BSD-2-Clause

using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace EnsembleWeather;

/// <summary>
/// The DOT's own status for the road over the pass -- chain controls and closures as the DOT publishes them, its
/// wording verbatim. Never blended with the forecast and never a verdict of Ourea's: a client labels it as the
/// DOT's ("Caltrans says", "WSDOT says") and links to <c>url</c>. Caltrans (CWWP2 chain controls, public domain)
/// and WSDOT (Mountain Pass Conditions); CDOT and ODOT are built and switched on once their keys and terms are
/// signed for. A pass whose road maps to no DOT feed has no <c>official_status</c>. Read every 5 minutes;
/// <c>stale</c> marks a <c>checked_at</c> more than 30 minutes old, and the block is still served so the client can
/// say how old it is.
/// </summary>
public sealed class PassOfficialStatus
{
    /// <summary>
    /// The DOT feed. added <c>wsdot</c>, <c>cdot</c> and <c>odot</c>; a client shows <c>attribution</c> and should
    /// accept a source it does not know.
    /// </summary>
    [JsonPropertyName("source")]
    public string Source { get; init; } = string.Empty;

    /// <summary>
    /// Who to credit, as the client shows it.
    /// </summary>
    [JsonPropertyName("attribution")]
    public string Attribution { get; init; } = string.Empty;

    /// <summary>
    /// The DOT's own page for this road, to link out to.
    /// </summary>
    [JsonPropertyName("url")]
    public string Url { get; init; } = string.Empty;

    /// <summary>
    /// How fresh the DOT's feed behind these segments was at Ourea's last read: the time the DOT wrote the file
    /// Ourea last read (Caltrans rewrites it every minute; Ourea reads it every 5), and the older of two when the
    /// segments span two DOT districts. A feed that does not say when it was written (WSDOT, CDOT, ODOT) is as
    /// fresh as Ourea's last successful read of it; each segment's <c>updated_at</c> is the DOT's own time for its
    /// report.
    /// </summary>
    [JsonPropertyName("checked_at")]
    public DateTimeOffset CheckedAt { get; init; }

    /// <summary>
    /// <c>checked_at</c> is more than 30 minutes before <c>as_of</c>.
    /// </summary>
    [JsonPropertyName("stale")]
    public bool Stale { get; init; }

    /// <summary>
    /// The DOT's own restriction, as an enum, for ordering and colour only: the words a client shows are the DOT's
    /// <c>status</c> and <c>status_text</c>. Caltrans's codes map one to one: R-0 (<c>none</c>, "No chain controls
    /// are in effect at this time."), R-1, R-2, R-3 and ROAD CLOSED (<c>closed</c>). WSDOT, CDOT and ODOT publish
    /// words, not codes, read conservatively: a closure is <c>closed</c>; chains required or Colorado's Chain Law
    /// is <c>chains</c>; traction tires required or Colorado's Traction Law is <c>traction</c>; "No restrictions"
    /// is <c>none</c>; any other restriction or advice, or wording not recognised, is <c>advisory</c> (WSDOT's
    /// "Traction Tires Advised, Oversize Vehicles Prohibited."). Levels order least to most restrictive within one
    /// DOT; a pass has one DOT. A client should treat a level it does not know as <c>advisory</c>.
    /// </summary>
    [JsonPropertyName("level")]
    public PassStatusLevel Level { get; init; }

    /// <summary>
    /// The DOT's chain-control and closure points on the road over the pass, each direction, in the pass's curated
    /// order. <c>level</c> is the most restrictive of theirs.
    /// </summary>
    [JsonPropertyName("segments")]
    public IReadOnlyList<PassStatusSegment> Segments { get => field ?? []; init; } = [];
}
