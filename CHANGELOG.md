# Changelog

All notable changes to this project are documented here. The format follows
[Keep a Changelog](https://keepachangelog.com/en/1.1.0/), and the project uses
[Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

## [0.1.0] - 2026-10-26

### Added

- `EnsembleWeatherClient` for the Ourea API v1 (contract 1.0.0): location and coordinate forecasts, current
  conditions, alerts, nearby locations, search, location lists, the catalog, model runs, storm reports and
  attribution, as `Async` methods with `CancellationToken`, and `IAsyncEnumerable` iterators for paged lists.
- Typed probability terms (`ProbabilityTerm`, `Threshold`, `ForecastWindow`) and window totals
  (`WindowTotalTerm`), sent in the server's canonical order and answered in the caller's order.
- Canonical, edge-cacheable requests; keyless open routes; an `ETag` LRU cache on by default (pluggable through
  `IResponseCache`); retries with full-jitter backoff and `Retry-After`; same-origin redirects; a 64 MiB body
  bound; typed exceptions per error category; rate-limit and deprecation metadata.
- `EnsembleWeather.Extensions.DependencyInjection`: `services.AddEnsembleWeather(...)` with
  `IHttpClientFactory`, `IOptions<EnsembleWeatherOptions>` and `ILogger`.
- Targets .NET 8 and .NET Standard 2.0; trimming and Native AOT compatible on .NET 8+.

[Unreleased]: https://github.com/Ourea-Technologies/ensemble-weather-dotnet/compare/v0.1.0...HEAD
[0.1.0]: https://github.com/Ourea-Technologies/ensemble-weather-dotnet/releases/tag/v0.1.0
