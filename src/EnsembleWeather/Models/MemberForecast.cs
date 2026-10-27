// SPDX-License-Identifier: BSD-2-Clause

using System.Text.Json.Serialization;

namespace EnsembleWeather;

/// <summary>
/// One ensemble member's values at a single forecast hour. All physical fields are in the caller's unit system
/// (metric or imperial) — temperature in °C/°F, precipitation &amp; snowfall in mm/in, wind speed in m/s or mph,
/// snow level in m/ft. <c>slr</c> is unitless.
/// </summary>
public sealed class MemberForecast
{
    /// <summary>
    /// The member's position on its model's ensemble axis, from 0: stable across the hours of a response and across
    /// cycles, and numbered per model (a member is <c>(model, member_id)</c>). 0 is the control for <c>aifs</c> (cf),
    /// <c>aigefs</c> (mem000) and <c>gefs</c> (gec00); <c>ifs_ens</c> has no control in ECMWF open data, so its 0..49
    /// are perturbed members 1..50; <c>rrfs</c> is the one deterministic run, 0. Serializes as a JSON number (changed
    /// from string in 0.30.x — ALLOC-5).
    /// </summary>
    [JsonPropertyName("member_id")]
    public int MemberId { get; init; }

    /// <summary>
    /// <c>null</c> when this member published nothing at this hour. Became nullable in 0.480.0 — previously a
    /// fabricated <c>0.0</c>.
    /// </summary>
    [JsonPropertyName("temperature")]
    public double? Temperature { get; init; }

    /// <summary>
    /// <c>null</c> when this member published nothing at this hour. A measured <c>0.0</c> (a genuinely dry member) is
    /// still served as <c>0.0</c> — the two are different claims. The value is the TOTAL over the
    /// <c>precipitation_hours</c> hours ending at this hour — a 6-hourly model's whole 6-hour amount at its native
    /// step, and <c>null</c> between — so summing a member's <c>precipitation</c> over time gives its accumulated
    /// total.
    /// </summary>
    [JsonPropertyName("precipitation")]
    public double? Precipitation { get; init; }

    /// <summary>
    /// How many hours <c>precipitation</c> (and <c>snowfall</c>) covers, ending at this hour: back to this model's
    /// previous precipitation step (1 for an hourly model, 6 for a 6-hourly one). A model's FIRST served value states
    /// its span too -- a run that stores no lead 0 (aigefs) carries the total since its own init (e.g. 6), and a
    /// model whose cycle is older than <c>model_run.initialized_at</c> has its first bucket cut to its one-hour share
    /// at the read, so it says 1. Omitted when <c>precipitation</c> is absent, and for a run's analysis value at its
    /// own init (lead 0), which covers no forecast hour.
    /// </summary>
    [JsonPropertyName("precipitation_hours")]
    public int? PrecipitationHours { get; init; }

    /// <summary>
    /// <c>null</c> when this member published nothing at this hour.
    /// </summary>
    [JsonPropertyName("wind_speed")]
    public double? WindSpeed { get; init; }

    /// <summary>
    /// Wind direction, degrees (circular).
    /// </summary>
    [JsonPropertyName("wind_direction")]
    public double? WindDirection { get; init; }

    /// <summary>
    /// Snow level, in the request's length unit.
    /// </summary>
    [JsonPropertyName("snow_level")]
    public double? SnowLevel { get; init; }

    /// <summary>
    /// Snow-to-liquid ratio for this member (unitless). Higher = fluffier snow. Omitted for models that don't
    /// diagnose SLR (RRFS may not always provide it).
    /// </summary>
    [JsonPropertyName("slr")]
    public double? SnowToLiquidRatio { get; init; }

    /// <summary>
    /// Per-member snowfall depth at the location's elevation, computed as <c>precipitation × effective_slr(slr,
    /// elevation, snow_level)</c>. Omitted when the derivation is impossible: no SLR, no snow level, or no
    /// precipitation MEASUREMENT for this member. A member whose precipitation was measured at <c>0.0</c> serves a
    /// present <c>0.0</c> instead, so "this member gets no snow" stays distinct from "we cannot say". Its surface
    /// wet-bulb gate reads the member's own temperature and dewpoint; a member without a 2 m dewpoint (<c>aigefs</c>)
    /// is gated on its own temperature with the served dewpoint. It also takes the headline's drizzle gate over the
    /// hours its <c>precipitation_hours</c> span -- a light hour (median precipitation under 0.5 mm) at or above 0 °C
    /// states 0 mm of snow in the headline, and the member's snowfall is scaled by the share of its hours the gate
    /// admits -- so a plume cannot snow where the hourly headline states none for that reason.
    /// </summary>
    [JsonPropertyName("snowfall")]
    public double? Snowfall { get; init; }

    /// <summary>
    /// This member's running precipitation total (mm / in) from the start of the served window through this hour --
    /// the sum of its <c>precipitation</c> over every served hour up to here. The window opens ONE HOUR BEFORE the
    /// first served <c>valid_time</c> (the span of the first hour's own value): the current hour by default, the
    /// run's init with <c>include_past=true</c> or a pinned <c>run</c> -- the window the served <c>snow_outlook</c>
    /// totals over too. A value whose span (<c>precipitation_hours</c>) began before the window counts only its share
    /// inside it, <c>precipitation × (hours of the span inside the window) / precipitation_hours</c> (the headline
    /// spreads a bucket evenly over its hours the same way); a value with no span (a run's analysis at its own init)
    /// counts in full. Present exactly when <c>precipitation</c> is. Clients should draw cumulative QPF from this
    /// instead of summing.
    /// </summary>
    [JsonPropertyName("cumulative_precipitation")]
    public double? CumulativePrecipitation { get; init; }

    /// <summary>
    /// This member's running snowfall total (mm / in), by the same window and share rule as
    /// <c>cumulative_precipitation</c> over its <c>snowfall</c>. Present exactly when <c>snowfall</c> is.
    /// </summary>
    [JsonPropertyName("cumulative_snowfall")]
    public double? CumulativeSnowfall { get; init; }
}
