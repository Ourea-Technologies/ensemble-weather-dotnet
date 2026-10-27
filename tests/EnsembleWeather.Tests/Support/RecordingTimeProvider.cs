// SPDX-License-Identifier: BSD-2-Clause

using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace EnsembleWeather.Tests.Support;

/// <summary>
/// A fake clock: every timer (the SDK's retry delays) fires at once and its due time is recorded, so no test sleeps.
/// </summary>
internal sealed class RecordingTimeProvider : TimeProvider
{
    private readonly DateTimeOffset _now;

    public RecordingTimeProvider(DateTimeOffset? now = null)
    {
        _now = now ?? new DateTimeOffset(2026, 11, 15, 0, 0, 0, TimeSpan.Zero);
    }

    public ConcurrentQueue<TimeSpan> Delays { get; } = new();

    public override DateTimeOffset GetUtcNow() => _now;

    public override ITimer CreateTimer(TimerCallback callback, object? state, TimeSpan dueTime, TimeSpan period)
    {
        if (dueTime != Timeout.InfiniteTimeSpan)
        {
            Delays.Enqueue(dueTime);
            ThreadPool.QueueUserWorkItem(_ => callback(state));
        }

        return new NoopTimer();
    }

    private sealed class NoopTimer : ITimer
    {
        public bool Change(TimeSpan dueTime, TimeSpan period) => true;

        public void Dispose()
        {
        }

        public ValueTask DisposeAsync() => ValueTask.CompletedTask;
    }
}
