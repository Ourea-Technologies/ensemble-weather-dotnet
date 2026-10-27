// SPDX-License-Identifier: BSD-2-Clause

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace EnsembleWeather;

/// <summary>
/// Which serve-path branch produced this hour. Each list names the <c>ensemble</c> field it applies to, so a tag
/// joins to the value it describes with no lookup table.
/// </summary>
public sealed class HourProvenance
{
    /// <summary>
    /// This hour sits strictly between two native model steps: its instantaneous values were linearly interpolated
    /// (plus a diurnal bulge), not read from a model step. Omitted when false.
    /// </summary>
    [JsonPropertyName("densified")]
    public bool? Densified { get; init; }

    /// <summary>
    /// Variables corrected by a fitted NGR / truncated-NGR / censored-gamma model for this location, variable and
    /// lead-time bin.
    /// </summary>
    [JsonPropertyName("ngr")]
    public IReadOnlyList<string> Ngr { get; init; } = [];

    /// <summary>
    /// Variables served through the <c>inflate_around_mean</c> fallback because no fitted model was available for
    /// that lead-time bin. The widening is a configured constant, not a calibration.
    /// </summary>
    [JsonPropertyName("fallback")]
    public IReadOnlyList<string> Fallback { get; init; } = [];

    /// <summary>
    /// Variables at a point no fitted station reaches (none within 75 km), served from the fits of climatologically
    /// similar supplemental stations (same terrain class, ranked by distance and elevation difference, kept apart
    /// from one another) and shrunk toward the fallback. Borrowed statistical strength, not a calibration trained for
    /// this point. Added 2026-10-09; only variables whose borrowed fit beat the fallback on held-out stations borrow.
    /// </summary>
    [JsonPropertyName("borrowed")]
    public IReadOnlyList<string> Borrowed { get; init; } = [];

    /// <summary>
    /// Variables whose percentile ladder was rebuilt as <c>N(mean, min_serve_sigma)</c> by the serve-time spread
    /// floor. Such a ladder carries no ensemble information: every probability read off it is fabricated at the floor
    /// width. Treat the mean as usable and the uncertainty around it as not.
    /// </summary>
    [JsonPropertyName("spread_floored")]
    public IReadOnlyList<string> SpreadFloored { get; init; } = [];

    /// <summary>
    /// Variables whose low tail was truncated by the zero floor. The truncation biases the conditional mean upward;
    /// it is a backstop, not a neutral operation.
    /// </summary>
    [JsonPropertyName("floor_clamped")]
    public IReadOnlyList<string> FloorClamped { get; init; } = [];

    /// <summary>
    /// Accumulation variables whose value at this hour is an even share of a wider accumulation, not an independently
    /// modelled hourly rate: either a native multi-hour bucket split across the hours between two model steps, or a
    /// contributor's accumulation spread evenly across the hours that failed to download before it — the missing
    /// hours are served with their share, and no hour carries a multi-step total. Every hour of the same split
    /// carries the identical share by construction. (This replaces the short-lived
    /// <c>accumulation_interval_hours</c>, which tagged the widened total instead of splitting it.)
    /// </summary>
    [JsonPropertyName("bucket_split")]
    public IReadOnlyList<string> BucketSplit { get; init; } = [];

    /// <summary>
    /// Variables whose blend included a contributor that supplied a two-parameter predictive distribution (a location
    /// and a scale) instead of ensemble members. NOAA's REFS publishes <c>mean</c> and <c>sprd</c> for RRFS and no
    /// member files at all, so RRFS contributes a measured Gaussian rather than a draw of members.
    /// </summary>
    [JsonPropertyName("distribution_parameters")]
    public IReadOnlyList<string> DistributionParameters { get; init; } = [];

    /// <summary>
    /// Variables whose spread was not measured by ANY contributor at this hour and was interpolated from the
    /// bracketing hours that did measure it. The global ensembles publish on a 6-hourly native step while RRFS is
    /// hourly, so at the five leads between those steps RRFS is the only contributor and supplies a location with no
    /// distribution; the served ladder there is the two bracketing hours' quantile shape, re-centred on this hour's
    /// own mean.
    /// </summary>
    [JsonPropertyName("spread_interpolated")]
    public IReadOnlyList<string> SpreadInterpolated { get; init; } = [];

