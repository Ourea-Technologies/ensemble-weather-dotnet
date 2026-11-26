// SPDX-License-Identifier: BSD-2-Clause

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EnsembleWeather;

/// <summary>
/// One API request in canonical form (BEHAVIOR.md 3.1): its path segments percent-encoded, and only the parameters
/// that differ from the route's defaults, each once, in alphabetical order.
/// </summary>
internal sealed class ApiRequest
{
    private readonly List<KeyValuePair<string, string>> _query = [];

    public ApiRequest(string operation, string path, bool openRoute = false)
    {
        Operation = operation;
        Path = path;
        OpenRoute = openRoute;
    }

    /// <summary>The OpenAPI operation id (for deprecation notices).</summary>
    public string Operation { get; }

    /// <summary>The path below the base URL, already encoded, starting with <c>/api/v1/</c>.</summary>
    public string Path { get; }

    /// <summary>Whether the route needs no credential (sent keyless, so the edge can share it).</summary>
    public bool OpenRoute { get; }

    public static string Segment(string value) => WireRules.PercentEncode(value);

    /// <summary>Adds a value that is already encoded (prob/totals, which keep <c>:</c> and <c>,</c> literal).</summary>
    public ApiRequest AddEncoded(string name, string? encoded)
    {
        if (!string.IsNullOrEmpty(encoded))
        {
            _query.Add(new KeyValuePair<string, string>(name, encoded!));
        }

        return this;
    }

    public ApiRequest Add(string name, string? value, string? defaultValue = null)
    {
        if (value is null || string.Equals(value, defaultValue, StringComparison.Ordinal))
        {
            return this;
        }

        return AddEncoded(name, WireRules.PercentEncode(value));
    }

    public ApiRequest Add(string name, bool? value, bool defaultValue)
        => value is bool b && b != defaultValue ? AddEncoded(name, WireRules.FormatBool(b)) : this;

    public ApiRequest Add(string name, long? value, long? defaultValue = null)
        => value is long v && v != defaultValue ? AddEncoded(name, WireRules.FormatInt(v)) : this;

    public ApiRequest Add(string name, double? value, double? defaultValue = null)
        => value is double v && !v.Equals(defaultValue) ? AddEncoded(name, WireRules.PercentEncode(WireRules.FormatNumber(v))) : this;

    /// <summary>
    /// A comma list in the API's canonical form (BEHAVIOR.md 3.1 rule 5): each distinct value once, sorted, each
    /// percent-encoded, joined by literal commas, so equal sets share one URL (any other order is a 301 from the
    /// server, and <c>%2C</c> would be another edge-cache entry).
    /// </summary>
    public ApiRequest AddList(string name, IEnumerable<string> values, string? defaultValue = null)
    {
        List<string> items = values.Distinct(StringComparer.Ordinal).OrderBy(v => v, StringComparer.Ordinal).ToList();
        if (items.Count == 0 || (items.Count == 1 && string.Equals(items[0], defaultValue, StringComparison.Ordinal)))
        {
            return this;
        }

        return AddEncoded(name, string.Join(",", items.Select(v => WireRules.PercentEncode(v))));
    }

    public ApiRequest AddUnits(UnitSystem units) => Add("units", units.Value, UnitSystem.Metric.Value);

    /// <summary>The query string, without <c>?</c>: parameters sorted by name (then value), joined by <c>&amp;</c>.</summary>
    public string QueryString()
    {
        var builder = new StringBuilder();
        foreach (KeyValuePair<string, string> pair in _query
            .OrderBy(p => p.Key, StringComparer.Ordinal)
            .ThenBy(p => p.Value, StringComparer.Ordinal))
        {
            if (builder.Length > 0)
            {
                builder.Append('&');
            }

            builder.Append(pair.Key).Append('=').Append(pair.Value);
        }

        return builder.ToString();
    }
}
