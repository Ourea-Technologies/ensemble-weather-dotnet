// SPDX-License-Identifier: BSD-2-Clause

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace EnsembleWeather;

/// <summary>
/// Builds the canonical <c>prob=</c> and <c>totals=</c> values (BEHAVIOR.md 4 and 5) and maps the server's answers to
/// the caller's own order by the canonical <c>term</c> each answer echoes, never by position.
/// </summary>
internal static class TermCodec
{
    private static readonly string[] AggregationOrder = ["total", "any", "all"];

    public static ProbabilityPlan PlanProbabilities(IList<ProbabilityTerm> terms, UnitSystem units)
    {
        var keys = new ProbabilityKey[terms.Count];
        for (int i = 0; i < terms.Count; i++)
        {
            ProbabilityTerm term = terms[i] ?? throw new ArgumentException("A probability term is null.", nameof(terms));
            int decimals = WireRules.Precision(term.Variable, units);
            double rounded = WireRules.Round(WireRules.ToRequestUnits(term.Threshold, term.Variable, units), decimals);
            keys[i] = new ProbabilityKey(
                term.Variable.Value,
                term.Operator.Value,
                Scaled(rounded, decimals),
                decimals,
                term.Window.StartHours,
                term.Window.EndHours,
                term.EffectiveAggregation.Value,
                WireRules.FormatFixed(rounded, decimals),
                rounded);
        }

        string wire = string.Join(",", keys
            .Distinct()
            .OrderBy(k => k.Variable, StringComparer.Ordinal)
            .ThenBy(k => k.Operator, StringComparer.Ordinal)
            .ThenBy(k => k.Start)
            .ThenBy(k => k.End)
            .ThenBy(k => Array.IndexOf(AggregationOrder, k.Aggregation))
            .ThenBy(k => k.Rounded)
            .Select(k => k.Wire()));
        return new ProbabilityPlan(wire, keys);
    }

    /// <summary>
    /// Maps the answers back to the caller's terms. A body with no <c>probabilities</c> at all maps to an empty list;
    /// a body that answers some terms must answer every one.
    /// </summary>
    public static IReadOnlyList<ProbabilityResult> MapProbabilities(
        IReadOnlyList<ProbabilityResult> answers, ProbabilityPlan plan)
    {
        if (answers.Count == 0)
        {
            return answers;
        }

        var results = new ProbabilityResult[plan.Keys.Count];
        for (int i = 0; i < plan.Keys.Count; i++)
        {
            string wanted = plan.Keys[i].Wire();
            results[i] = answers.FirstOrDefault(a => string.Equals(a.Term, wanted, StringComparison.Ordinal))
                ?? throw new EnsembleWeatherException($"The response did not answer the probability term '{wanted}'.");
        }

        return results;
    }

    public static TotalsPlan PlanTotals(IList<WindowTotalTerm> terms)
    {
        var keys = new TotalsKey[terms.Count];
        for (int i = 0; i < terms.Count; i++)
        {
            WindowTotalTerm term = terms[i] ?? throw new ArgumentException("A window total term is null.", nameof(terms));
            keys[i] = new TotalsKey(term.Variable.Value, term.Window.StartHours, term.Window.EndHours, term.ToString());
        }

        string wire = string.Join(",", keys
            .Distinct()
            .OrderBy(k => k.Variable, StringComparer.Ordinal)
            .ThenBy(k => k.Start)
            .ThenBy(k => k.End)
            .Select(k => k.Wire));
        return new TotalsPlan(wire, keys);
    }

    public static IReadOnlyList<WindowTotal> MapTotals(IReadOnlyList<WindowTotal> answers, TotalsPlan plan)
    {
        if (answers.Count == 0)
        {
            return answers;
        }

        var results = new WindowTotal[plan.Keys.Count];
        for (int i = 0; i < plan.Keys.Count; i++)
        {
            TotalsKey wanted = plan.Keys[i];
            results[i] = answers.FirstOrDefault(a => string.Equals(a.Term, wanted.Wire, StringComparison.Ordinal))
                ?? throw new EnsembleWeatherException($"The response did not answer the window total '{wanted.Wire}'.");
        }

        return results;
    }

    private static long Scaled(double value, int decimals)
        => (long)Math.Round(value * Math.Pow(10, decimals), MidpointRounding.AwayFromZero);

    internal readonly record struct ProbabilityKey(
        string Variable, string Operator, long Threshold, int Decimals, int Start, int End, string Aggregation, string Formatted, double Rounded)
    {
        public bool Equals(ProbabilityKey other)
            => Variable == other.Variable && Operator == other.Operator && Threshold == other.Threshold
                && Start == other.Start && End == other.End && Aggregation == other.Aggregation;

        public override int GetHashCode() => (Variable, Operator, Threshold, Start, End, Aggregation).GetHashCode();

        public string Wire()
        {
            string window = Start == 0
                ? End.ToString(CultureInfo.InvariantCulture) + "h"
                : string.Format(CultureInfo.InvariantCulture, "{0}-{1}h", Start, End);
            bool isDefault = Aggregation == (Variable is "precipitation" or "snowfall" ? "total" : "any");
            return $"{Variable}:{Operator}:{Formatted}:{window}" + (isDefault ? string.Empty : ":" + Aggregation);
        }
    }

    internal readonly record struct TotalsKey(string Variable, int Start, int End, string Wire);

    internal sealed record ProbabilityPlan(string Wire, IReadOnlyList<ProbabilityKey> Keys);

    internal sealed record TotalsPlan(string Wire, IReadOnlyList<TotalsKey> Keys);
}
