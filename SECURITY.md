# Security policy

Please report vulnerabilities **privately**, through GitHub's
[private vulnerability reporting](https://github.com/Ourea-Technologies/ensemble-weather-dotnet/security/advisories/new)
for this repository. Do not open a public issue, pull request or discussion for a security problem.

We acknowledge reports within three working days and keep you updated until a fix is released. Fixes ship
in a patch release of the latest minor version.

## What this SDK guarantees

These are tested in `tests/EnsembleWeather.Tests/SecurityTests.cs` and the shared vectors:

- The API key is never logged, never in an exception message or `ToString()`, and never sent to a host other
  than the configured base URL's (a redirect to another origin drops it). The DI package also redacts
  `X-API-Key` from `IHttpClientFactory`'s request logging.
- TLS certificate validation is always on, with revocation checking, and there is no option to turn it off.
- Response bodies are bounded at 64 MiB: a larger body is an error, never an out-of-memory.
- JSON is parsed into fixed, source-generated types only: no code evaluation, no polymorphic deserialisation.

If you supply your own `HttpClient`, keep its handler's `AllowAutoRedirect = false` (the SDK follows redirects
itself so that it can drop the key across origins), and keep the key out of any logging you add.

Supported versions: the latest release.
