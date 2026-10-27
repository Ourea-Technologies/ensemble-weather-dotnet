// SPDX-License-Identifier: BSD-2-Clause

using System;

namespace EnsembleWeather;

/// <summary>
/// A request for the distribution of a window total ("storm total"): each member's own precipitation or snowfall
/// total over <see cref="Window"/>, as quantiles computed by the server. Pass terms in
/// <see cref="ForecastOptions.WindowTotals"/>; the answers come back in <see cref="ForecastResponse.WindowTotals"/>,
/// in the same order.
/// </summary>
/// <remarks>
/// This is the only storm total the SDK offers: summing hourly quantiles gives a wrong total, so the SDK never does.
/// </remarks>
public sealed class WindowTotalTerm : IEquatable<WindowTotalTerm>
{
    /// <summary>Initializes a term.</summary>
    /// <param name="variable"><see cref="ForecastVariable.Precipitation"/> or <see cref="ForecastVariable.Snowfall"/>.</param>
    /// <param name="window">The window.</param>
    /// <exception cref="ArgumentException">The variable is not precipitation or snowfall.</exception>
    public WindowTotalTerm(ForecastVariable variable, ForecastWindow window)
    {
        if (!WireRules.IsAmount(variable))
        {
            throw new ArgumentException($"Window totals are for precipitation and snowfall, not '{variable}'.", nameof(variable));
        }

        Variable = variable;
        Window = window;
    }

    /// <summary>Gets the variable.</summary>
    public ForecastVariable Variable { get; }

    /// <summary>Gets the window.</summary>
    public ForecastWindow Window { get; }

    /// <summary>The distribution of total precipitation over a window.</summary>
    /// <param name="window">The window.</param>
    /// <returns>The term.</returns>
    public static WindowTotalTerm Precipitation(ForecastWindow window) => new(ForecastVariable.Precipitation, window);

    /// <summary>The distribution of total snowfall over a window.</summary>
    /// <param name="window">The window.</param>
    /// <returns>The term.</returns>
    public static WindowTotalTerm Snowfall(ForecastWindow window) => new(ForecastVariable.Snowfall, window);

    /// <inheritdoc/>
    public bool Equals(WindowTotalTerm? other) => other is not null && Variable == other.Variable && Window == other.Window;

    /// <inheritdoc/>
    public override bool Equals(object? obj) => Equals(obj as WindowTotalTerm);

    /// <inheritdoc/>
    public override int GetHashCode() => (Variable, Window).GetHashCode();

    /// <summary>Formats the term as the API writes it, for example <c>snowfall:72h</c>.</summary>
    /// <returns>The wire form.</returns>
    public override string ToString() => Variable.Value + ":" + Window;
}
