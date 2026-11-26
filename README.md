# EnsembleWeather for .NET

Official .NET SDK for the [Ourea](https://oureawx.com) probabilistic weather API.

[![CI](https://github.com/Ourea-Technologies/ensemble-weather-dotnet/actions/workflows/ci.yml/badge.svg?branch=main)](https://github.com/Ourea-Technologies/ensemble-weather-dotnet/actions/workflows/ci.yml?query=branch%3Amain)
[![Security](https://github.com/Ourea-Technologies/ensemble-weather-dotnet/actions/workflows/security.yml/badge.svg?branch=main)](https://github.com/Ourea-Technologies/ensemble-weather-dotnet/actions/workflows/security.yml?query=branch%3Amain)
[![NuGet](https://img.shields.io/nuget/v/EnsembleWeather.svg)](https://www.nuget.org/packages/EnsembleWeather)
[![.NET](https://img.shields.io/badge/.NET-8%20%7C%2010%20%7C%20Standard%202.0-512BD4.svg)](https://www.nuget.org/packages/EnsembleWeather#supportedframeworks-body-tab)
[![Licence: BSD-2-Clause](https://img.shields.io/badge/licence-BSD--2--Clause-blue.svg)](LICENSE)
[![Docs](https://img.shields.io/badge/docs-docs.oureawx.com%2Fdotnet-informational.svg)](https://docs.oureawx.com/dotnet)
[![OpenSSF Scorecard](https://api.scorecard.dev/projects/github.com/Ourea-Technologies/ensemble-weather-dotnet/badge)](https://scorecard.dev/viewer/?uri=github.com/Ourea-Technologies/ensemble-weather-dotnet)

## Install

```sh
dotnet add package EnsembleWeather
```

## Quickstart

No signup and no API key: this runs anonymously at the Free tier. Paste it into a new console app
(`dotnet new console`) as `Program.cs`, then `dotnet run`.

```csharp
// Quickstart: an anonymous forecast for Mt. Hood Meadows, Oregon. Prints each of the next 24 hours'
// temperature as a range (10th to 90th percentile) and the chance of at least 6 in (15 cm) of snow
// in the next 24 hours. No API key needed.
using EnsembleWeather;

using var client = new OureaClient();

var options = new ForecastOptions();
options.Probabilities.Add(ProbabilityTerm.AtLeast(ForecastVariable.Snowfall, Threshold.Inches(6), ForecastWindow.Hours(24)));

ForecastResponse forecast = await client.GetLocationForecastAsync("or/mt-hood-meadows", options);

Console.WriteLine($"{forecast.Location.Name}: temperature, next 24 h (p10 to p90, {forecast.UnitLabels.Temperature})");
foreach (TimeSeriesPoint hour in forecast.Series.Timeseries)
{
    if (hour.Ensemble.Temperature.GetRange(10, 90) is QuantileRange range)
    {
        Console.WriteLine($"  {hour.ValidTime:ddd HH:mm}Z  {range.Low,6:F1} to {range.High,6:F1}");
    }
}

ProbabilityResult snow = forecast.Probabilities[0];
Console.WriteLine(snow.Value is double chance
    ? $"Chance of at least 6 in of snow in the next 24 h: {chance:P0} ({snow.Members} members)"
    : $"Chance of at least 6 in of snow in the next 24 h: not available ({snow.NullReason})");
```

This exact program is [`examples/Quickstart`](examples/Quickstart/Program.cs); CI runs it on every change.

## Authentication

Without a key, calls are anonymous and served at the Free tier. To use a key, pass it to the constructor or set
the `OUREA_API_KEY` environment variable (the client reads it when no key is passed):

```csharp
using var client = new OureaClient("ow_...");          // explicit
using var fromEnv = new OureaClient();                 // OUREA_API_KEY, else anonymous
```

| | Anonymous / Free | Pro |
|---|---|---|
| Named locations, current conditions, alerts, search, catalog | yes | yes |
| Forecast horizon | 10 days (240 h) | 15 days (360 h) |
| Members (`Members = true` or `ForecastMembers.Plume`) | the pooled members | every variable |
| Snow trend (`GetSnowTrendAsync`) | no (`PLUS_REQUIRED`) | yes (Plus too) |
| Per-model breakdowns, pinned runs, coordinates (`GetPointForecastAsync`, ...) | no | yes |

Plans and prices: https://oureawx.com/pricing. The key is sent as `X-API-Key` and never logged or put in an
exception. Search, catalog detail, composition and attribution need no credential, so the SDK sends them **without** the key
and every caller shares one edge-cache entry; set `OureaClientOptions.SendApiKeyOnOpenRoutes` to attribute
them to your key too.

With dependency injection, add
[EnsembleWeather.Extensions.DependencyInjection](https://www.nuget.org/packages/EnsembleWeather.Extensions.DependencyInjection):

```csharp
builder.Services.AddEnsembleWeather(o => o.ApiKey = builder.Configuration["Ourea:ApiKey"]);
// inject OureaClient (an IHttpClientFactory typed client configured by IOptions<OureaClientOptions>)
```

## Probabilities, not single numbers

Every forecast value is a distribution (`Quantiles`: `P10` ... `P90`, `Mean`, `Min`, `Max`), computed by the
server from the full ensemble. Ask the server for the probability or the window total you need:

```csharp
// chance: P(at least 6 in of snow in the next 24 h), with its reference class
options.Probabilities.Add(ProbabilityTerm.AtLeast(ForecastVariable.Snowfall, Threshold.Inches(6), ForecastWindow.Hours(24)));
ProbabilityResult p = forecast.Probabilities[0];   // p.Value, p.Members, p.From, p.To, p.Threshold ...

// window totals: the distribution of each member's own 72 h snowfall total (a storm total)
options.WindowTotals.Add(WindowTotalTerm.Snowfall(ForecastWindow.Hours(72)));
Quantiles? storm = forecast.WindowTotals[0].Quantiles;   // null (with NullReason) when it cannot be answered

// quantiles: the served ranges only (p10-p90 or p25-p75), and the median
QuantileRange? likely = hour.Ensemble.Temperature.GetRange(10, 90);
double? median = hour.Ensemble.Temperature.Median;
```

The SDK never sums, averages or interpolates quantiles, and never derives a probability from them: summing
hourly percentiles gives a wrong window total. The only arithmetic it does is converting your own threshold
(6 in on a metric request is sent as 152.40 mm). See https://docs.oureawx.com/concepts/probabilities.

## Names that differ from the API's JSON

A JSON field that repeats a word of the type holding it would read twice (`forecast.Forecast`,
`current.Current`, `probability.Probability`), so the SDK names it differently. There is one table for every
Ourea SDK (ensemble-weather-spec BEHAVIOR.md 3.0); each renamed property's XML doc starts with its JSON name, and
serialising a model writes the JSON names back. Every other field keeps its JSON name in .NET case (with units
spelled out as before: `lat` is `Latitude`, `elevation_m` is `ElevationMeters`). The GeoJSON routes
(`GetAlertOutlinesAsync`, `GetAvalancheZonesAsync`) are passed through untouched: their properties map one to one
to the wire keys, so an avalanche zone's `properties.forecast_url` is `AvalancheZoneProperties.ForecastUrl`, and
the geometry's `Coordinates` is the raw JSON array.

| .NET type | JSON field | .NET property |
|---|---|---|
| `ForecastResponse` | `forecast` | `Series` |
| `ForecastResponse` | `elevation_forecasts` | `Elevations` |
| `CurrentConditionsResponse` | `current` | `Now` |
| `CurrentConditions` | `condition` | `Code` |
| `CurrentConditions` | `condition_text` | `Text` |
| `CurrentConditions` | `condition_icon` | `Icon` |
| `ObservingStation` | `station_id` | `Id` |
| `ProbabilityResult` | `probability` | `Value` |
| `ProbabilityThresholds` | `daily_precipitation_probability` | `DailyPrecipitation` |
| `ProbabilityThresholds` | `daily_precipitation_occurrence_probability` | `DailyPrecipitationOccurrence` |
| `ProbabilityThresholds` | `daily_snow_probability` | `DailySnow` |
| `WindowTotal` | `window_start_hours` | `StartHours` |
| `WindowTotal` | `window_end_hours` | `EndHours` |
| `ModelMembers` | `members` | `Items` |
| `MemberForecast` | `member_id` | `Id` |
| `MemberPlume` | `members` | `Pool` |
| `AlertsResponse` | `alerts` | `Items` |
| `AlertsResponse` | `alert_count` | `Count` |
| `LocationsResponse` | `locations` | `Items` |
| `NearbyLocationsResponse` | `location` | `Origin` |
| `NearbyLocationsResponse` | `locations` | `Items` |
| `NearbyPointResponse` | `locations` | `Items` |
| `AvailableRunsResponse` | `runs` | `Items` |
| `ElevationForecast` | `elevation` | `Height` |
| `ElevationSummary` | `elevation` | `Height` |
| `SnowTrendElevation` | `elevation` | `Height` |
| `CatalogElevation` | `elevation_m` | `HeightMeters` |
| `CatalogStation, CatalogStationDetail` | `source_station_id` | `SourceId` |
| `CatalogStation, CatalogStationDetail` | `station_status` | `Status` |
| `CatalogStation, CatalogStationDetail` | `station_url` | `Url` |
| `CatalogNearbyStation` | `station_status` | `Status` |
| `CatalogTemplateSummary` | `template_variant` | `Variant` |
| `CatalogResolvedEntry` | `resolved_locale` | `Locale` |
| `SensorObservation` | `observation_time` | `ObservedAt` |
| `PassHour` | `snow_level_above_pass` | `SnowLevelMargin` |
| `PassStatusSegment` | `status` | `State` |
| `PassStatusSegment` | `status_text` | `Text` |
| `AvalancheForecast` | `forecast_url` | `Url` |
| `SeasonTotal` | `total` | `Amount` |
| `SnowTrendDay` | `snow` | `Amount` |

The SDK's own types are named after the brand, not the package, so nothing reads twice:
`OureaClient`, `OureaClientOptions`, `OureaException` (`services.AddEnsembleWeather()` keeps the package's name,
the DI idiom).

## Every route

| Method | Route | Access |
|---|---|---|
| `GetLocationForecastAsync`, `GetPointForecastAsync` | a named location's forecast; one at coordinates | anonymous; Pro |
| `GetCurrentConditionsAsync`, `GetPointCurrentAsync` | current conditions (a mountain pass also gets `Pass`) | anonymous; Pro |
| `GetLocationAlertsAsync`, `GetPointAlertsAsync` | active alerts | anonymous; Pro |
| `GetNearbyLocationsAsync`, `GetPointNearbyAsync` | nearby locations | anonymous |
| `SearchLocationsAsync` | search by name, optionally sorted by a metric (`Sort`) and `Near` a point | anonymous |
| `ExplorePageAsync`, `ExploreAsync` | places ranked by a metric (`Snow5d`, `BaseDepth`, `SeasonPrecip`, ...) | anonymous |
| `GetSeasonAsync` | observed season-to-date precipitation and snowfall | anonymous |
| `GetSnowTrendAsync` | each recent run's daily snowfall, run by run | Plus or Pro |
| `GetSnowpackAsync`, `GetSnowpackPlumeAsync` | the modelled snowpack and its snow-depth plume (beta) | anonymous |
| `GetAlertOutlinesAsync`, `GetAvalancheZonesAsync` | GeoJSON for maps: alert outlines, avalanche zones and ratings | anonymous |
| `GetAllLocationsAsync`, `GetLocationsAsync` (and `...PageAsync`) | every location, paged | anonymous |
| `GetCatalogLocationsAsync`, `GetCatalogLocationAsync`, `GetCatalogStationsAsync`, `GetCatalogStationAsync` | the catalog | anonymous |
| `GetAvailableRunsAsync`, `GetCompositionAsync`, `GetAttributionAsync` | served runs, the blend's models and members, data credits | anonymous |

`Members = ForecastMembers.Plume` on a forecast returns the plume view: columns in `forecast.Series.Plume`, ready
to draw (`Members = true` gives each hour's members).

## More examples

| Example | Shows |
|---|---|
| [Quickstart](examples/Quickstart/Program.cs) | the program above, anonymous |
| [PointForecast](examples/PointForecast/Program.cs) | a forecast at coordinates with an elevation (Pro key from `OUREA_API_KEY`) |
| [ChanceOfSnow](examples/ChanceOfSnow/Program.cs) | several probability terms in one call, with their windows and member counts |
| [StormTotal](examples/StormTotal/Program.cs) | window-total distributions for precipitation and snowfall |
| [SearchAndCatalog](examples/SearchAndCatalog/Program.cs) | search, then page through the catalog with the cursor (`await foreach`) |
| [CurrentAndAlerts](examples/CurrentAndAlerts/Program.cs) | current conditions and active alerts for a location |
| [ErrorHandling](examples/ErrorHandling/Program.cs) | catching an out-of-coverage error by its type and code; reading rate-limit state |
| [Units](examples/Units/Program.cs) | the same forecast in metric and imperial |
| [SnowReport](examples/SnowReport/Program.cs) | explore, the plume, season, snowpack, avalanche zones, alert outlines, composition, snow trend |

Run one with `dotnet run --project examples/ChanceOfSnow`. To run them all against a local mock API instead of
the real one: `scripts/run-examples`.

## Errors, retries and rate limits

Errors are exceptions deriving from `OureaException`, which carries `StatusCode`, the API's
`ErrorCode` (verbatim), `ApiMessage`, `RequestId`, `DocUrl` (the code's entry on the docs errors page),
`Parameter` (on a `400` about one query parameter), `NearestLocation` (on a `PRO_REQUIRED` coordinate request: a
named location that answers without a key) and, for `429`/`503`/`504`, `RetryAfter`:

| Exception | When |
|---|---|
| `AuthenticationFailedException` | `INVALID_API_KEY`, `UNAUTHORIZED`, 401 |
| `PermissionDeniedException` | `PRO_REQUIRED`, `PLUS_REQUIRED`, 403 |
| `ResourceNotFoundException` | `LOCATION_NOT_FOUND`, `NOT_FOUND`, 404/410 |
| `InvalidRequestException` | `BAD_REQUEST`, `INVALID_COORDINATES`, ..., 400 |
| `OutsideCoverageException` | `OUTSIDE_COVERAGE` (a 404) |
| `RateLimitExceededException` | `TOO_MANY_REQUESTS`, 429 |
| `ServiceUnavailableException` | `FORECAST_UNAVAILABLE`, `TIMEOUT`, `AUTH_UNAVAILABLE`, ..., 5xx |
| `ServiceConnectionException` | no response: DNS, TLS, reset, timeout |

An invalid argument (a probability term whose aggregation does not fit its variable, an unserved quantile pair)
throws `ArgumentException` before anything is sent, as .NET does for invalid arguments.

Connection failures (except TLS verification), `429`, `502`, `503` and `504` are retried twice by default
(`MaxRetries`), with full-jitter backoff (0.5 s doubling, at most 8 s) or exactly the server's `Retry-After`
(when it is 60 s or less; longer, the SDK throws with `RetryAfter` set). Each attempt times out after 30 s
(`Timeout`).

Rate-limit state is on every result, `result.Metadata.RateLimit` (`Limit`, `Remaining`, `Reset`, `Policy`), and
the latest is on `client.RateLimit`. Read it rather than hard-coding limits. Anonymous responses that the edge
may cache carry none. `result.Metadata` also says whether a cached body was revalidated (`Revalidated`, from the
`ETag` cache that is on by default), the `RequestId`, and any `Deprecation` notice.

### Networking behind proxies

TLS certificates are always validated, and there is no way to turn that off. By default the client also checks
whether the server's certificate has been revoked, which means fetching CRL/OCSP data from the certificate
authority. If a restrictive corporate proxy or an offline network blocks those fetches, every call fails with a
`ServiceConnectionException` ("could not be verified") although the certificate is fine. Only then, skip the
revocation lookup:

```csharp
using var client = new OureaClient(new OureaClientOptions { CheckCertificateRevocation = false });
// or: services.AddEnsembleWeather(o => o.CheckCertificateRevocation = false);
```

The trade-off: a certificate revoked before it expires would still be accepted. The setting applies to the
handler the SDK creates; an `HttpClient` you pass in keeps its own settings (and must not follow redirects
itself). The SDK uses the system proxy settings (`HTTPS_PROXY` and the OS configuration).

## Documentation

- Guides: https://docs.oureawx.com
- This SDK's guide: https://docs.oureawx.com/dotnet
- API reference (generated from the XML docs with DocFX): https://docs.oureawx.com/dotnet/api
- The OpenAPI contract and behaviour spec: [ensemble-weather-spec](https://github.com/Ourea-Technologies/ensemble-weather-spec)
  (pinned copy in [`spec/`](spec/)). This SDK targets API contract **v1**.

## Supported versions

CI tests on .NET 8 and .NET 10. The package targets `net8.0` (trimming and Native AOT compatible) and
`netstandard2.0` (.NET Framework 4.6.2+, Unity, Xamarin and other .NET Standard 2.0 platforms).

## Contributing, security, licence

- Contributing: [CONTRIBUTING.md](CONTRIBUTING.md) (setup, checks, commit style, dependency policy).
- Security: report vulnerabilities privately, see [SECURITY.md](SECURITY.md).
- Licence: [BSD-2-Clause](LICENSE), Copyright (c) 2026 Ourea Technologies.
