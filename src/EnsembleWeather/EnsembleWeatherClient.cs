// SPDX-License-Identifier: BSD-2-Clause

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using EnsembleWeather.Serialization;

namespace EnsembleWeather;

/// <summary>
/// The client for the Ourea probabilistic weather API. One method per API operation; every forecast value is a
/// distribution (<see cref="Quantiles"/>), and probabilities and storm totals are computed by the server.
/// </summary>
/// <remarks>
/// <para>
/// Create one client and reuse it: it is thread-safe and holds a connection pool and an <c>ETag</c> cache. With
/// dependency injection, use <c>services.AddEnsembleWeather(...)</c> from the
/// <c>EnsembleWeather.Extensions.DependencyInjection</c> package instead.
/// </para>
/// <para>
/// Every request is sent in canonical form (defaults omitted, parameters sorted, RFC 3986 encoding) so equal
/// requests share one edge-cache entry. Connection failures, <c>429</c>, <c>502</c>, <c>503</c> and <c>504</c> are
/// retried (<see cref="EnsembleWeatherOptions.MaxRetries"/>); errors are thrown as subclasses of
/// <see cref="EnsembleWeatherException"/>.
/// </para>
/// </remarks>
/// <example>
/// <code>
/// using var client = new EnsembleWeatherClient();
/// ForecastResponse forecast = await client.GetLocationForecastAsync("or/mt-hood-meadows");
/// </code>
/// </example>
public sealed class EnsembleWeatherClient : IDisposable
{
    private readonly HttpClient _http;
    private readonly bool _ownsHttp;
    private readonly ApiPipeline _pipeline;
    private readonly UnitSystem _units;

    /// <summary>
    /// Initializes a client with the default options: the API key from <c>OUREA_API_KEY</c> if set (else anonymous),
    /// metric units, the in-memory cache.
    /// </summary>
    public EnsembleWeatherClient()
        : this(new EnsembleWeatherOptions())
    {
    }

    /// <summary>Initializes a client with an API key and otherwise default options.</summary>
    /// <param name="apiKey">The API key; <see langword="null"/> to read <c>OUREA_API_KEY</c> (or call anonymously).</param>
    public EnsembleWeatherClient(string? apiKey)
        : this(new EnsembleWeatherOptions { ApiKey = apiKey })
    {
    }

    /// <summary>Initializes a client that owns its own HTTP connection pool.</summary>
    /// <param name="options">The options.</param>
    /// <exception cref="ArgumentNullException"><paramref name="options"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">An option is invalid.</exception>
    public EnsembleWeatherClient(EnsembleWeatherOptions options)
        : this(CreateHttpClient(), options ?? throw new ArgumentNullException(nameof(options)), ownsHttpClient: true)
    {
    }

    /// <summary>
    /// Initializes a client on an <see cref="HttpClient"/> you manage (for example from <c>IHttpClientFactory</c>).
    /// The client does not dispose it.
    /// </summary>
    /// <param name="httpClient">
    /// The HTTP client. Its handler must not follow redirects itself (<c>AllowAutoRedirect = false</c>): the SDK
    /// follows them, and drops the API key on a redirect to another host, which an auto-redirecting handler would not.
    /// Its <see cref="HttpClient.Timeout"/> should be infinite or longer than <see cref="EnsembleWeatherOptions.Timeout"/>.
    /// </param>
    /// <param name="options">The options; <see langword="null"/> for the defaults.</param>
    /// <exception cref="ArgumentNullException"><paramref name="httpClient"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">An option is invalid.</exception>
    public EnsembleWeatherClient(HttpClient httpClient, EnsembleWeatherOptions? options = null)
        : this(httpClient ?? throw new ArgumentNullException(nameof(httpClient)), options ?? new EnsembleWeatherOptions(), ownsHttpClient: false)
    {
    }

    private EnsembleWeatherClient(HttpClient httpClient, EnsembleWeatherOptions options, bool ownsHttpClient)
    {
        EnsembleWeatherOptions snapshot = options.Clone();
        _http = httpClient;
        _ownsHttp = ownsHttpClient;
        _units = snapshot.Units;
        _pipeline = new ApiPipeline(httpClient, snapshot, snapshot.Environment ?? Environment.GetEnvironmentVariable);
    }

    /// <summary>Gets the base URL requests go to.</summary>
    public Uri BaseUrl => _pipeline.BaseUrl;

