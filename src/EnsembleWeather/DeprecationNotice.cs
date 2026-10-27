// SPDX-License-Identifier: BSD-2-Clause

namespace EnsembleWeather;

/// <summary>
/// A response's announcement that what it used is deprecated: the raw <c>Deprecation</c> and <c>Sunset</c> headers
/// and the <c>Link rel="deprecation"</c> URL.
/// </summary>
public sealed class DeprecationNotice
{
    internal DeprecationNotice(string operation, string? deprecation, string? sunset, string? link)
    {
        Operation = operation;
        Deprecation = deprecation;
        Sunset = sunset;
        Link = link;
    }

    /// <summary>Gets the OpenAPI operation id of the call that received the notice.</summary>
    public string Operation { get; }

    /// <summary>Gets the raw <c>Deprecation</c> header (for example <c>@1798761600</c>).</summary>
    public string? Deprecation { get; }

    /// <summary>Gets the raw <c>Sunset</c> header (an HTTP date).</summary>
    public string? Sunset { get; }

    /// <summary>Gets the <c>Link rel="deprecation"</c> URL, which explains the change.</summary>
    public string? Link { get; }

    /// <summary>Describes the notice in one line.</summary>
    /// <returns>The description.</returns>
    public override string ToString()
        => $"Ourea API deprecation on {Operation}: Deprecation={Deprecation ?? "(none)"}, Sunset={Sunset ?? "(none)"}; see {Link ?? "https://docs.oureawx.com/changelog"}";
}
