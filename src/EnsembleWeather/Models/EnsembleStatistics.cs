// SPDX-License-Identifier: BSD-2-Clause

using System.Text.Json.Serialization;

namespace EnsembleWeather;

/// <summary>
/// The ensemble distribution of each variable at one hour, in the request's units.
/// </summary>
public sealed class EnsembleStatistics
{
    /// <summary>
    /// Temperature.
    /// </summary>
    [JsonPropertyName("temperature")]
    public Quantiles? Temperature { get; init; }

    /// <summary>
    /// Apparent temperature, the NWS convention: the wind chill (NWS 2001, Osczevski &amp; Bluestein 2005) where the
    /// air is at or below 10 C (50 F) and the 10 m wind above 4.8 km/h (3 mph); the heat index (NWS / Rothfusz 1990)
    /// where the air is at or above 26.7 C (80 F); the air temperature everywhere else. It is computed for EACH
    /// ensemble member from that member's own temperature, wind and dewpoint (the calibrated values at the member's
    /// rank in each), and only then summarised, so the statistics describe states members actually forecast: a cold,
    /// calm member and a warm, windy one never combine into a cold, windy extreme. Every quantile lies within the
    /// range of the members' own values. A member with no dewpoint of its own takes the ensemble's mean dewpoint;
    /// with no dewpoint at all for the hour, no heat index is applied. Omitted when the hour has no members to
    /// compute it from. This is the default index (<c>feels_like=nws</c>); see <c>feels_like_utci</c>.
    /// </summary>
    [JsonPropertyName("feels_like")]
    public Quantiles? FeelsLike { get; init; }

    /// <summary>
    /// The Universal Thermal Climate Index (Bröde et al. 2012), same unit as <c>temperature</c>, with a mean radiant
    /// temperature from the members' surface radiation, computed per member like <c>feels_like</c>. Present only with
    /// <c>feels_like=utci</c> (or <c>nws,utci</c>), which is NOT served yet -- a <c>400 FEELS_LIKE_UNAVAILABLE</c>
    /// until -- so it is absent from every response today.
    /// </summary>
    [JsonPropertyName("feels_like_utci")]
    public Quantiles? FeelsLikeUtci { get; init; }

    /// <summary>
    /// Dewpoint.
    /// </summary>
    [JsonPropertyName("dewpoint")]
    public Quantiles? Dewpoint { get; init; }

    /// <summary>
    /// Precipitation.
    /// </summary>
    [JsonPropertyName("precipitation")]
    public Quantiles? Precipitation { get; init; }

    /// <summary>
    /// Wind speed.
    /// </summary>
    [JsonPropertyName("wind_speed")]
    public Quantiles? WindSpeed { get; init; }

    /// <summary>
    /// Peak gust (same unit as <c>wind_speed</c>). Indicates lift closure risk at ski areas. Derived from the served
    /// sustained wind: every rank (and the mean and spread) is <c>wind_speed</c> times a fitted gust factor of 1.54,
    /// so a gust is never below the wind it accompanies. The wind's calibration (station fit, spread) is the gust's,
    /// and so is its <c>provenance</c>. Absent at lead 0.
    /// </summary>
    [JsonPropertyName("wind_gust")]
    public Quantiles? WindGust { get; init; }

    /// <summary>
    /// **Circular — read this before using any of these numbers.** Wind direction is pooled across models ON THE
    /// CIRCLE, so this <c>Quantiles</c> describes an ARC, not an interval on the number line:
    /// </summary>
    [JsonPropertyName("wind_direction")]
    public Quantiles? WindDirection { get; init; }

    /// <summary>
    /// The wind direction as a CIRCULAR distribution: the resultant mean, an offset-frame standard deviation, and the
    /// central 50 % and 90 % of the members as clockwise arcs. Present on every hour that carries
    /// <c>wind_direction</c>.
    /// </summary>
    [JsonPropertyName("wind_direction_circular")]
    public CircularStatistics? WindDirectionCircular { get; init; }

