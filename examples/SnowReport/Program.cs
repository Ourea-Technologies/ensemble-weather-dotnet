// A snow report: where the most snow is coming, the plume, the season so far, the snowpack, the avalanche danger,
// the winter alerts, the blend's composition and the snow trend (Plus or Pro; an anonymous caller sees why it was
// refused).
using EnsembleWeather;

using var client = new OureaClient();

ExploreResponse top = await client.ExplorePageAsync(ExploreMetric.Snow5d, new ExploreOptions { State = "OR", Limit = 5 });
Console.WriteLine($"Most snow in the next 5 days ({top.Unit}):");
foreach (ExploreItem item in top.Items)
{
    Console.WriteLine($"  {item.Rank}. {item.Location.Name}: {item.Value} (p10 {item.P10}, p90 {item.P90})");
}

var plumeOptions = new ForecastOptions { Members = ForecastMembers.Plume, MaxHours = 48 };
ForecastResponse plume = await client.GetLocationForecastAsync("or/mt-hood-meadows", plumeOptions);
if (plume.Series.Plume is MemberPlume view)
{
    IReadOnlyList<double?> p90 = view.Bands.CumulativeSnowfall.P90;
    Console.WriteLine($"Plume ({plume.MembersView}): {view.Models.Count} models, " +
        $"cumulative snowfall p90 at the end {(p90.Count > 0 ? p90[^1] : null)} {plume.UnitLabels.Amount}");
}

SeasonResponse season = await client.GetSeasonAsync("or/mt-hood-meadows");
if (season.Precipitation is SeasonTotal precipitation)
{
    Console.WriteLine($"Water year {season.WaterYear} through {season.Through:yyyy-MM-dd}: " +
        $"{precipitation.Amount} {precipitation.Unit} of precipitation ({precipitation.PercentOfNormal}% of normal)");
}

SnowpackResponse snowpack = await client.GetSnowpackAsync("or/mt-hood-meadows");
foreach (SnowpackSpot spot in snowpack.Spots)
{
    Console.WriteLine($"  Snowpack at {spot.DisplayName ?? spot.Name ?? "the location"}: {spot.Layers.Count} layers (beta: {snowpack.Beta})");
}

AvalancheZoneCollection zones = await client.GetAvalancheZonesAsync();
int rated = zones.Features.Count(f => f.Properties.DangerLevel is not null);
Console.WriteLine($"{zones.Features.Count} avalanche zones, {rated} rated today");

AlertOutlineCollection outlines = await client.GetAlertOutlinesAsync(new AlertOutlineOptions { Classes = { AlertClass.Winter } });
Console.WriteLine($"{outlines.Features.Count} active winter alert outline(s)");

CompositionResponse composition = await client.GetCompositionAsync();
Console.WriteLine($"The blend: {composition.ModelCount} models, {composition.TotalMembers} members");

try
{
    SnowTrendResponse trend = await client.GetSnowTrendAsync("or/mt-hood-meadows");
    Console.WriteLine($"Snow trend: {trend.Grids.Count} grid(s) over {trend.Dates.Count} days");
}
catch (PermissionDeniedException e) when (e.ErrorCode == "PLUS_REQUIRED")
{
    Console.WriteLine("Snow trend: needs a Plus or Pro key (PLUS_REQUIRED).");
}