    /// <summary>
    /// Gets the most recent rate-limit state the server reported, or <see langword="null"/> before any response
    /// carried one (edge-cacheable anonymous responses carry none). Each result also has its own
    /// <see cref="ResponseMetadata.RateLimit"/>.
    /// </summary>
    public RateLimitState? RateLimit => _pipeline.LastRateLimit;

    // ---- Forecasts -------------------------------------------------------------------------------------------

    /// <summary>
    /// Gets the ensemble forecast for a named location (<c>getLocationForecast</c>). Works without an API key at the
    /// Free tier.
    /// </summary>
    /// <param name="location">The location, for example <c>"or/mt-hood-meadows"</c>.</param>
    /// <param name="options">Forecast options, probability terms and window totals; <see langword="null"/> for none.</param>
    /// <param name="cancellationToken">Cancels the call.</param>
    /// <returns>The forecast.</returns>
    /// <exception cref="ArgumentException">The location or an option is invalid (nothing was sent).</exception>
    /// <exception cref="EnsembleWeatherException">The API or the connection failed.</exception>
    public Task<ForecastResponse> GetLocationForecastAsync(
        string location, ForecastOptions? options = null, CancellationToken cancellationToken = default)
        => GetLocationForecastAsync(LocationId.Parse(location), options, cancellationToken);

    /// <summary>Gets the ensemble forecast for a named location (<c>getLocationForecast</c>).</summary>
    /// <param name="location">The location.</param>
    /// <param name="options">Forecast options, probability terms and window totals; <see langword="null"/> for none.</param>
    /// <param name="cancellationToken">Cancels the call.</param>
    /// <returns>The forecast.</returns>
    /// <exception cref="ArgumentException">An option is invalid (nothing was sent).</exception>
    /// <exception cref="EnsembleWeatherException">The API or the connection failed.</exception>
    public Task<ForecastResponse> GetLocationForecastAsync(
        LocationId location, ForecastOptions? options = null, CancellationToken cancellationToken = default)
    {
        var request = new ApiRequest("getLocationForecast", LocationPath(location));
        options ??= new ForecastOptions();
        request.AddList("elevations", options.Elevations);
        return ForecastAsync(request, options, cancellationToken);
    }

    /// <summary>
    /// Gets the ensemble forecast at coordinates (<c>getPointForecast</c>). Requires a Pro API key; the server
    /// quantizes the point to 0.01°.
    /// </summary>
    /// <param name="latitude">Latitude, decimal degrees (sent exactly as given).</param>
    /// <param name="longitude">Longitude, decimal degrees, west negative (sent exactly as given).</param>
    /// <param name="elevationMeters">The point's elevation in metres; <see langword="null"/> for the terrain's.</param>
    /// <param name="options">Forecast options, probability terms and window totals; <see langword="null"/> for none.</param>
    /// <param name="cancellationToken">Cancels the call.</param>
    /// <returns>The forecast, with <see cref="ForecastResponse.Point"/> saying where it was computed.</returns>
    /// <exception cref="ArgumentException">An option is invalid (nothing was sent).</exception>
    /// <exception cref="EnsembleWeatherException">The API or the connection failed.</exception>
    public Task<ForecastResponse> GetPointForecastAsync(
        double latitude,
        double longitude,
        double? elevationMeters = null,
        ForecastOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        options ??= new ForecastOptions();
        if (options.Elevations.Count > 0)
        {
            throw new ArgumentException("Elevations apply to named locations only; pass elevationMeters for a point.", nameof(options));
        }

        var request = new ApiRequest("getPointForecast", "/api/v1/forecast")
            .Add("lat", latitude)
            .Add("lon", longitude)
            .Add("elevation_m", elevationMeters);
        return ForecastAsync(request, options, cancellationToken);
    }

    // ---- Current conditions, alerts, nearby -------------------------------------------------------------------

    /// <summary>Gets current sensor observations and active alerts near a location (<c>getCurrentConditions</c>).</summary>
    /// <param name="location">The location, for example <c>"or/mt-hood-meadows"</c>.</param>
    /// <param name="options">Options; <see langword="null"/> for none.</param>
    /// <param name="cancellationToken">Cancels the call.</param>
    /// <returns>The current conditions.</returns>
    /// <exception cref="EnsembleWeatherException">The API or the connection failed.</exception>
    public Task<CurrentConditionsResponse> GetCurrentConditionsAsync(
        string location, CurrentConditionsOptions? options = null, CancellationToken cancellationToken = default)
        => GetCurrentConditionsAsync(LocationId.Parse(location), options, cancellationToken);