    /// <summary>
    /// Variables whose pool at this hour contains at least one contributor that did NOT publish at this lead. Its
    /// distribution was interpolated from its two bracketing native steps (linearly in the mean, in every quantile
    /// and in the scale, with the same diurnal shaping the hourly densifier applies to any chord) so the pool — and
    /// therefore its spread — is the same set of contributors at every lead.
    /// </summary>
    [JsonPropertyName("interpolated_contributor")]
    public IReadOnlyList<string> InterpolatedContributor { get; init; } = [];

    /// <summary>
    /// Variables whose served <c>probability</c> was computed from a distribution recovered by METHOD OF MOMENTS from
    /// the hour's mean and standard deviation, rather than from the exact parameters the calibration fit produced.
    /// </summary>
    [JsonPropertyName("moment_matched_family")]
    public IReadOnlyList<string> MomentMatchedFamily { get; init; } = [];

    /// <summary>
    /// Variables carried from the location's primary elevation to this point's elevation by the hour's OWN upper-air
    /// lapse rate (the profile bracketed the layer). Only an <c>elevation_forecasts[].hourly</c> hour can carry it.
    /// </summary>
    [JsonPropertyName("lapse_profile")]
    public IReadOnlyList<string> LapseProfile { get; init; } = [];

    /// <summary>
    /// Variables carried from the location's primary elevation to this point's elevation by a CLIMATOLOGICAL lapse
    /// rate — for <c>temperature</c> because the hour's profile could not bracket the layer (always the case below
    /// the primary, where the profile starts), the diurnal table; for <c>dewpoint</c> always, a mean 2 °C/km (no
    /// upper-air humidity is ingested). Only an <c>elevation_forecasts[].hourly</c> hour can carry it.
    /// </summary>
    [JsonPropertyName("lapse_climatology")]
    public IReadOnlyList<string> LapseClimatology { get; init; } = [];

    /// <summary>
    /// Variables carried from the location's primary elevation to this point's elevation by the local PRISM
    /// precipitation–elevation regression: the PRISM 800 m climatological normals at the observing stations within 40
    /// km, regressed on the stations' elevations (distance-weighted, Daly et al. 2008), give the ratio of the normal
    /// at this elevation to the normal at the primary's for the valid time's month, and the hour's corrected
    /// <c>precipitation</c> and <c>snowfall</c> distributions are scaled by it (every rank, mean and spread; the
    /// probability of any precipitation is unchanged). Absent when the neighbourhood cannot determine a slope (fewer
    /// than five stations, or under 300 m of elevation span) or PRISM is not loaded — the amounts are then the
    /// primary's. Only an <c>elevation_forecasts[].hourly</c> hour can carry it.
    /// </summary>
    [JsonPropertyName("lapse_prism")]
    public IReadOnlyList<string> LapsePrism { get; init; } = [];

    /// <summary>
    /// Variables whose calibration fit was accepted and served, but whose fitted spread fell BELOW the serve-time
    /// spread band and was re-inflated to the band's low edge. The mean is the fit's; the uncertainty around it is
    /// the band's, not the fit's. A coverage or PIT statistic computed over such hours describes the band.
    /// </summary>
    [JsonPropertyName("spread_clamped_low")]
    public IReadOnlyList<string> SpreadClampedLow { get; init; } = [];

    /// <summary>
    /// Variables whose calibration fit was accepted and served, but whose fitted spread rose ABOVE the serve-time
    /// spread band and was cut back to the band's high edge. The mean is the fit's; the uncertainty around it is the
    /// band's, not the fit's. A coverage or PIT statistic computed over such hours describes the band.
    /// </summary>
    [JsonPropertyName("spread_clamped_high")]
    public IReadOnlyList<string> SpreadClampedHigh { get; init; } = [];

    /// <summary>
    /// Variables whose pool at this hour included a contributor that publishes a 10 m wind but no gust (the AI
    /// ensembles) and was dressed from its mean wind by the WMO in-land gust factor of 1.75 (Harper, Kepert &amp;
    /// Ginger 2010, WMO/TD-No. 1555, Table 1.1). The dressed contributor carries its wind's spread, not a gust-
    /// specific one. Only <c>wind_gust</c> can appear here. Since the served gust is derived from the served wind, it
    /// carries the wind's provenance and this list is empty on every hour that serves a wind.
    /// </summary>
    [JsonPropertyName("gust_dressed")]
    public IReadOnlyList<string> GustDressed { get; init; } = [];
}
