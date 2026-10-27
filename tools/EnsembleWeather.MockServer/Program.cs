// SPDX-License-Identifier: BSD-2-Clause

// A tiny local stand-in for the Ourea API, so the examples run in CI without a key or the network:
//
//   dotnet run --project tools/EnsembleWeather.MockServer -- 5099
//   OUREA_BASE_URL=http://127.0.0.1:5099 dotnet run --project examples/Quickstart
//
// It serves the shared vector bodies (spec/vectors/bodies) plus a few of its own, answers prob=/totals= terms with
// canned numbers (it is a test double: the real server computes them from the ensemble), and mimics the tier gate:
// the coordinate routes answer 403 PRO_REQUIRED without an X-API-Key and 404 OUTSIDE_COVERAGE outside CONUS.

using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

int port = args.Length > 0 ? int.Parse(args[0], CultureInfo.InvariantCulture) : 5099;
string vectorBodies = FindVectorBodies();
string ownBodies = Path.Combine(AppContext.BaseDirectory, "bodies");

using var listener = new HttpListener();
listener.Prefixes.Add($"http://127.0.0.1:{port}/");
listener.Start();
Console.WriteLine($"mock Ourea API on http://127.0.0.1:{port}/");

while (true)
{
    HttpListenerContext context = await listener.GetContextAsync();
    _ = Task.Run(() => Handle(context));
}

void Handle(HttpListenerContext context)
{
    HttpListenerRequest request = context.Request;
    string path = request.Url!.AbsolutePath;
    var query = System.Web.HttpUtility.ParseQueryString(request.Url.Query);
    bool keyed = !string.IsNullOrEmpty(request.Headers["X-API-Key"]);
    (int status, string body) = Route(path, query, keyed);
    byte[] bytes = Encoding.UTF8.GetBytes(body);
    context.Response.StatusCode = status;
    context.Response.ContentType = "application/json";
    context.Response.Headers["Cache-Control"] = keyed ? "private, max-age=0" : "public, max-age=0, s-maxage=60";
    if (keyed)
    {
        context.Response.Headers["RateLimit-Limit"] = "600";
        context.Response.Headers["RateLimit-Remaining"] = "599";
        context.Response.Headers["RateLimit-Reset"] = "1";
        context.Response.Headers["RateLimit-Policy"] = "600;w=60";
    }

    context.Response.OutputStream.Write(bytes);
    context.Response.Close();
    Console.WriteLine($"{status} {request.Url.PathAndQuery}");
}

(int, string) Route(string path, System.Collections.Specialized.NameValueCollection query, bool keyed)
{
    string[] parts = path.Trim('/').Split('/').Select(Uri.UnescapeDataString).ToArray();
    bool imperial = query["units"] == "imperial";
    if (path is "/api/v1/forecast" or "/api/v1/current" or "/api/v1/alerts")
    {
        if (!keyed)
        {
            return Error(403, "PRO_REQUIRED", "Forecasts by coordinates require a Pro subscription.");
        }

        double lat = double.Parse(query["lat"]!, CultureInfo.InvariantCulture);
        double lon = double.Parse(query["lon"]!, CultureInfo.InvariantCulture);
        if (lat is < 24 or > 50 || lon is < -125 or > -66)
        {
            return Error(404, "OUTSIDE_COVERAGE", $"({lat}, {lon}) is outside the forecast coverage window (lat 24..50, lon -125..-66)");
        }

        return path switch
        {
            "/api/v1/current" => (200, Own("current.json")),
            "/api/v1/alerts" => (200, Own("alerts.json")),
            _ => (200, Forecast(imperial, query)),
        };
    }

    if (parts.Length >= 5 && parts[0] == "api" && parts[2] == "locations")
    {
        if (parts[4] != "mt-hood-meadows")
        {
            return Error(404, "LOCATION_NOT_FOUND", $"No location {parts[3]}/{parts[4]}.");
        }

        return parts.Length == 5 ? (200, Forecast(imperial, query))
            : parts[5] == "current" ? (200, Own("current.json"))
            : parts[5] == "alerts" ? (200, Own("alerts.json"))
            : Error(404, "NOT_FOUND", "No route for " + path);
    }

    return path switch
    {
        "/api/v1/search/locations" => (200, Vector("search.json")),
        "/api/v1/catalog/locations" => (200, Vector(query["cursor"] == "c2" ? "catalog-page-2.json" : "catalog-page-1.json")),
        "/api/v1/catalog/locations/mt-hood-meadows" => (200, Vector("catalog-detail.json")),
        "/api/v1/attribution" => (200, Vector("attribution.json")),
        _ => Error(404, "NOT_FOUND", "No route for " + path),
    };
}

