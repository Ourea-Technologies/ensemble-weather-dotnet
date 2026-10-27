# Probabilities

A forecast value is a distribution, <xref:EnsembleWeather.Quantiles>, never one number. The server computes
every probability and window total from the full ensemble; the SDK builds the request and carries the answer.

## Chance of an event

```csharp
var options = new ForecastOptions { Units = UnitSystem.Imperial };
options.Probabilities.Add(ProbabilityTerm.AtLeast(ForecastVariable.Snowfall, Threshold.Inches(6), ForecastWindow.Hours(24)));
options.Probabilities.Add(ProbabilityTerm.AtMost(ForecastVariable.Temperature, Threshold.Fahrenheit(32), ForecastWindow.Between(6, 18), ProbabilityAggregation.All));

ForecastResponse forecast = await client.GetLocationForecastAsync("or/mt-hood-meadows", options);
foreach (ProbabilityResult p in forecast.Probabilities) // in the order asked
{
    Console.WriteLine($"{p.Variable} {p.Operator} {p.Threshold}: {p.Probability:P0} of {p.Members} members, {p.From} to {p.To}");
}
```

- A threshold with a unit is converted to the request's units with exact factors and rounded half away from
  zero, as the server rounds. That is the only arithmetic the SDK does.
- `precipitation` and `snowfall` are window totals (`total`, the only aggregation for them); the others take
  `any` (the default) or `all`. A mismatch throws `ArgumentException` before anything is sent.
- A `null` `Probability` means the forecast does not reach the window's end. It is never 0.

## Storm totals

```csharp
options.WindowTotals.Add(WindowTotalTerm.Snowfall(ForecastWindow.Hours(72)));
WindowTotal total = forecast.WindowTotals[0];
QuantileRange? range = total.Quantiles?.GetRange(10, 90);
```

Each member's own total over the window, as quantiles. Never sum hourly quantiles: percentiles are not
additive.

## Ranges and the median

`GetRange(10, 90)` and `GetRange(25, 75)` are the served ranges; any other pair throws, because the SDK never
interpolates. `Median` is `P50`, the ensemble median, not "the forecast". `wind_direction` quantiles are
circular: `Max` may be less than `Min`.
