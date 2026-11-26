// SPDX-License-Identifier: BSD-2-Clause

using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace EnsembleWeather;

/// <summary>
/// "Is the pass getting weather?" at the pass's own point and elevation: forecast quantities only, in the
/// response's units. <c>snow_24h</c> is the default pass page's own (<c>/locations/{state}/{slug}</c>, unpinned,
/// the nowcast on); the hours, snow spells, windows and chances are the pass's forecast from the current hour with
/// its members (<c>nowcast=false</c>, rebuilt once an hour), and every chance but the gust's is a share of those
/// members. It states weather, never a road verdict: no closure, chain or hazard calls of our own. The DOT's own
/// status (<c>official_status</c>), its road sensors (<c>road_sensors</c>) and its cameras (<c>cameras</c>) ride
/// beside the forecast, each attributed to the DOT and never mixed into a forecast number.
/// </summary>
public sealed class PassForecast
{
    /// <summary>
    /// The pass's elevation, in the response's length units (m, ft).
    /// </summary>
    [JsonPropertyName("elevation")]
    public double Elevation { get; init; }

    /// <summary>
    /// The pass's elevation in metres, whatever <c>units</c> is.
    /// </summary>
    [JsonPropertyName("elevation_m")]
    public double ElevationMeters { get; init; }

    /// <summary>
    /// The first hour of <c>hours</c> (the hour containing now); the 24 h and 48 h windows open here.
    /// </summary>
    [JsonPropertyName("from")]
    public DateTimeOffset From { get; init; }

    /// <summary>
    /// The next 24 h of snow at the pass: exactly the default forecast's own <c>snow_outlook.next_24h</c> and
    /// <c>snow_outlook.quantiles.next_24h</c> (unpinned, the nowcast on, the same cycle), so the pass page, the
    /// passes board and search state one number.
    /// </summary>
    [JsonPropertyName("snow_24h")]
    public PassSnow24h Snow24Hours { get; init; } = null!;

    /// <summary>
    /// Chances at the pass over the next <c>window_hours</c> hours, each the share of the forecast's members
    /// meeting the event (the <c>?prob=</c> machinery), except the gust chance (see its <c>method</c>). Thresholds
    /// are fixed product constants named in imperial (1 in, 6 in, 40 mph): they do not change with <c>units</c>;
    /// <c>threshold</c> states each in the response's units (25.4 mm, 152.4 mm, 17.9 m/s).
    /// </summary>
    [JsonPropertyName("chances")]
    public PassChances Chances { get; init; } = null!;

    /// <summary>
    /// Each spell of snow at the pass in the next 48 h, in time order; empty when none. A spell is a run of hours
    /// whose snowfall mean is at least 0.04 in (1 mm) an hour, with breaks of up to 2 h joined, and a total of at
    /// least a trace (0.1 in, 2.54 mm). It is a timing window, not a split of a storm into days (the storm-band
    /// ruling, is open).
    /// </summary>
    [JsonPropertyName("snow_windows")]
    public IReadOnlyList<PassSnowWindow> SnowWindows { get; init; } = [];

    /// <summary>
    /// The next 48 hours at the pass, from <c>from</c>, one per hour.
    /// </summary>
    [JsonPropertyName("hours")]
    public IReadOnlyList<PassHour> Hours { get; init; } = [];

    /// <summary>
    /// The quietest and the busiest stretch of weather at the pass in the next <c>horizon_hours</c>, by one fixed
    /// rule on the hours above: an hour has weather when it is snowing (<c>snowfall</c> at least 0.04 in, 1 mm),
    /// <c>freezing_on_wet_road</c>, or <c>gust_over_40mph</c>. <c>least_weather</c> is the longest run of hours
    /// without weather (earliest on a tie); <c>most_weather</c> the run of hours with weather holding the most
    /// snowfall, then the most freezing hours, then the highest gust. A description of the weather, never advice on
    /// whether to travel.
    /// </summary>
    [JsonPropertyName("windows")]
    public PassTravelWindows Windows { get; init; } = null!;

    /// <summary>
    /// The DOT's own status for the road over the pass -- chain controls and closures as the DOT publishes them,
    /// its wording verbatim. Never blended with the forecast and never a verdict of Ourea's: a client labels it as
    /// the DOT's ("Caltrans says", "WSDOT says") and links to <c>url</c>. Caltrans (CWWP2 chain controls, public
    /// domain) and WSDOT (Mountain Pass Conditions); CDOT and ODOT are built and switched on once their keys and
    /// terms are signed for. A pass whose road maps to no DOT feed has no <c>official_status</c>. Read every 5
    /// minutes; <c>stale</c> marks a <c>checked_at</c> more than 30 minutes old, and the block is still served so
    /// the client can say how old it is.
    /// </summary>
    [JsonPropertyName("official_status")]
    public PassOfficialStatus? OfficialStatus { get; init; }

    /// <summary>
    /// The state DOT road-weather stations nearest the pass, nearest first: at most 3, within 25 km, each with its
    /// newest reading in the last 3 h. What the sensors measured, never our forecast and never the DOT's official
    /// status. A reading the observation QC flagged is omitted. Omitted when no road station qualifies.
    /// </summary>
    [JsonPropertyName("road_sensors")]
    public IReadOnlyList<PassRoadSensor> RoadSensors { get; init; } = [];

    /// <summary>
    /// The DOT's own cameras at the pass, in the order the pass's curation lists them. The client loads
    /// <c>image_url</c> from the DOT itself (Ourea neither copies nor stores the image) and credits
    /// <c>attribution</c>. Omitted where the pass maps to no camera.
    /// </summary>
    [JsonPropertyName("cameras")]
    public IReadOnlyList<PassCamera> Cameras { get; init; } = [];
}
