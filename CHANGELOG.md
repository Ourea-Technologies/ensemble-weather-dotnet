# Changelog

All notable changes to this project are documented here. The format follows
[Keep a Changelog](https://keepachangelog.com/en/1.1.0/), and the project uses
[Semantic Versioning](https://semver.org/spec/v2.0.0.html). The SDK version is independent of the API version;
this release targets API contract v1.

## [Unreleased]

## [0.1.0] - unreleased

The first release on NuGet. Targets the v1 contract at ensemble-weather-spec `7954268` (the server's public spec
at `256cd770`, byte for byte), and passes all 58 shared vectors.

### Added

- `OureaClient` for the Ourea API v1: location and coordinate forecasts (`GetLocationForecastAsync`,
  `GetPointForecastAsync`), current conditions, alerts, nearby locations, search, location lists, the catalog,
  model runs and attribution, as `Async` methods with `CancellationToken`, and `IAsyncEnumerable` iterators for
  paged lists. The coordinate methods follow the operation ids: `GetPointCurrentAsync`, `GetPointAlertsAsync`,
  `GetPointNearbyAsync`, beside `GetPointForecastAsync`.
- `ExplorePageAsync` and `ExploreAsync`: places ranked by a metric (`ExploreMetric`: `Snow5d`, `BaseDepth`,
  `SeasonPrecip`, ...), with `Order`, `Type`, `State` (a two-letter code is sent in lowercase), `Near`,
  `RadiusKm`, elevation bounds, `Points` and cursor paging (`ExploreOptions`).
- `GetSeasonAsync` (observed season-to-date precipitation and snowfall; `SeasonOptions.WaterYear`,
  `IncludeDaily`), `GetSnowTrendAsync` (each recent run's daily snowfall; Plus or Pro, else a
  `PermissionDeniedException` with `PLUS_REQUIRED`), `GetSnowpackAsync` and `GetSnowpackPlumeAsync` (beta),
  `GetCompositionAsync` (an open route, sent without the key), and the GeoJSON routes `GetAlertOutlinesAsync`
  (`AlertOutlineOptions.Classes`, `BoundingBox`) and `GetAvalancheZonesAsync`.
- `SearchOptions.Sort`, `Order`, `Near`, `RadiusKm` and `Units`; with a metric sort each hit carries
  `SearchHit.Explore`.
- `ForecastOptions.Members` is a `ForecastMembers`: `true`/`false` convert to it, and `ForecastMembers.Plume`
  asks for the plume view (`ForecastData.Plume`, a `MemberPlume` of columns, bands and a headline, also on
  `ElevationForecast.Plume`); `MembersView.Pool`.
- `GeoPoint` and `BoundingBox` take `near` and `bbox` as numbers; each is sent with literal commas, every number
  in its shortest form, and an out-of-range value or a box with west > east or south > north throws
  `ArgumentException` before anything is sent.
- Models for every schema of the contract: 67 new types (explore, composition, season, snow trend, snowpack,
  GeoJSON, the plume, `ForecastLocation`, `ElevationSummary`, `MemberQuantiles`, the place summary and the 13
  mountain-pass types, normals and departures, ...), and new fields on 25 existing ones
  (`SnowLevelWetBulb`, `CloudCover`, `Measured`, `SnowCharacter`, `ForecastResponse.Avalanche` and
  `.SnowOnGround`, `CurrentConditionsResponse.Summary` and `.Pass`, `SensorObservation.DistanceKm`, ...).
  `SnowpackDefinitions.Surface` and `.LayerForm` are code-to-text dictionaries whose keys are kept as sent.
- New extensible enumerations `SnowCharacter`, `AlertClass`, `PassStatusLevel` and `ExploreMetric`, and new
  values of `Condition` (`LightSnow`, `HeavySnow`), `ConditionIcon` (four snow icons), `AttributionSourceKind`
  (`AvalancheForecast`) and `MembersView` (`Pool`).
- `PLUS_REQUIRED` maps to `PermissionDeniedException`.
- Typed probability terms (`ProbabilityTerm`, `Threshold`, `ForecastWindow`) and window totals
  (`WindowTotalTerm`), sent in the server's canonical order and matched back to the caller's terms by the
  canonical `Term` the server echoes.
- Canonical, edge-cacheable requests: defaults omitted, parameters sorted, comma lists (`sources`,
  `exclude_sources`, `elevations`, `include`, `network`, `capability`, `class`, ...) sorted, de-duplicated and
  joined by literal commas; keyless open routes; an `ETag` LRU cache on by default (pluggable through
  `IResponseCache`); retries with full-jitter backoff and `Retry-After`; same-origin redirects; a 64 MiB body
  bound; typed exceptions per error category (`OureaException` and its subclasses, with `DocUrl`, `Parameter`
  and `NearestLocation`); rate-limit and deprecation metadata.
- `EnsembleWeather.Extensions.DependencyInjection`: `services.AddEnsembleWeather(...)` with
  `IHttpClientFactory`, `IOptions<OureaClientOptions>` and `ILogger`.
- Certificate revocation is checked by default; `OureaClientOptions.CheckCertificateRevocation = false` (also
  through `AddEnsembleWeather`) skips only the CRL/OCSP lookup, for proxies or offline networks that block it.
  Certificate validation itself can never be turned off.
- Targets .NET 8 and .NET Standard 2.0; trimming and Native AOT compatible on .NET 8+.
- The `SnowReport` example, and the mock API serves every new route.

### Names (BEHAVIOR.md 3.0: no stutter)

- The SDK's own types are named after the brand: `OureaClient`, `OureaClientOptions`, `OureaException` and
  `OureaServiceCollectionExtensions` (`services.AddEnsembleWeather()` keeps its name, the DI idiom), and the
  hourly and daily condition is `Condition`.
- JSON fields that repeat their type's word carry the SDK name of the cross-SDK table, and serialising writes
  the JSON name back: `ForecastResponse.Series` (JSON `forecast`) and `.Elevations` (`elevation_forecasts`),
  `CurrentConditionsResponse.Now` (`current`), `CurrentConditions.Code`/`Text`/`Icon`, `ProbabilityResult.Value`,
  `WindowTotal.StartHours`/`EndHours`, `Items` and `Count` on the list responses, `NearbyLocationsResponse.Origin`,
  an elevation's `Height`, `SensorObservation.ObservedAt`, and the rest of the table in the README. Each renamed
  property's XML doc starts with its JSON name.
- `ElevationSummary` (was `LocationElevation`) also holds `ForecastResponse.Location.Elevations`; the forecast's
  `Location` is a `ForecastLocation` (a `LocationReference` plus `Elevation` and `Elevations`).

[Unreleased]: https://github.com/Ourea-Technologies/ensemble-weather-dotnet/compare/v0.1.0...HEAD
[0.1.0]: https://github.com/Ourea-Technologies/ensemble-weather-dotnet/releases/tag/v0.1.0
