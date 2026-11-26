// SPDX-License-Identifier: BSD-2-Clause

using System;
using System.Text.Json.Serialization;

namespace EnsembleWeather;

/// <summary>
/// Present only on a day part of which is a measurement -- the leading date, when the radar (MRMS radar-only)
/// measured hours between its local midnight and the reference init -- and absent otherwise. Always absent on a
/// pinned <c>run</c>: a pinned row counts nothing from before the run's init, so its <c>total_precipitation</c> and
/// <c>total_snowfall</c> are its served hours'. <c>amount</c> is that measured precipitation, in the response's
/// precipitation unit (as <c>precipitation_display.amount</c>); <c>through</c> is the end (UTC) of its last
/// measured hour. The day's <c>total_precipitation</c> and <c>total_snowfall</c> include it with its measurement
/// error: its band is no longer only the forecast hours' spread, because a radar amount is itself uncertain
/// (against MRMS MultiSensor QPE, the scoring truth, a radar-only day sum runs from about 0.7x to 1.2x at 15 mm or
/// more, wider when lighter). The day's <c>mean</c> does not count <c>amount</c> in full, so a first day's
/// <c>total_precipitation.mean</c> can be below <c>measured_precipitation.amount</c>. <c>amount</c> is the radar's
/// own sum; the total counts it at the measured ratio of truth to radar for an amount of its size, about 0.87 to
/// 0.93: 0.914 of it up to 1.9 mm, 0.868 at 9.0 mm, 0.925 from 22.1 mm (the radar amount in mm, whatever the
/// response unit). Between those amounts the water counted rises in a straight line with the amount measured, so
/// the ratio has no step and more measured water never counts for less. Example: 11.33 mm measured counts as 10.06
/// mm (0.888), and with 0.02 mm in the hours still forecast the day's <c>mean</c> is 10.08 mm. Each percentile
/// takes the ratio's own quantile the same way.
/// </summary>
public sealed class MeasuredPrecipitation
{
    /// <summary>
    /// The amount, in the response's units.
    /// </summary>
    [JsonPropertyName("amount")]
    public double Amount { get; init; }

    /// <summary>
    /// The last day the totals cover.
    /// </summary>
    [JsonPropertyName("through")]
    public DateTimeOffset Through { get; init; }
}