    /// <summary>Gets current sensor observations and active alerts near a location (<c>getCurrentConditions</c>).</summary>
    /// <param name="location">The location.</param>
    /// <param name="options">Options; <see langword="null"/> for none.</param>
    /// <param name="cancellationToken">Cancels the call.</param>
    /// <returns>The current conditions.</returns>
    /// <exception cref="EnsembleWeatherException">The API or the connection failed.</exception>
    public Task<CurrentConditionsResponse> GetCurrentConditionsAsync(
        LocationId location, CurrentConditionsOptions? options = null, CancellationToken cancellationToken = default)
        => CurrentAsync(new ApiRequest("getCurrentConditions", LocationPath(location) + "/current"), options, cancellationToken);

    /// <summary>Gets current conditions at coordinates (<c>getCurrentByCoordinates</c>). Requires a Pro API key.</summary>
    /// <param name="latitude">Latitude, decimal degrees.</param>
    /// <param name="longitude">Longitude, decimal degrees, west negative.</param>
    /// <param name="elevationMeters">The point's elevation in metres; <see langword="null"/> for the terrain's.</param>
    /// <param name="options">Options; <see langword="null"/> for none.</param>
    /// <param name="cancellationToken">Cancels the call.</param>
    /// <returns>The current conditions.</returns>
    /// <exception cref="EnsembleWeatherException">The API or the connection failed.</exception>
    public Task<CurrentConditionsResponse> GetCurrentByCoordinatesAsync(
        double latitude,
        double longitude,
        double? elevationMeters = null,
        CurrentConditionsOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        var request = new ApiRequest("getCurrentByCoordinates", "/api/v1/current")
            .Add("lat", latitude)
            .Add("lon", longitude)
            .Add("elevation_m", elevationMeters);
        return CurrentAsync(request, options, cancellationToken);
    }

    /// <summary>Gets the active NWS alerts for a location (<c>getLocationAlerts</c>).</summary>
    /// <param name="location">The location, for example <c>"or/mt-hood-meadows"</c>.</param>
    /// <param name="cancellationToken">Cancels the call.</param>
    /// <returns>The alerts (possibly none).</returns>
    /// <exception cref="EnsembleWeatherException">The API or the connection failed.</exception>
    public Task<AlertsResponse> GetLocationAlertsAsync(string location, CancellationToken cancellationToken = default)
        => GetLocationAlertsAsync(LocationId.Parse(location), cancellationToken);

    /// <summary>Gets the active NWS alerts for a location (<c>getLocationAlerts</c>).</summary>
    /// <param name="location">The location.</param>
    /// <param name="cancellationToken">Cancels the call.</param>
    /// <returns>The alerts (possibly none).</returns>
    /// <exception cref="EnsembleWeatherException">The API or the connection failed.</exception>
    public Task<AlertsResponse> GetLocationAlertsAsync(LocationId location, CancellationToken cancellationToken = default)
        => _pipeline.SendAsync(
            new ApiRequest("getLocationAlerts", LocationPath(location) + "/alerts"),
            EnsembleWeatherJsonContext.Default.AlertsResponse,
            cancellationToken);

    /// <summary>Gets the active NWS alerts at coordinates (<c>getAlertsByCoordinates</c>). Requires a Pro API key.</summary>
    /// <param name="latitude">Latitude, decimal degrees.</param>
    /// <param name="longitude">Longitude, decimal degrees, west negative.</param>
    /// <param name="cancellationToken">Cancels the call.</param>
    /// <returns>The alerts (possibly none).</returns>
    /// <exception cref="EnsembleWeatherException">The API or the connection failed.</exception>
    public Task<AlertsResponse> GetAlertsByCoordinatesAsync(
        double latitude, double longitude, CancellationToken cancellationToken = default)
        => _pipeline.SendAsync(
            new ApiRequest("getAlertsByCoordinates", "/api/v1/alerts").Add("lat", latitude).Add("lon", longitude),
            EnsembleWeatherJsonContext.Default.AlertsResponse,
            cancellationToken);

    /// <summary>Gets a location and its nearest public neighbours (<c>getNearbyLocations</c>).</summary>
    /// <param name="location">The location, for example <c>"or/mt-hood-meadows"</c>.</param>
    /// <param name="options">Options; <see langword="null"/> for the API's defaults (3 within 50 km).</param>
    /// <param name="cancellationToken">Cancels the call.</param>
    /// <returns>The location and its neighbours, nearest first.</returns>
    /// <exception cref="EnsembleWeatherException">The API or the connection failed.</exception>
    public Task<NearbyLocationsResponse> GetNearbyLocationsAsync(
        string location, NearbyOptions? options = null, CancellationToken cancellationToken = default)
        => GetNearbyLocationsAsync(LocationId.Parse(location), options, cancellationToken);

