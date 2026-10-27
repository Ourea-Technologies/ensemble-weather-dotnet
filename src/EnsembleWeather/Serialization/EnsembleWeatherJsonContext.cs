// SPDX-License-Identifier: BSD-2-Clause

using System.Text.Json.Serialization;

namespace EnsembleWeather.Serialization;

/// <summary>
/// Source-generated JSON metadata for every response type, so the SDK needs no reflection (trimming and
/// Native AOT safe) and never deserialises to a type it did not name (no polymorphic deserialisation).
/// </summary>
[JsonSourceGenerationOptions(
    GenerationMode = JsonSourceGenerationMode.Metadata,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull)]
[JsonSerializable(typeof(ForecastResponse))]
[JsonSerializable(typeof(CurrentConditionsResponse))]
[JsonSerializable(typeof(AlertsResponse))]
[JsonSerializable(typeof(NearbyLocationsResponse))]
[JsonSerializable(typeof(NearbyPointResponse))]
[JsonSerializable(typeof(SearchLocationsResponse))]
[JsonSerializable(typeof(LocationsResponse))]
[JsonSerializable(typeof(CatalogLocationPage))]
[JsonSerializable(typeof(CatalogLocationDetail))]
[JsonSerializable(typeof(CatalogStationPage))]
[JsonSerializable(typeof(CatalogStationDetail))]
[JsonSerializable(typeof(AvailableRunsResponse))]
[JsonSerializable(typeof(StormReportsResponse))]
[JsonSerializable(typeof(StormReport))]
[JsonSerializable(typeof(AttributionRegistry))]
[JsonSerializable(typeof(ErrorBody))]
internal sealed partial class EnsembleWeatherJsonContext : JsonSerializerContext
{
}
