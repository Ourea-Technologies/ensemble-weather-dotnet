# EnsembleWeather.Extensions.DependencyInjection

Registers the official .NET client for the [Ourea](https://oureawx.com) probabilistic weather API
([EnsembleWeather](https://www.nuget.org/packages/EnsembleWeather)) with `Microsoft.Extensions.DependencyInjection`.

```csharp
builder.Services.AddEnsembleWeather(o => o.ApiKey = builder.Configuration["Ourea:ApiKey"]);

// then inject OureaClient
app.MapGet("/snow", async (OureaClient ourea) =>
    (await ourea.GetLocationForecastAsync("or/mt-hood-meadows")).Series.SnowOutlook);
```

The client is a typed `HttpClient` client from `IHttpClientFactory`, configured through
`IOptions<OureaClientOptions>`; deprecation notices are logged through `ILogger`. Documentation:
https://docs.oureawx.com/dotnet. Licence: BSD-2-Clause.