    /// <summary>Gets a location and its nearest public neighbours (<c>getNearbyLocations</c>).</summary>
    /// <param name="location">The location.</param>
    /// <param name="options">Options; <see langword="null"/> for the API's defaults (3 within 50 km).</param>
    /// <param name="cancellationToken">Cancels the call.</param>
    /// <returns>The location and its neighbours, nearest first.</returns>
    /// <exception cref="EnsembleWeatherException">The API or the connection failed.</exception>
    public Task<NearbyLocationsResponse> GetNearbyLocationsAsync(
        LocationId location, NearbyOptions? options = null, CancellationToken cancellationToken = default)
        => _pipeline.SendAsync(
            Nearby(new ApiRequest("getNearbyLocations", LocationPath(location) + "/nearby"), options),
            EnsembleWeatherJsonContext.Default.NearbyLocationsResponse,
            cancellationToken);

    /// <summary>Gets the nearest public locations to coordinates (<c>getNearbyByCoordinates</c>).</summary>
    /// <param name="latitude">Latitude, decimal degrees.</param>
    /// <param name="longitude">Longitude, decimal degrees, west negative.</param>
    /// <param name="options">Options; <see langword="null"/> for the API's defaults (3 within 50 km).</param>
    /// <param name="cancellationToken">Cancels the call.</param>
    /// <returns>The nearest locations, nearest first.</returns>
    /// <exception cref="EnsembleWeatherException">The API or the connection failed.</exception>
    public Task<NearbyPointResponse> GetNearbyByCoordinatesAsync(
        double latitude, double longitude, NearbyOptions? options = null, CancellationToken cancellationToken = default)
        => _pipeline.SendAsync(
            Nearby(new ApiRequest("getNearbyByCoordinates", "/api/v1/nearby").Add("lat", latitude).Add("lon", longitude), options),
            EnsembleWeatherJsonContext.Default.NearbyPointResponse,
            cancellationToken);

    // ---- Search and lists -------------------------------------------------------------------------------------

    /// <summary>
    /// Searches locations by name (<c>searchLocations</c>). Sent without the API key (an open route) unless
    /// <see cref="EnsembleWeatherOptions.SendApiKeyOnOpenRoutes"/> is set.
    /// </summary>
    /// <param name="query">The text, at least 2 characters, for example <c>"hood meadows"</c>.</param>
    /// <param name="options">Hard filters and the hit limit; <see langword="null"/> for none.</param>
    /// <param name="cancellationToken">Cancels the call.</param>
    /// <returns>Ranked hits, best first. Key results by <see cref="SearchHit.Key"/>: slugs are unique per state only.</returns>
    /// <exception cref="ArgumentException"><paramref name="query"/> is null or empty.</exception>
    /// <exception cref="EnsembleWeatherException">The API or the connection failed.</exception>
    public Task<SearchLocationsResponse> SearchLocationsAsync(
        string query, SearchOptions? options = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(query))
        {
            throw new ArgumentException("A search needs a query.", nameof(query));
        }

