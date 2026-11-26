// Current conditions and alerts: the latest sensor readings and active NWS alerts near a location.
using EnsembleWeather;

using var client = new OureaClient();

CurrentConditionsResponse current = await client.GetCurrentConditionsAsync("or/mt-hood-meadows");
if (current.Now is CurrentConditions now)
{
    Console.WriteLine($"{current.Location.Name} at {now.Time:HH:mm}Z: {now.Text}, {now.Temperature} {UnitLabels.For(current.Units).Temperature} ({now.Source})");
}

foreach (SensorObservation sensor in current.Sensors)
{
    Console.WriteLine($"  {sensor.Name} ({sensor.Source}): {sensor.Temperature} at {sensor.ObservedAt:HH:mm}Z");
}

AlertsResponse alerts = await client.GetLocationAlertsAsync("or/mt-hood-meadows");
Console.WriteLine(alerts.Count == 0 ? "No active alerts." : $"{alerts.Count} active alert(s):");
foreach (ActiveAlert alert in alerts.Items)
{
    Console.WriteLine($"  {alert.Event} ({alert.Severity}) until {alert.Expires:ddd HH:mm}Z: {alert.Headline}");
}
