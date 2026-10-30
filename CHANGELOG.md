# Changelog

All notable changes to this project are documented here. The format follows
[Keep a Changelog](https://keepachangelog.com/en/1.1.0/), and the project uses
[Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

The v1 contract as deployed on api.oureawx.com (KAN-1561): pinned spec `8b85fea`, the server's public spec
byte for byte. Breaking for callers of the removed and renamed members below.

### Removed

- `GetStormReportsAsync`, `GetStormReportAsync`, `StormReportsResponse` and `StormReport`:
  `/api/v1/storm-reports` is not in the public API.
- The catalog's `Ads` and the `CatalogResolvedAdSlot`, `CatalogHousePlacement`, `CatalogThirdPartyPlacement` and
  `CatalogAdCreative` types: not in the public API.
- `EnsembleStatistics.FeelsLikeUtci`, `.WindDirectionCircular` and `.PrecipitationTypeCode` (`ptype`),
  `DailyForecast.FeelsLikeUtciHigh`/`Low`, `ElevationForecast.Timeseries`/`IsDefault` and
  `ElevationTimeSeriesPoint`: withdrawn by the server.
- The `FORBIDDEN`, `MODEL_RUN_NOT_FOUND`, `GONE`, `INVALID_MAX_HOURS`, `AMBIGUOUS` and `FEELS_LIKE_UNAVAILABLE`
  code mappings: not public codes (an unknown code still keeps its string and maps by HTTP status).

### Changed

- The coordinate methods follow the operation ids: `GetCurrentByCoordinatesAsync` is `GetPointCurrentAsync`,
  `GetAlertsByCoordinatesAsync` is `GetPointAlertsAsync` and `GetNearbyByCoordinatesAsync` is
  `GetPointNearbyAsync`, beside `GetPointForecastAsync`. There are no aliases.
- `EnsembleStatistics.WindDirection` and `ModelStatistics.WindDirection` are `CircularStatistics` (mean,
  standard deviation, 50 % and 90 % sectors), not `Quantiles`.
- Probability and window-total answers are matched to the caller's terms by the canonical `Term` the server
  echoes, never by position.
- The `precip_*` wire names are `precipitation_*` (daily, hourly, current, sensor and threshold fields); the C#
  property names were already spelled out and are unchanged.
- `LocationReference.Slug` and `.Type` are nullable: `null` on a point forecast.
- `CurrentConditions.ConditionIcon` is a `ConditionIcon`; `ObservingStation` gains `StationId`; `ActiveAlert`
  gains `Certainty`, and its `Severity` and `Urgency` are snake_case.
- `GetAvailableRunsAsync(limit, cursor)` pages with `AvailableRunsResponse.NextCursor`.
- Comma lists (`sources`, `elevations`, `network`, `capability`, ...) are sent sorted and de-duplicated, and
  `capability` as one comma list, the server's canonical form. Catalog `network`, `status`, `type` and
  `template` values are snake_case (`snotel`, `active`, `ski_area`, `mountain`).
- `OUTSIDE_COVERAGE` is a 404; `Retry-After` is honoured on `504` as well as `429` and `503`.

### Added

- `EnsembleWeatherException.DocUrl`, `.Parameter` and `.NearestLocation`; `RequestId` is read from the error
  body, else `X-Request-Id`. `AUTH_UNAVAILABLE` maps to `ServiceUnavailableException`.
- `ProbabilityResult.Term`/`.NullReason`, `WindowTotal.Term`/`.Aggregation`/`.NullReason`, `NullReason`,
  `WindowTotalAggregation`, `NearestLocation`.
- `ForecastOptions.Include` (`skin_temperature`, `mslp`): those variables are served only when asked for.
- Four night `ConditionIcon` values (`NightMoonShowers`, `NightChanceRain`, `NightRainLikely`, `NightShowers`).

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
- Certificate revocation is checked by default; `EnsembleWeatherOptions.CheckCertificateRevocation = false`
  (also through `AddEnsembleWeather`) skips only the CRL/OCSP lookup, for proxies or offline networks that
  block it. Certificate validation itself can never be turned off.
- Targets .NET 8 and .NET Standard 2.0; trimming and Native AOT compatible on .NET 8+.

[Unreleased]: https://github.com/Ourea-Technologies/ensemble-weather-dotnet/compare/v0.1.0...HEAD
[0.1.0]: https://github.com/Ourea-Technologies/ensemble-weather-dotnet/releases/tag/v0.1.0