        var request = new ApiRequest("searchLocations", "/api/v1/search/locations", openRoute: true)
            .Add("q", query)
            .Add("state", options?.State)
            .Add("county", options?.County)
            .Add("type", options?.Type?.Value)
            .Add("limit", options?.Limit, 20);
        return _pipeline.SendAsync(request, EnsembleWeatherJsonContext.Default.SearchLocationsResponse, cancellationToken);
    }

    /// <summary>Gets one page of every forecast location, all states (<c>getAllLocations</c>).</summary>
    /// <param name="options">Filters, page size and cursor; <see langword="null"/> for the first page.</param>
    /// <param name="cancellationToken">Cancels the call.</param>
    /// <returns>The page; <see cref="LocationsResponse.NextCursor"/> is <see langword="null"/> on the last.</returns>
    /// <exception cref="EnsembleWeatherException">The API or the connection failed.</exception>
    public Task<LocationsResponse> GetAllLocationsPageAsync(
        LocationListOptions? options = null, CancellationToken cancellationToken = default)
        => LocationsPageAsync(new ApiRequest("getAllLocations", "/api/v1/locations"), options, options?.Cursor, cancellationToken);

    /// <summary>Lists every forecast location, following the cursor page by page (<c>getAllLocations</c>).</summary>
    /// <param name="options">Filters, page size and starting cursor; <see langword="null"/> for everything.</param>
    /// <param name="cancellationToken">Cancels the enumeration.</param>
    /// <returns>The locations, lazily, in slug order.</returns>
    public IAsyncEnumerable<LocationSummary> GetAllLocationsAsync(
        LocationListOptions? options = null, CancellationToken cancellationToken = default)
        => Paginate(
            options?.Cursor,
            (cursor, ct) => LocationsPageAsync(new ApiRequest("getAllLocations", "/api/v1/locations"), options, cursor, ct),
            page => (page.Locations, page.NextCursor),
            cancellationToken);

    /// <summary>Gets one page of a state's forecast locations (<c>getLocations</c>).</summary>
    /// <param name="state">The state: USPS code (<c>or</c>) or name.</param>
    /// <param name="options">Filters, page size and cursor; <see langword="null"/> for the first page.</param>
    /// <param name="cancellationToken">Cancels the call.</param>
    /// <returns>The page; <see cref="LocationsResponse.NextCursor"/> is <see langword="null"/> on the last.</returns>
    /// <exception cref="EnsembleWeatherException">The API or the connection failed.</exception>
    public Task<LocationsResponse> GetLocationsPageAsync(
        string state, LocationListOptions? options = null, CancellationToken cancellationToken = default)
        => LocationsPageAsync(StateRequest(state), options, options?.Cursor, cancellationToken);

    /// <summary>Lists a state's forecast locations, following the cursor page by page (<c>getLocations</c>).</summary>
    /// <param name="state">The state: USPS code (<c>or</c>) or name.</param>
    /// <param name="options">Filters, page size and starting cursor; <see langword="null"/> for everything.</param>
    /// <param name="cancellationToken">Cancels the enumeration.</param>
    /// <returns>The locations, lazily, in slug order.</returns>
    public IAsyncEnumerable<LocationSummary> GetLocationsAsync(
        string state, LocationListOptions? options = null, CancellationToken cancellationToken = default)
    {
        StateRequest(state);
        return Paginate(
            options?.Cursor,
            (cursor, ct) => LocationsPageAsync(StateRequest(state), options, cursor, ct),
            page => (page.Locations, page.NextCursor),
            cancellationToken);
    }

    // ---- Catalog ----------------------------------------------------------------------------------------------

    /// <summary>
    /// Gets one page of catalog locations (<c>listCatalogLocations</c>). Sent with the API key, so a keyed client
    /// keeps its own (larger) budget.
    /// </summary>
    /// <param name="options">Filters, page size and cursor; <see langword="null"/> for the first page.</param>
    /// <param name="cancellationToken">Cancels the call.</param>
    /// <returns>The page; <see cref="CatalogLocationPage.NextCursor"/> is <see langword="null"/> on the last.</returns>
    /// <exception cref="EnsembleWeatherException">The API or the connection failed.</exception>
    public Task<CatalogLocationPage> GetCatalogLocationsPageAsync(
        CatalogLocationListOptions? options = null, CancellationToken cancellationToken = default)
        => CatalogLocationsPageAsync(options, options?.Cursor, cancellationToken);

    /// <summary>Lists catalog locations, following the cursor page by page (<c>listCatalogLocations</c>).</summary>
    /// <param name="options">Filters, page size and starting cursor; <see langword="null"/> for everything.</param>
    /// <param name="cancellationToken">Cancels the enumeration.</param>
    /// <returns>The catalog locations, lazily.</returns>
    public IAsyncEnumerable<CatalogLocation> GetCatalogLocationsAsync(
        CatalogLocationListOptions? options = null, CancellationToken cancellationToken = default)
        => Paginate(
            options?.Cursor,
            (cursor, ct) => CatalogLocationsPageAsync(options, cursor, ct),
            page => (page.Items, page.NextCursor),
            cancellationToken);

    /// <summary>
    /// Gets a catalog location's detail by slug (<c>getCatalogLocation</c>). Sent without the API key (an open
    /// route) unless <see cref="EnsembleWeatherOptions.SendApiKeyOnOpenRoutes"/> is set.
    /// </summary>
    /// <param name="slug">The slug, for example <c>mt-hood-meadows</c>.</param>
    /// <param name="options">Opt-in extras; <see langword="null"/> for none.</param>
    /// <param name="cancellationToken">Cancels the call.</param>
    /// <returns>The detail.</returns>
    /// <exception cref="EnsembleWeatherException">The API or the connection failed.</exception>
    public Task<CatalogLocationDetail> GetCatalogLocationAsync(
        string slug, CatalogDetailOptions? options = null, CancellationToken cancellationToken = default)
        => _pipeline.SendAsync(
            CatalogDetail("getCatalogLocation", "/api/v1/catalog/locations/", slug, options),
            EnsembleWeatherJsonContext.Default.CatalogLocationDetail,
            cancellationToken);

    /// <summary>
    /// Gets one page of catalog observation stations (<c>listCatalogStations</c>). Sent with the API key.
    /// </summary>
    /// <param name="options">Filters, page size and cursor; <see langword="null"/> for the first page.</param>
    /// <param name="cancellationToken">Cancels the call.</param>
    /// <returns>The page; <see cref="CatalogStationPage.NextCursor"/> is <see langword="null"/> on the last.</returns>
    /// <exception cref="EnsembleWeatherException">The API or the connection failed.</exception>
    public Task<CatalogStationPage> GetCatalogStationsPageAsync(
        CatalogStationListOptions? options = null, CancellationToken cancellationToken = default)
        => CatalogStationsPageAsync(options, options?.Cursor, cancellationToken);

    /// <summary>Lists catalog observation stations, following the cursor page by page (<c>listCatalogStations</c>).</summary>
    /// <param name="options">Filters, page size and starting cursor; <see langword="null"/> for everything.</param>
    /// <param name="cancellationToken">Cancels the enumeration.</param>
    /// <returns>The stations, lazily.</returns>
    public IAsyncEnumerable<CatalogStation> GetCatalogStationsAsync(
        CatalogStationListOptions? options = null, CancellationToken cancellationToken = default)
        => Paginate(
            options?.Cursor,
            (cursor, ct) => CatalogStationsPageAsync(options, cursor, ct),
            page => (page.Items, page.NextCursor),
            cancellationToken);

    /// <summary>
    /// Gets a catalog station's detail by slug (<c>getCatalogStation</c>). Sent without the API key (an open route)
    /// unless <see cref="EnsembleWeatherOptions.SendApiKeyOnOpenRoutes"/> is set.
    /// </summary>
    /// <param name="slug">The slug, for example <c>mt-hood-test-site-snotel</c>.</param>
    /// <param name="options">Opt-in extras; <see langword="null"/> for none.</param>
    /// <param name="cancellationToken">Cancels the call.</param>
    /// <returns>The detail.</returns>
    /// <exception cref="EnsembleWeatherException">The API or the connection failed.</exception>
    public Task<CatalogStationDetail> GetCatalogStationAsync(
        string slug, CatalogDetailOptions? options = null, CancellationToken cancellationToken = default)
        => _pipeline.SendAsync(
            CatalogDetail("getCatalogStation", "/api/v1/catalog/stations/", slug, options),
            EnsembleWeatherJsonContext.Default.CatalogStationDetail,
            cancellationToken);

    // ---- Runs, storm reports, attribution ---------------------------------------------------------------------

    /// <summary>Gets the model runs a pinned <c>run</c> can name, and what a forecast serves now (<c>getAvailableRuns</c>).</summary>
    /// <param name="limit">The most runs wanted (up to 100); <see langword="null"/> for the API's default of 10.</param>
    /// <param name="cancellationToken">Cancels the call.</param>
    /// <returns>The runs, newest first.</returns>
    /// <exception cref="EnsembleWeatherException">The API or the connection failed.</exception>
    public Task<AvailableRunsResponse> GetAvailableRunsAsync(int? limit = null, CancellationToken cancellationToken = default)
        => _pipeline.SendAsync(
            new ApiRequest("getAvailableRuns", "/api/v1/runs").Add("limit", limit, 10),
            EnsembleWeatherJsonContext.Default.AvailableRunsResponse,
            cancellationToken);

    /// <summary>Gets published storm report cards (<c>getStormReports</c>).</summary>
    /// <param name="limit">The most reports wanted (up to 100); <see langword="null"/> for the API's default of 20.</param>
    /// <param name="cancellationToken">Cancels the call.</param>
    /// <returns>The reports, most recent onset first.</returns>
    /// <exception cref="EnsembleWeatherException">The API or the connection failed.</exception>
    public Task<StormReportsResponse> GetStormReportsAsync(int? limit = null, CancellationToken cancellationToken = default)
        => _pipeline.SendAsync(
            new ApiRequest("getStormReports", "/api/v1/storm-reports").Add("limit", limit, 20),
            EnsembleWeatherJsonContext.Default.StormReportsResponse,
            cancellationToken);

    /// <summary>Gets one storm report card (<c>getStormReport</c>).</summary>
    /// <param name="id">The report id.</param>
    /// <param name="cancellationToken">Cancels the call.</param>
    /// <returns>The report.</returns>
    /// <exception cref="EnsembleWeatherException">The API or the connection failed.</exception>
    public Task<StormReport> GetStormReportAsync(long id, CancellationToken cancellationToken = default)
        => _pipeline.SendAsync(
            new ApiRequest("getStormReport", "/api/v1/storm-reports/" + id.ToString(CultureInfo.InvariantCulture)),
            EnsembleWeatherJsonContext.Default.StormReport,
            cancellationToken);

    /// <summary>
    /// Gets every data source, its licence and required notices (<c>getAttribution</c>). Sent without the API key
    /// (an open route) unless <see cref="EnsembleWeatherOptions.SendApiKeyOnOpenRoutes"/> is set.
    /// </summary>
    /// <param name="cancellationToken">Cancels the call.</param>
    /// <returns>The attribution registry.</returns>
    /// <exception cref="EnsembleWeatherException">The API or the connection failed.</exception>
    public Task<AttributionRegistry> GetAttributionAsync(CancellationToken cancellationToken = default)
        => _pipeline.SendAsync(
            new ApiRequest("getAttribution", "/api/v1/attribution", openRoute: true),
            EnsembleWeatherJsonContext.Default.AttributionRegistry,
            cancellationToken);

    /// <summary>Disposes the HTTP connection pool, if the client created it.</summary>
    public void Dispose()
    {
        if (_ownsHttp)
        {
            _http.Dispose();
        }
    }

    // ---- Internals --------------------------------------------------------------------------------------------

    private static HttpClient CreateHttpClient()
    {
        // TLS validation is the platform's, with certificate revocation checked; nothing here can turn it off.
#pragma warning disable CA2000 // The HttpClient owns the handler (disposeHandler: true) and the client disposes the HttpClient.
#if NET5_0_OR_GREATER
        var handler = new SocketsHttpHandler
        {
            AllowAutoRedirect = false,
            AutomaticDecompression = DecompressionMethods.All,
            PooledConnectionLifetime = TimeSpan.FromMinutes(5),
            SslOptions = { CertificateRevocationCheckMode = System.Security.Cryptography.X509Certificates.X509RevocationMode.Online },
        };
#else
        var handler = new HttpClientHandler
        {
            AllowAutoRedirect = false,
            AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
            CheckCertificateRevocationList = true,
        };
#endif
        return new HttpClient(handler, disposeHandler: true) { Timeout = Timeout.InfiniteTimeSpan };
#pragma warning restore CA2000
    }

    private static string LocationPath(LocationId location)
    {
        if (location.State is null)
        {
            throw new ArgumentException("The location is empty.", nameof(location));
        }

        return "/api/v1/locations/" + ApiRequest.Segment(location.CanonicalState) + "/" + ApiRequest.Segment(location.Slug);
    }

    private static ApiRequest StateRequest(string state)
    {
        if (string.IsNullOrEmpty(state))
        {
            throw new ArgumentException("A state is required.", nameof(state));
        }

        return new ApiRequest("getLocations", "/api/v1/locations/" + ApiRequest.Segment(new LocationId(state, "-").CanonicalState));
    }

    private async Task<ForecastResponse> ForecastAsync(ApiRequest request, ForecastOptions options, CancellationToken cancellationToken)
    {
        UnitSystem units = options.Units ?? _units;
        TermCodec.ProbabilityPlan? probabilities = options.Probabilities.Count > 0
            ? TermCodec.PlanProbabilities(options.Probabilities, units)
            : null;
        TermCodec.TotalsPlan? totals = options.WindowTotals.Count > 0 ? TermCodec.PlanTotals(options.WindowTotals) : null;

        request
            .AddUnits(units)
            .Add("max_hours", options.MaxHours, 24)
            .Add("include_past", options.IncludePast, false)
            .Add("models", options.Models, false)
            .Add("members", options.Members, false)
            .Add("daily", options.Daily, true)
            .Add("max_days", options.MaxDays)
            .Add("hourly", options.Hourly, true)
            .Add("run", options.Run?.UtcDateTime.ToString("yyyy-MM-dd'T'HH:mm:ss'Z'", CultureInfo.InvariantCulture))
            .AddList("sources", options.Sources)
            .AddList("exclude_sources", options.ExcludeSources)
            .Add("nowcast", options.Nowcast, true)
            .AddEncoded("prob", probabilities?.Wire)
            .AddEncoded("totals", totals?.Wire)
            .AddList("diagnostics", options.Diagnostics)
            .AddList("feels_like", options.FeelsLike, "nws");

        ForecastResponse forecast = await _pipeline
            .SendAsync(request, EnsembleWeatherJsonContext.Default.ForecastResponse, cancellationToken)
            .ConfigureAwait(false);
        if (probabilities is not null)
        {
            forecast.Probabilities = TermCodec.MapProbabilities(forecast.Probabilities, probabilities);
        }

        if (totals is not null)
        {
            forecast.WindowTotals = TermCodec.MapTotals(forecast.WindowTotals, totals);
        }

        return forecast;
    }

    private Task<CurrentConditionsResponse> CurrentAsync(
        ApiRequest request, CurrentConditionsOptions? options, CancellationToken cancellationToken)
    {
        request
            .AddUnits(options?.Units ?? _units)
            .AddList("sources", options?.Sources ?? [])
            .AddList("exclude_sources", options?.ExcludeSources ?? [])
            .Add("include", options?.IncludeNowcastHours == true ? "nowcast_hours" : null);
        return _pipeline.SendAsync(request, EnsembleWeatherJsonContext.Default.CurrentConditionsResponse, cancellationToken);
    }

    private ApiRequest Nearby(ApiRequest request, NearbyOptions? options)
        => request
            .Add("limit", options?.Limit, 3)
            .Add("radius_km", options?.RadiusKm, 50)
            .AddUnits(options?.Units ?? _units);

    private Task<LocationsResponse> LocationsPageAsync(
        ApiRequest request, LocationListOptions? options, string? cursor, CancellationToken cancellationToken)
    {
        request
            .Add("type", options?.Type?.Value)
            .AddUnits(options?.Units ?? _units)
            .Add("limit", options?.Limit)
            .Add("cursor", cursor);
        return _pipeline.SendAsync(request, EnsembleWeatherJsonContext.Default.LocationsResponse, cancellationToken);
    }

    private Task<CatalogLocationPage> CatalogLocationsPageAsync(
        CatalogLocationListOptions? options, string? cursor, CancellationToken cancellationToken)
    {
        var request = new ApiRequest("listCatalogLocations", "/api/v1/catalog/locations")
            .Add("admin1", options?.Admin1)
            .Add("country", options?.Country)
            .Add("type", options?.Type)
            .Add("template", options?.Template)
            .Add("limit", options?.Limit, 20)
            .Add("cursor", cursor);
        return _pipeline.SendAsync(request, EnsembleWeatherJsonContext.Default.CatalogLocationPage, cancellationToken);
    }

    private Task<CatalogStationPage> CatalogStationsPageAsync(
        CatalogStationListOptions? options, string? cursor, CancellationToken cancellationToken)
    {
        var request = new ApiRequest("listCatalogStations", "/api/v1/catalog/stations")
            .Add("admin1", options?.Admin1)
            .Add("country", options?.Country)
            .AddList("network", options?.Networks ?? [])
            .AddRepeated("capability", options?.Capabilities ?? [])
            .Add("status", options?.Status)
            .Add("limit", options?.Limit, 20)
            .Add("cursor", cursor);
        return _pipeline.SendAsync(request, EnsembleWeatherJsonContext.Default.CatalogStationPage, cancellationToken);
    }

    private static ApiRequest CatalogDetail(string operation, string prefix, string slug, CatalogDetailOptions? options)
    {
        if (string.IsNullOrEmpty(slug))
        {
            throw new ArgumentException("A slug is required.", nameof(slug));
        }

        return new ApiRequest(operation, prefix + ApiRequest.Segment(slug), openRoute: true)
            .AddList("include", options?.Include ?? [])
            .Add("locale", options?.Locale, "en");
    }

    private static async IAsyncEnumerable<TItem> Paginate<TPage, TItem>(
        string? firstCursor,
        Func<string?, CancellationToken, Task<TPage>> fetch,
        Func<TPage, (IReadOnlyList<TItem> Items, string? Next)> read,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        string? cursor = firstCursor;
        var seen = new HashSet<string>(StringComparer.Ordinal);
        while (true)
        {
            TPage page = await fetch(cursor, cancellationToken).ConfigureAwait(false);
            (IReadOnlyList<TItem> items, string? next) = read(page);
            foreach (TItem item in items)
            {
                yield return item;
            }

            if (string.IsNullOrEmpty(next))
            {
                yield break;
            }

            if (!seen.Add(next!))
            {
                throw new EnsembleWeatherException($"The Ourea API returned the cursor '{next}' twice; stopping to avoid a loop.");
            }

            cursor = next;
        }
    }
}