string Forecast(bool imperial, System.Collections.Specialized.NameValueCollection query)
{
    JsonNode body = JsonNode.Parse(Vector(imperial ? "forecast-imperial.json" : "forecast-metric.json"))!;
    var from = new DateTimeOffset(2026, 11, 15, 0, 0, 0, TimeSpan.Zero);
    if (query["prob"] is string prob)
    {
        var answers = new JsonArray();
        foreach (string term in prob.Split(','))
        {
            string[] f = term.Split(':');
            (int start, int end) = Window(f[3]);
            bool amount = f[0] is "precipitation" or "snowfall";
            answers.Add(new JsonObject
            {
                ["variable"] = f[0],
                ["op"] = f[1],
                ["threshold"] = double.Parse(f[2], CultureInfo.InvariantCulture),
                ["window_start_hours"] = start,
                ["window_end_hours"] = end,
                ["aggregation"] = f.Length > 4 ? f[4] : amount ? "total" : "any",
                ["from"] = from.AddHours(start).ToString("yyyy-MM-ddTHH:mm:ssZ", CultureInfo.InvariantCulture),
                ["to"] = from.AddHours(end).ToString("yyyy-MM-ddTHH:mm:ssZ", CultureInfo.InvariantCulture),
                ["probability"] = Math.Round(0.9 / (1 + (Math.Abs(double.Parse(f[2], CultureInfo.InvariantCulture)) / 100)), 3),
                ["members"] = 131,
            });
        }

        body["probabilities"] = answers;
    }

    if (query["totals"] is string totals)
    {
        var answers = new JsonArray();
        foreach (string term in totals.Split(','))
        {
            string[] f = term.Split(':');
            (int start, int end) = Window(f[1]);
            double scale = (end - start) / 24.0 * (imperial ? 0.04 : 1);
            answers.Add(new JsonObject
            {
                ["variable"] = f[0],
                ["window_start_hours"] = start,
                ["window_end_hours"] = end,
                ["from"] = from.AddHours(start).ToString("yyyy-MM-ddTHH:mm:ssZ", CultureInfo.InvariantCulture),
                ["to"] = from.AddHours(end).ToString("yyyy-MM-ddTHH:mm:ssZ", CultureInfo.InvariantCulture),
                ["quantiles"] = new JsonObject
                {
                    ["mean"] = Math.Round(14 * scale, 2),
                    ["p10"] = Math.Round(4 * scale, 2),
                    ["p25"] = Math.Round(8 * scale, 2),
                    ["p50"] = Math.Round(13 * scale, 2),
                    ["p75"] = Math.Round(19 * scale, 2),
                    ["p90"] = Math.Round(26 * scale, 2),
                    ["min"] = 0.0,
                    ["max"] = Math.Round(41 * scale, 2),
                },
                ["members"] = 131,
            });
        }

        body["window_totals"] = answers;
    }

    return body.ToJsonString();
}

static (int Start, int End) Window(string window)
{
    string[] bounds = window.TrimEnd('h').Split('-');
    return bounds.Length == 1
        ? (0, int.Parse(bounds[0], CultureInfo.InvariantCulture))
        : (int.Parse(bounds[0], CultureInfo.InvariantCulture), int.Parse(bounds[1], CultureInfo.InvariantCulture));
}

(int, string) Error(int status, string code, string message)
    => (status, new JsonObject { ["error"] = code, ["message"] = message }.ToJsonString());

string Vector(string name) => File.ReadAllText(Path.Combine(vectorBodies, name));

string Own(string name) => File.ReadAllText(Path.Combine(ownBodies, name));

static string FindVectorBodies()
{
    for (var dir = new DirectoryInfo(AppContext.BaseDirectory); dir is not null; dir = dir.Parent)
    {
        string candidate = Path.Combine(dir.FullName, "spec", "vectors", "bodies");
        if (Directory.Exists(candidate))
        {
            return candidate;
        }
    }

    throw new DirectoryNotFoundException("spec/vectors/bodies not found above the mock server.");
}
