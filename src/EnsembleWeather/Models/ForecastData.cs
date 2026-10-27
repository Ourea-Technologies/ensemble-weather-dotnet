// SPDX-License-Identifier: BSD-2-Clause

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace EnsembleWeather;

/// <summary>
/// The hourly series, the daily summaries and the snow outlook.
/// </summary>
public sealed class ForecastData
{
    /// <summary>
    /// The last served hour, in hours from the FIRST served one (the current hour by default, the run's init with
    /// <c>include_past=true</c> or a pinned <c>run</c>) -- the request's <c>max_hours</c> clamped to the tier, less
    /// when the run ends first. <c>0</c> with <c>hourly=false</c>, which serves no hour.
    /// </summary>
    [JsonPropertyName("max_hours")]
    public int MaxHours { get; init; }

    /// <summary>
    /// Hours between points of the hourly series.
    /// </summary>
    [JsonPropertyName("interval_hours")]
    public int IntervalHours { get; init; }

    /// <summary>
    /// Omitted with <c>hourly=false</c>. Starts at the hour containing the request time (<c>floor(now)</c>; 23:00Z
    /// for a 23:23Z request against the 18Z run) and runs <c>max_hours</c> past it; with <c>include_past=true</c> or
    /// a pinned <c>run</c>, at the run's init. <c>lead_time_hours</c> counts from <c>model_run.initialized_at</c>
    /// either way. Hourly points, ascending by <c>valid_time</c>. **Not guaranteed contiguous.** <c>temperature</c>,
    /// <c>precipitation</c>, <c>wind_speed</c> and <c>wind_direction</c> are non-nullable on a <c>TimeSeriesPoint</c>
    /// because the blend always produces them; an hour where it did not is OMITTED from this array rather than served
    /// with fabricated zeros. That is a system failure on our side, it is alarmed on, and it is expected to be
    /// vanishingly rare — but a client must key on <c>valid_time</c> / <c>lead_time_hours</c> and must not assume
    /// <c>timeseries[i]</c> is <c>i</c> hours after <c>timeseries[0]</c>. The same hours are absent from <c>daily</c>
    /// (their day totals sum only the hours present, exactly as the first and last day of any forecast already do)
    /// and from <c>elevation_forecasts[].timeseries</c>.
    /// </summary>
    [JsonPropertyName("timeseries")]
    public IReadOnlyList<TimeSeriesPoint> Timeseries { get; init; } = [];

    /// <summary>
    /// The daily summaries.
    /// </summary>
    [JsonPropertyName("daily")]
    public IReadOnlyList<DailyForecast> Daily { get; init; } = [];

    /// <summary>
    /// Rolling snow-accumulation summary for common UI windows. Always forward-looking (next N hours / days), summed
    /// from the same corrected hourly series as <c>daily</c> and <c>timeseries</c> — exposed here so clients don't
    /// each re-implement the same rolling sums. Like <c>daily</c>, it does not depend on <c>max_hours</c> or
    /// <c>hourly</c>: the windows are read from the forecast, not from the served hours. Its windows open at the
    /// first served hour -- the current hour by default, the run's init with <c>include_past=true</c> -- with
    /// <c>hourly=false</c> too. Present exactly when <c>daily</c> is.
    /// </summary>
    [JsonPropertyName("snow_outlook")]
    public SnowOutlook? SnowOutlook { get; init; }
}
