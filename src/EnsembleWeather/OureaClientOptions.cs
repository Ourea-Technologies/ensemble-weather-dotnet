// SPDX-License-Identifier: BSD-2-Clause

using System;

namespace EnsembleWeather;

/// <summary>
/// Settings for an <see cref="EnsembleWeatherClient"/>. Every setting has a working default: a client built from
/// <c>new EnsembleWeatherOptions()</c> calls the production API anonymously (or with <c>OUREA_API_KEY</c>).
/// </summary>
/// <remarks>
/// There is deliberately no setting that disables TLS certificate validation;
/// <see cref="CheckCertificateRevocation"/> controls only the revocation lookup.
/// </remarks>
public sealed class EnsembleWeatherOptions
{
    /// <summary>The production API, <c>https://api.oureawx.com</c>.</summary>
    public static readonly Uri DefaultBaseUrl = new("https://api.oureawx.com");

    /// <summary>
    /// Gets or sets the API key, sent as <c>X-API-Key</c>. When <see langword="null"/>, the client reads the
    /// <c>OUREA_API_KEY</c> environment variable; with neither, calls are anonymous (the Free tier), which is valid.
    /// </summary>
    /// <remarks>
    /// The key is never logged, never put in an exception message or <see cref="object.ToString"/>, and never sent
    /// to any host but <see cref="BaseUrl"/>'s (a redirect to another host drops it). Routes that need no credential
    /// (search, catalog detail, attribution) are sent without it so every caller shares one edge-cache entry; see
    /// <see cref="SendApiKeyOnOpenRoutes"/>.
    /// </remarks>
    public string? ApiKey { get; set; }

    /// <summary>
    /// Gets or sets the API's base URL (tests, proxies). When <see langword="null"/>, the client reads the
    /// <c>OUREA_BASE_URL</c> environment variable, else uses <see cref="DefaultBaseUrl"/>. A trailing slash and a
    /// path prefix are both fine.
    /// </summary>
    public Uri? BaseUrl { get; set; }

    /// <summary>
    /// Gets or sets the unit system sent as <c>units=</c> on every request that accepts it, unless the request sets
    /// its own. Defaults to <see cref="UnitSystem.Metric"/>.
    /// </summary>
    public UnitSystem Units { get; set; } = UnitSystem.Metric;

    /// <summary>Gets or sets the timeout of each attempt (not of the call with its retries). Defaults to 30 s.</summary>
    public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(30);

    /// <summary>
    /// Gets or sets how many times a call is retried after a connection failure, <c>429</c>, <c>502</c>, <c>503</c> or
    /// <c>504</c>. Defaults to 2 (three attempts in all); 0 disables retrying.
    /// </summary>
    public int MaxRetries { get; set; } = 2;

    /// <summary>
    /// Gets or sets whether the local <c>ETag</c> cache is on. Defaults to <see langword="true"/>: a repeated call
    /// sends <c>If-None-Match</c> and a <c>304</c> returns the stored body without downloading it again.
    /// </summary>
    public bool EnableCache { get; set; } = true;

    /// <summary>
    /// Gets or sets the cache store. When <see langword="null"/> (and <see cref="EnableCache"/> is on) the client
    /// uses its own <see cref="MemoryResponseCache"/> (256 entries, 32 MiB). Supply your own
    /// <see cref="IResponseCache"/> to share entries across clients or processes.
    /// </summary>
    public IResponseCache? Cache { get; set; }

    /// <summary>
    /// Gets or sets whether to send the API key on the routes that need no credential (search, catalog detail,
    /// attribution) too, so every request is attributed to the key. Defaults to <see langword="false"/>: those
    /// requests go keyless and share the edge cache with every other caller.
    /// </summary>
    public bool SendApiKeyOnOpenRoutes { get; set; }

    /// <summary>
    /// Gets or sets whether the server certificate's revocation status is checked (CRL/OCSP) when the client
    /// creates its own HTTP handler. Defaults to <see langword="true"/>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This controls <em>only</em> the revocation lookup. Certificate validation itself (a trusted chain, a valid
    /// date, a matching host name) is always performed by the platform and cannot be turned off.
    /// </para>
    /// <para>
    /// Checking revocation means fetching CRL or OCSP data from the certificate authority. Behind a restrictive
    /// corporate proxy or on an offline or air-gapped network those fetches can be blocked, and every connection
    /// then fails with a TLS error even though the certificate is valid. Set this to <see langword="false"/> only
    /// in that situation. The trade-off: a certificate that was revoked after it was issued (for example because
    /// its private key leaked) would still be accepted until it expires.
    /// </para>
    /// <para>
    /// It applies to the handler the SDK creates (the parameterless and options constructors, and the
    /// dependency-injection package); an <see cref="System.Net.Http.HttpClient"/> you pass in keeps its own
    /// handler's settings.
    /// </para>
    /// </remarks>
    public bool CheckCertificateRevocation { get; set; } = true;

    /// <summary>
    /// Gets or sets your application's product token (for example <c>my-ski-app/2.1</c>), appended to the SDK's
    /// <c>User-Agent</c>. Never put user data in it.
    /// </summary>
    public string? ApplicationId { get; set; }

    /// <summary>
    /// Gets or sets the clock used for retry delays and <c>Retry-After</c> dates. Defaults to
    /// <see cref="TimeProvider.System"/>; tests can supply a fake one.
    /// </summary>
    public TimeProvider? TimeProvider { get; set; }

    /// <summary>
    /// Gets or sets a callback for deprecation notices. The SDK reports each operation's first notice once per
    /// process; with no callback it writes it to <see cref="System.Diagnostics.Trace"/>. The dependency-injection
    /// package routes it to <c>ILogger</c>.
    /// </summary>
    public Action<DeprecationNotice>? OnDeprecation { get; set; }

    /// <summary>Test hook: the jitter source, a value in [0, 1] (the vectors seed it to 1.0).</summary>
    internal Func<double>? Jitter { get; set; }

    /// <summary>Test hook: the once-per-operation registry (the process-wide one by default).</summary>
    internal DeprecationRegistry? Deprecations { get; set; }

    /// <summary>Test hook: the environment the client reads <c>OUREA_API_KEY</c> and <c>OUREA_BASE_URL</c> from.</summary>
    internal Func<string, string?>? Environment { get; set; }

    internal EnsembleWeatherOptions Clone() => (EnsembleWeatherOptions)MemberwiseClone();
}
