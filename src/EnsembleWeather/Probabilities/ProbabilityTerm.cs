// SPDX-License-Identifier: BSD-2-Clause

using System;

namespace EnsembleWeather;

/// <summary>
/// One question for the server: "what is the chance that <see cref="Variable"/> is at least (or at most)
/// <see cref="Threshold"/> over <see cref="Window"/>?" Pass terms in <see cref="ForecastOptions.Probabilities"/>; the
/// answers come back in <see cref="ForecastResponse.Probabilities"/>, in the same order.
/// </summary>
/// <remarks>
/// The server computes every probability from the full ensemble; the SDK only builds the <c>prob=</c> request.
/// Each answer carries its reference class (variable, comparison, threshold, window and member count), never a bare
/// number.
/// </remarks>
/// <example>
/// <code>
/// var sixInches = ProbabilityTerm.AtLeast(ForecastVariable.Snowfall, Threshold.Inches(6), ForecastWindow.Hours(24));
/// var freeze = ProbabilityTerm.AtMost(ForecastVariable.Temperature, Threshold.Fahrenheit(32), ForecastWindow.Hours(12));
/// </code>
/// </example>
public sealed class ProbabilityTerm : IEquatable<ProbabilityTerm>
{
    /// <summary>Initializes a term.</summary>
    /// <param name="variable">The variable.</param>
    /// <param name="comparison">At least or at most.</param>
    /// <param name="threshold">The threshold.</param>
    /// <param name="window">The window; <see langword="null"/> for the next 24 h.</param>
    /// <param name="aggregation">
    /// The aggregation; <see langword="null"/> for the variable's default (<c>total</c> for precipitation and
    /// snowfall, <c>any</c> for the others).
    /// </param>
    /// <exception cref="ArgumentException">
    /// The variable, comparison or aggregation is unknown, the aggregation does not apply to the variable
    /// (<c>any</c>/<c>all</c> on precipitation or snowfall, <c>total</c> on the others), or the threshold's unit does
    /// not measure the variable.
    /// </exception>
    public ProbabilityTerm(
        ForecastVariable variable,
        ProbabilityOperator comparison,
        Threshold threshold,
        ForecastWindow? window = null,
        ProbabilityAggregation? aggregation = null)
    {
        WireRules.RequireKnownVariable(variable, nameof(variable));
        if (comparison != ProbabilityOperator.AtLeast && comparison != ProbabilityOperator.AtMost)
        {
            throw new ArgumentException($"Unknown comparison '{comparison}'; use AtLeast or AtMost.", nameof(comparison));
        }

        bool isAmount = WireRules.IsAmount(variable);
        if (aggregation is ProbabilityAggregation a)
        {
            bool fits = isAmount
                ? a == ProbabilityAggregation.Total
                : a == ProbabilityAggregation.Any || a == ProbabilityAggregation.All;
            if (!fits)
            {
                throw new ArgumentException(
                    isAmount
                        ? $"'{variable}' is a window total: its only aggregation is 'total', not '{a}'."
                        : $"'{variable}' takes the aggregation 'any' or 'all', not '{a}'.",
                    nameof(aggregation));
            }
        }

        WireRules.RequireUnitFits(variable, threshold, nameof(threshold));
        Variable = variable;
        Operator = comparison;
        Threshold = threshold;
        Window = window ?? ForecastWindow.Default;
        Aggregation = aggregation;
    }

    /// <summary>Gets the variable.</summary>
    public ForecastVariable Variable { get; }

    /// <summary>Gets the comparison.</summary>
    public ProbabilityOperator Operator { get; }

    /// <summary>Gets the threshold, as the caller stated it.</summary>
    public Threshold Threshold { get; }

    /// <summary>Gets the window.</summary>
    public ForecastWindow Window { get; }

    /// <summary>Gets the aggregation the caller asked for, or <see langword="null"/> for the variable's default.</summary>
    public ProbabilityAggregation? Aggregation { get; }

    /// <summary>Gets the aggregation the server applies: <see cref="Aggregation"/>, or the variable's default.</summary>
    public ProbabilityAggregation EffectiveAggregation
        => Aggregation ?? (WireRules.IsAmount(Variable) ? ProbabilityAggregation.Total : ProbabilityAggregation.Any);

    /// <summary>The chance that <paramref name="variable"/> is at least <paramref name="threshold"/>.</summary>
    /// <param name="variable">The variable.</param>
    /// <param name="threshold">The threshold.</param>
    /// <param name="window">The window; <see langword="null"/> for the next 24 h.</param>
    /// <param name="aggregation">The aggregation; <see langword="null"/> for the default.</param>
    /// <returns>The term.</returns>
    /// <exception cref="ArgumentException">See the constructor.</exception>
    public static ProbabilityTerm AtLeast(
        ForecastVariable variable, Threshold threshold, ForecastWindow? window = null, ProbabilityAggregation? aggregation = null)
        => new(variable, ProbabilityOperator.AtLeast, threshold, window, aggregation);

    /// <summary>The chance that <paramref name="variable"/> is at most <paramref name="threshold"/>.</summary>
    /// <param name="variable">The variable.</param>
    /// <param name="threshold">The threshold.</param>
    /// <param name="window">The window; <see langword="null"/> for the next 24 h.</param>
    /// <param name="aggregation">The aggregation; <see langword="null"/> for the default.</param>
    /// <returns>The term.</returns>
    /// <exception cref="ArgumentException">See the constructor.</exception>
    public static ProbabilityTerm AtMost(
        ForecastVariable variable, Threshold threshold, ForecastWindow? window = null, ProbabilityAggregation? aggregation = null)
        => new(variable, ProbabilityOperator.AtMost, threshold, window, aggregation);

    /// <inheritdoc/>
    public bool Equals(ProbabilityTerm? other)
        => other is not null && Variable == other.Variable && Operator == other.Operator && Threshold == other.Threshold
            && Window == other.Window && EffectiveAggregation == other.EffectiveAggregation;

    /// <inheritdoc/>
    public override bool Equals(object? obj) => Equals(obj as ProbabilityTerm);

    /// <inheritdoc/>
    public override int GetHashCode() => (Variable, Operator, Threshold, Window, EffectiveAggregation).GetHashCode();

    /// <summary>Describes the term, for example <c>snowfall ge 6 Inches over 24h</c>.</summary>
    /// <returns>The description.</returns>
    public override string ToString() => $"{Variable} {Operator} {Threshold} over {Window} ({EffectiveAggregation})";
}
