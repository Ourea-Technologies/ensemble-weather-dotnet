# Snow, explore and map layers

The 0.1.0 contract adds the routes a snow report is built from. Each is one method on `OureaClient`; every
value is computed by the server, and the SDK only sends the request in canonical form and decodes the answer.

## Rank places: explore

```csharp
ExploreResponse page = await client.ExplorePageAsync(ExploreMetric.Snow5d, new ExploreOptions
{
    State = "OR",                          // sent lowercase: state=or
    Near = new GeoPoint(45.33, -121.66),   // sent as near=45.33,-121.66 (a literal comma)
    RadiusKm = 50,
});

await foreach (ExploreItem item in client.ExploreAsync(ExploreMetric.BaseDepth))
{
    Console.WriteLine($"{item.Rank}. {item.Location.Name}: {item.Value} {page.Unit}");
}
```

`ExplorePageAsync` returns one page; `ExploreAsync` follows `NextCursor` lazily. `SearchLocationsAsync` takes the
same metrics as `SearchOptions.Sort`, and each hit then carries `SearchHit.Explore` (which can be `null`).

## The plume view

```csharp
ForecastResponse forecast = await client.GetLocationForecastAsync(
    "or/mt-hood-meadows", new ForecastOptions { Members = ForecastMembers.Plume });
MemberPlume plume = forecast.Series.Plume!;   // columns: plume.Headline, plume.Bands, plume.Models, plume.Pool
```

With the plume view, `forecast.Series.Timeseries` is empty. `ForecastResponse.MembersView` names the member set the
tier carries (`Full` for Pro, `Pool` for anonymous callers), not the layout. A plume column is
`IReadOnlyList<double?>`: a `null` is a member or an hour with no value.

## Season, snow trend and snowpack

- `GetSeasonAsync`: the water year's observed precipitation and snowfall (`SeasonOptions.IncludeDaily` for the
  day-by-day record).
- `GetSnowTrendAsync`: each recent run's daily snowfall, one grid per source. Plus or Pro: anonymous and Free callers
  get a `PermissionDeniedException` with `ErrorCode` `PLUS_REQUIRED`. A day a run does not cover is `null`.
- `GetSnowpackAsync` and `GetSnowpackPlumeAsync` (beta, `Beta` is `true`): the modelled snowpack at a location's
  spots. `SnowpackResponse.Definitions.Surface` and `.LayerForm` map codes (`buried_surface_hoar`) to text; the keys
  are kept exactly as sent.

## Map layers (GeoJSON)

```csharp
AlertOutlineCollection outlines = await client.GetAlertOutlinesAsync(new AlertOutlineOptions
{
    Classes = { AlertClass.Winter, AlertClass.Wind },          // class=wind,winter
    BoundingBox = new BoundingBox(-125, 41, -116, 49),         // bbox=-125,41,-116,49
});
AvalancheZoneCollection zones = await client.GetAvalancheZonesAsync();
```

Both bodies are passed through untouched: every property maps one to one to its wire key (an avalanche zone's
`properties.forecast_url` is `AvalancheZoneProperties.ForecastUrl`, although `ForecastResponse.Avalanche.Url`
renames it), and `Geometry.Coordinates` is the raw `JsonElement`, never reprojected or simplified, so
`JsonSerializer.Serialize(outlines)` gives GeoJSON a map library can read.

`GetCompositionAsync` returns the blend's served models and members; it is an open route, sent without the key.