    /// <summary>
    /// Snow level, in the request's length unit.
    /// </summary>
    [JsonPropertyName("snow_level")]
    public Quantiles? SnowLevel { get; init; }

    /// <summary>
    /// Snow-to-liquid ratio.
    /// </summary>
    [JsonPropertyName("slr")]
    public Quantiles? SnowToLiquidRatio { get; init; }

    /// <summary>
    /// Hourly snowfall depth at the location's elevation (mm, or inches for imperial callers): the hour's
    /// precipitation times the effective snow-to-liquid ratio at the point, with <c>probability</c> the chance of
    /// measurable snow. An hour whose snowfall was ASSESSED -- precipitation, SLR and snow level all present -- and
    /// came out dry (the point is below the snow level, the surface is too warm, or the median precipitation is under
    /// the drizzle gate) serves a present zero: every rank <c>0</c> and <c>probability</c> <c>0</c>. The field is
    /// absent (or <c>null</c>) only when an input is missing and the hour could not be assessed, so absent means
    /// "unknown", never "no snow" -- the convention the member plumes' <c>snowfall</c> already follows.
    /// </summary>
    [JsonPropertyName("snowfall")]
    public Quantiles? Snowfall { get; init; }

    /// <summary>
    /// Cloud cover percentage (0-100%)
    /// </summary>
    [JsonPropertyName("cloud_cover")]
    public Quantiles? CloudCover { get; init; }

    /// <summary>
    /// Skin / ground surface temperature in the caller's temperature units. Sourced from IFS ENS <c>skt</c> — a proxy
    /// for snow-surface temperature at ski areas and pavement-approximate temperature off-road. Null when no model
    /// with this variable contributed to this hour's blend.
    /// </summary>
    [JsonPropertyName("skin_temperature")]
    public Quantiles? SkinTemperature { get; init; }

    /// <summary>
    /// Present only with <c>?diagnostics=ptype</c> (a model-native diagnostic; per-category probabilities are a
    /// follow-up). Model-native precipitation type code (ECMWF <c>ptype</c> convention). 1 = rain, 3 = freezing rain,
    /// 5 = snow, 6 = wet snow, 7 = mixed, 8 = ice pellets. Encoded as <c>Quantiles</c> so ensemble agreement is
    /// visible — e.g. <c>p25 == p75 == 5</c> means all members agree on snow. Distinct from the derived
    /// <c>precip_type</c> label on <c>TimeSeriesPoint</c>, which is a friendlier snow/rain/mix categorization derived
    /// from SLR plus snow level.
    /// </summary>
    [JsonPropertyName("ptype")]
    public Quantiles? PrecipitationTypeCode { get; init; }

    /// <summary>
    /// Mean sea-level pressure: hPa for <c>metric</c> and <c>kelvin</c>, inHg for <c>imperial</c>. A raw blend of the
    /// contributing models; no pressure observation trains a correction. Omitted when no contributing model published
    /// it for this hour.
    /// </summary>
    [JsonPropertyName("mslp")]
    public Quantiles? MeanSeaLevelPressure { get; init; }

    /// <summary>
    /// Convective available potential energy, J/kg in every unit system. A raw blend of the models that publish it
    /// (AIFS and AIGEFS do not). Omitted when none contributed to this hour — never served as a zero, which would
    /// read as "stable".
    /// </summary>
    [JsonPropertyName("cape")]
    public Quantiles? Cape { get; init; }

    /// <summary>
    /// Relative humidity, percent 0–100 in every unit system, computed like <c>feels_like</c>: for each ensemble
    /// member from its own calibrated temperature and dewpoint, then summarised, so no percentile pairs one member's
    /// temperature with another's dewpoint. Omitted when <c>dewpoint</c> is.
    /// </summary>
    [JsonPropertyName("relative_humidity")]
    public Quantiles? RelativeHumidity { get; init; }
}
