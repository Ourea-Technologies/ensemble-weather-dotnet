# Errors, retries and caching

Every API or transport error derives from <xref:EnsembleWeather.EnsembleWeatherException>. Catch a category by
type, or test `ErrorCode` (the API's code, verbatim):

```csharp
try
{
    await client.GetPointForecastAsync(51.5, -0.12);
}
catch (OutsideCoverageException ex)
{
    Console.WriteLine($"{ex.ErrorCode}: {ex.ApiMessage} (request {ex.RequestId})");
}
```

## Retries

Connection failures (except TLS verification), `429`, `502`, `503` and `504` are retried `MaxRetries` times
(default 2) with full-jitter backoff, or after exactly `Retry-After` (on a `429`, `503` or `504`) when the server
sends 60 s or less. `RequestId` comes from the error body (else `X-Request-Id`), `DocUrl` links the code's
explanation, and `Parameter` names the query parameter a `400` is about.

## Rate limits

`result.Metadata.RateLimit` and `client.RateLimit` report `RateLimit-Limit`, `-Remaining`, `-Reset` and
`-Policy`. Anonymous, edge-cacheable responses carry none.

## Caching

Requests are canonical (defaults omitted, parameters sorted, RFC 3986 encoding), so equal requests share one
edge-cache entry. Locally, an `ETag` LRU cache (256 entries, 32 MiB) is on by default: a repeated call sends
`If-None-Match`, and a `304` returns the stored body with `Metadata.Revalidated` set. Disable it with
`EnableCache = false`, or supply your own <xref:EnsembleWeather.IResponseCache>.

## TLS and proxies

Certificates are always validated. Revocation is checked by default; behind a proxy or on an offline network that
blocks CRL/OCSP fetches, set <xref:EnsembleWeather.EnsembleWeatherOptions.CheckCertificateRevocation> to `false`
to skip only that lookup (a certificate revoked before it expires would then be accepted).
