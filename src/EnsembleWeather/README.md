# EnsembleWeather

Official .NET SDK for the [Ourea](https://oureawx.com) probabilistic weather API: ensemble forecasts for the
contiguous United States where every value is a distribution, with server-computed probabilities and storm totals.

```csharp
using EnsembleWeather;

using var client = new EnsembleWeatherClient(); // anonymous, or OUREA_API_KEY
var options = new ForecastOptions();
options.Probabilities.Add(ProbabilityTerm.AtLeast(ForecastVariable.Snowfall, Threshold.Inches(6)));

ForecastResponse forecast = await client.GetLocationForecastAsync("or/mt-hood-meadows", options);
ProbabilityResult snow = forecast.Probabilities[0];
Console.WriteLine($"P(>= 6 in snow, next 24 h) = {snow.Probability:P0} of {snow.Members} members");
```

- Targets .NET 8+ and .NET Standard 2.0; trimming and Native AOT compatible on .NET 8+.
- Canonical, edge-cacheable requests; an `ETag` cache on by default; retries with backoff; typed errors.
- Dependency injection: [EnsembleWeather.Extensions.DependencyInjection](https://www.nuget.org/packages/EnsembleWeather.Extensions.DependencyInjection).

Documentation: https://docs.oureawx.com/dotnet · Source: https://github.com/Ourea-Technologies/ensemble-weather-dotnet ·
Licence: BSD-2-Clause.
