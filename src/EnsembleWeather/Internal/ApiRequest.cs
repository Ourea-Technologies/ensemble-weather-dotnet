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

    public ApiRequest AddList(string name, ICollection<string> values, string? defaultValue = null)
    {
        if (values.Count == 0)
        {
            return this;
        }

        return Add(name, string.Join(",", values), defaultValue);
    }

    /// <summary>A repeated (exploded) parameter: each distinct value once, sorted, so equal sets share one URL.</summary>
    public ApiRequest AddRepeated(string name, IEnumerable<string> values)
    {
        foreach (string value in values.Distinct(StringComparer.Ordinal).OrderBy(v => v, StringComparer.Ordinal))
        {
            Add(name, value);
        }

        return this;
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
