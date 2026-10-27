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

using var client = new EnsembleWeatherClient();

var options = new ForecastOptions();
options.Probabilities.Add(ProbabilityTerm.AtLeast(ForecastVariable.Snowfall, Threshold.Inches(6), ForecastWindow.Hours(24)));

ForecastResponse forecast = await client.GetLocationForecastAsync("or/mt-hood-meadows", options);

Console.WriteLine($"{forecast.Location.Name}: temperature, next 24 h (p10 to p90, {forecast.UnitLabels.Temperature})");
foreach (TimeSeriesPoint hour in forecast.Forecast.Timeseries)
{
    if (hour.Ensemble.Temperature.GetRange(10, 90) is QuantileRange range)
    {
        Console.WriteLine($"  {hour.ValidTime:ddd HH:mm}Z  {range.Low,6:F1} to {range.High,6:F1}");
    }
}

ProbabilityResult snow = forecast.Probabilities[0];
Console.WriteLine(snow.Probability is double chance
    ? $"Chance of at least 6 in of snow in the next 24 h: {chance:P0} ({snow.Members} members)"
    : "Chance of at least 6 in of snow in the next 24 h: not available (the forecast does not reach 24 h)");
```

This exact program is [`examples/Quickstart`](examples/Quickstart/Program.cs); CI runs it on every change.

## Authentication

Without a key, calls are anonymous and served at the Free tier. To use a key, pass it to the constructor or set
the `OUREA_API_KEY` environment variable (the client reads it when no key is passed):

```csharp
using var client = new EnsembleWeatherClient("ow_...");          // explicit
using var fromEnv = new EnsembleWeatherClient();                 // OUREA_API_KEY, else anonymous
```

| | Anonymous / Free | Pro |
|---|---|---|
| Named locations, current conditions, alerts, search, catalog | yes | yes |
| Forecast horizon | 10 days (240 h) | 15 days (360 h) |
| Members (`Members = true`) | plume view | every variable |
| Per-model breakdowns, pinned runs, coordinates (`GetPointForecastAsync`, ...) | no | yes |

Plans and prices: https://oureawx.com/pricing. The key is sent as `X-API-Key` and never logged or put in an
exception. Search, catalog detail and attribution need no credential, so the SDK sends them **without** the key
and every caller shares one edge-cache entry; set `EnsembleWeatherOptions.SendApiKeyOnOpenRoutes` to attribute
them to your key too.

With dependency injection, add
[EnsembleWeather.Extensions.DependencyInjection](https://www.nuget.org/packages/EnsembleWeather.Extensions.DependencyInjection):

```csharp
builder.Services.AddEnsembleWeather(o => o.ApiKey = builder.Configuration["Ourea:ApiKey"]);
// inject EnsembleWeatherClient (an IHttpClientFactory typed client configured by IOptions<EnsembleWeatherOptions>)
```

## Probabilities, not single numbers

Every forecast value is a distribution (`Quantiles`: `P10` ... `P90`, `Mean`, `Min`, `Max`), computed by the
server from the full ensemble. Ask the server for the probability or the window total you need:

```csharp
// chance: P(at least 6 in of snow in the next 24 h), with its reference class
options.Probabilities.Add(ProbabilityTerm.AtLeast(ForecastVariable.Snowfall, Threshold.Inches(6), ForecastWindow.Hours(24)));
ProbabilityResult p = forecast.Probabilities[0];   // p.Probability, p.Members, p.From, p.To, p.Threshold ...

// window totals: the distribution of each member's own 72 h snowfall total (a storm total)
options.WindowTotals.Add(WindowTotalTerm.Snowfall(ForecastWindow.Hours(72)));
Quantiles? storm = forecast.WindowTotals[0].Quantiles;   // null when the forecast does not cover the window

// quantiles: the served ranges only (p10-p90 or p25-p75), and the median
QuantileRange? likely = hour.Ensemble.Temperature.GetRange(10, 90);
double? median = hour.Ensemble.Temperature.Median;
```

The SDK never sums, averages or interpolates quantiles, and never derives a probability from them: summing
hourly percentiles gives a wrong window total. The only arithmetic it does is converting your own threshold
(6 in on a metric request is sent as 152.40 mm). See https://docs.oureawx.com/concepts/probabilities.

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

Run one with `dotnet run --project examples/ChanceOfSnow`. To run them all against a local mock API instead of
the real one: `scripts/run-examples`.

## Errors, retries and rate limits

Errors are exceptions deriving from `EnsembleWeatherException`, which carries `StatusCode`, the API's
`ErrorCode` (verbatim), `ApiMessage`, `RequestId` and, for `429`, `RetryAfter`:

| Exception | When |
|---|---|
| `AuthenticationFailedException` | `INVALID_API_KEY`, `UNAUTHORIZED`, 401 |
| `PermissionDeniedException` | `PRO_REQUIRED`, `FORBIDDEN`, 403 |
| `ResourceNotFoundException` | `LOCATION_NOT_FOUND`, `NOT_FOUND`, `GONE`, ..., 404/410 |
| `InvalidRequestException` | `BAD_REQUEST`, `INVALID_COORDINATES`, ..., 400 |
| `OutsideCoverageException` | `OUTSIDE_COVERAGE` |
| `RateLimitExceededException` | `TOO_MANY_REQUESTS`, 429 |
| `ServiceUnavailableException` | `FORECAST_UNAVAILABLE`, `TIMEOUT`, 5xx |
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
using var client = new EnsembleWeatherClient(new EnsembleWeatherOptions { CheckCertificateRevocation = false });
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
