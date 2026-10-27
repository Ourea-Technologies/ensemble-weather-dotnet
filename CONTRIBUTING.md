# Contributing

Thanks for helping. This is the official .NET SDK for the [Ourea](https://oureawx.com) weather API. Its
behaviour is fixed by the shared spec in [`spec/`](spec/) (`BEHAVIOR.md`, `SDK-STANDARD.md`, the OpenAPI
contract and the test vectors), pinned from
[ensemble-weather-spec](https://github.com/Ourea-Technologies/ensemble-weather-spec). Where this SDK and the
spec disagree, the spec wins; a change to behaviour starts as a change to the spec.

## Setup

- .NET SDK 10 (see `global.json`) plus the .NET 8 runtime (the tests run on both).
- Python 3 for the small repository scripts.

```sh
dotnet restore
dotnet build
```

## Checks

Everything CI runs, in order; each must pass before a commit:

```sh
dotnet format EnsembleWeather.sln --verify-no-changes   # format + code style
dotnet build EnsembleWeather.sln -c Release             # analyzers (latest-recommended + all security rules), warnings as errors, nullable
dotnet test EnsembleWeather.sln -c Release              # unit tests + all 43 shared vectors + spec drift, on net8.0 and net10.0
scripts/run-examples                                    # every example, against the local mock API
scripts/check-dependencies                              # every runtime dependency is justified below
scripts/audit-dependencies                              # no known-vulnerable package, direct or transitive
scripts/sync-spec --check                               # spec/ matches spec/VERSION
dotnet tool restore && dotnet docfx docs/docfx.json     # the API reference builds
dotnet pack -c Release -o artifacts                     # .nupkg + .snupkg
```

Tests and examples never call the real API: the tests use a mock `HttpMessageHandler`, and the examples run
against `tools/EnsembleWeather.MockServer` (set `OUREA_BASE_URL=http://127.0.0.1:5099`).

To pin a newer spec: `scripts/sync-spec <ref>`, then make the vectors pass.

## Code style

- [.NET Framework Design Guidelines](https://learn.microsoft.com/dotnet/standard/design-guidelines/):
  PascalCase, `Async` methods taking a `CancellationToken cancellationToken = default`, exceptions for errors.
- XML docs on every public member (the build fails without them).
- The server computes; the SDK never does statistics (no summing, averaging or interpolating quantiles, no
  derived probabilities). Converting the caller's own threshold input is the only arithmetic.
- Models are hand-maintained `System.Text.Json` source-generated types; no reflection-based serialization
  (the library is trimming and Native AOT compatible on .NET 8+).

- TLS: certificate validation is never optional (BEHAVIOR.md and SDK-STANDARD "Security"). The one TLS-related
  setting is `EnsembleWeatherOptions.CheckCertificateRevocation` (default `true`), which controls only the CRL/OCSP
  revocation lookup for users whose proxies block it; `TlsTests` pins that, including a real handshake against a
  self-signed certificate with the option on and off. Do not add a validation callback or any setting whose name
  suggests TLS can be disabled. The `CA5399` suppressions where the option is applied are deliberate and
  justified in place.

## Commits

Imperative summary line, then a short body saying what changed and why. Keep history linear (rebase, no merge
commits). Do not add `Co-Authored-By` trailers.

## Dependencies

As few as possible (SDK-STANDARD "Dependencies"). The core package uses the BCL's `HttpClient` and
`System.Text.Json` only; optional integrations live in separate packages. `scripts/check-dependencies` fails CI
when a package under `src/` references something without a line in the list below, or when the core gains any
direct dependency beyond the two netstandard2.0 back-ports. A new runtime dependency needs Harrison's approval.

### Runtime dependencies

- `Microsoft.Bcl.TimeProvider` (EnsembleWeather): netstandard2.0 only. Back-port of .NET 8's `TimeProvider`, the
  standard clock abstraction for retry delays and `Retry-After` dates (and for tests). Inbox on net8.0.
- `System.Text.Json` (EnsembleWeather): netstandard2.0 only. The BCL JSON serializer and its source generator
  (AOT-safe, no polymorphic deserialisation). Inbox on net8.0. It brings its own BCL back-ports
  (`Microsoft.Bcl.AsyncInterfaces` for `IAsyncEnumerable`, `System.Memory`, ...) transitively.
- `Microsoft.Extensions.Http` (EnsembleWeather.Extensions.DependencyInjection): the optional DI package only.
  `IHttpClientFactory`, `IOptions` and `ILogger` registration (`services.AddEnsembleWeather(...)`); never a
  dependency of the core.

Test-only packages (xUnit, the test SDK, coverlet, YamlDotNet for reading the OpenAPI spec,
`Microsoft.Extensions.DependencyInjection`) and the DocFX tool are not shipped. All versions are pinned
centrally in `Directory.Packages.props` and locked in `packages.lock.json` (CI restores in locked mode).

## Releases

1. Update `CHANGELOG.md` and the version (`Directory.Build.props` and `SdkInfo.Version`; a test keeps them equal).
2. Tag `vX.Y.Z` on `main`. `release.yml` builds, tests, packs, attests build provenance, attaches an SBOM and
   publishes both packages to NuGet.org with **trusted publishing**: GitHub's OIDC token is exchanged by
   `NuGet/login` for a short-lived API key, so no long-lived key is stored anywhere.
   - One-time setup on nuget.org: a trusted publishing policy for this repository and `release.yml`, and the
     repository variable `NUGET_USER` (the nuget.org profile name; not a secret).
   - Fallback, only if trusted publishing is unavailable: create a package-scoped, glob-limited API key with a
     short expiry, store it as the `NUGET_API_KEY` environment secret of the `nuget` environment (required
     reviewers on), and replace the `NuGet/login` step's output with that secret. Never commit a key.
3. The workflow creates the GitHub release with the changelog section.
