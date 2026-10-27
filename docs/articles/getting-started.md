# Getting started

```sh
dotnet new console -n SnowCheck && cd SnowCheck
dotnet add package EnsembleWeather
```

Replace `Program.cs` with the [quickstart](https://github.com/Ourea-Technologies/ensemble-weather-dotnet/blob/main/examples/Quickstart/Program.cs)
and `dotnet run`. It needs no key: anonymous calls are served at the Free tier.

## Keys

Pass a key to the constructor, or set `OUREA_API_KEY`:

```csharp
using var client = new EnsembleWeatherClient(new EnsembleWeatherOptions
{
    ApiKey = Environment.GetEnvironmentVariable("MY_OUREA_KEY"),
    Units = UnitSystem.Imperial,
});
```

Pro unlocks coordinates (<xref:EnsembleWeather.EnsembleWeatherClient.GetPointForecastAsync*>), the 15-day
horizon, per-model data and pinned runs. See [pricing](https://oureawx.com/pricing).

## Dependency injection

```csharp
builder.Services.AddEnsembleWeather(o => o.ApiKey = builder.Configuration["Ourea:ApiKey"]);
```

The client is a typed `HttpClient` client; one `ETag` cache is shared by every client the container creates.

## One client, reused

`EnsembleWeatherClient` is thread-safe. Create one and keep it: it holds the connection pool and the cache.
