// SPDX-License-Identifier: BSD-2-Clause

using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace EnsembleWeather;

/// <summary>
/// One run's row of a snow-trend grid; a day it does not cover is <see langword="null"/>.
/// </summary>
public sealed class SnowTrendRun
{
    /// <summary>
    /// <c>true</c>: re-rendered after the fact from the pinned run (<c>?run=&lt;initialized_at&gt;</c>) by the
    /// post-deploy backfill -- today's calibration and no live nowcast, so it can differ slightly from what was
    /// served then. <c>false</c>: recorded while it was served, or the live newest run. A served recording always
    /// replaces a backfilled one.
    /// </summary>
    [JsonPropertyName("backfilled")]
    public bool Backfilled { get; init; }

    /// <summary>
    /// <c>ourea</c>: the served body's <c>model_run.initialized_at</c>. A model: its own cycle.
    /// </summary>
    [JsonPropertyName("initialized_at")]
    public DateTimeOffset InitializedAt { get; init; }

    /// <summary>
    /// One entry per <c>dates</c> entry, in order; <c>null</c> where the run has no value.
    /// </summary>
    [JsonPropertyName("days")]
    public IReadOnlyList<SnowTrendDay?> Days { get => field ?? []; init; } = [];
}
