// SPDX-License-Identifier: BSD-2-Clause

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace EnsembleWeather.Tests.Support;

/// <summary>A request as the SDK sent it.</summary>
internal sealed record RecordedRequest(string Method, Uri Uri, IReadOnlyDictionary<string, string> Headers)
{
    public string? Header(string name)
        => Headers.FirstOrDefault(h => string.Equals(h.Key, name, StringComparison.OrdinalIgnoreCase)).Value;
}

/// <summary>A canned response.</summary>
internal sealed record CannedResponse(int Status, IReadOnlyList<KeyValuePair<string, string>> Headers, byte[] Body);

/// <summary>An in-process transport: answers from a queue, records every request. Never touches the network.</summary>
internal sealed class MockHandler : HttpMessageHandler
{
    private readonly Queue<CannedResponse> _responses = new();
    private readonly Func<HttpRequestMessage, CannedResponse?>? _responder;

    public MockHandler(IEnumerable<CannedResponse>? responses = null, Func<HttpRequestMessage, CannedResponse?>? responder = null)
    {
        foreach (CannedResponse r in responses ?? [])
        {
            _responses.Enqueue(r);
        }

        _responder = responder;
    }

    public List<RecordedRequest> Requests { get; } = [];

    public void Enqueue(CannedResponse response) => _responses.Enqueue(response);

    public static CannedResponse Json(int status, string body, params (string Name, string Value)[] headers)
        => new(
            status,
            [new("Content-Type", "application/json"), .. headers.Select(h => new KeyValuePair<string, string>(h.Name, h.Value))],
            System.Text.Encoding.UTF8.GetBytes(body));

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var headers = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (KeyValuePair<string, HeaderStringValues> h in request.Headers.NonValidated)
        {
            headers[h.Key] = h.Value.ToString();
        }

        Requests.Add(new RecordedRequest(request.Method.Method, request.RequestUri!, headers));
        CannedResponse? canned = _responder?.Invoke(request);
        if (canned is null)
        {
            if (_responses.Count == 0)
            {
                throw new InvalidOperationException($"No canned response left for {request.RequestUri}.");
            }

            canned = _responses.Dequeue();
        }

        var response = new HttpResponseMessage((HttpStatusCode)canned.Status)
        {
            RequestMessage = request,
            Content = new ByteArrayContent(canned.Body),
        };
        foreach (KeyValuePair<string, string> h in canned.Headers)
        {
            if (!response.Headers.TryAddWithoutValidation(h.Key, h.Value))
            {
                response.Content.Headers.TryAddWithoutValidation(h.Key, h.Value);
            }
        }

        return Task.FromResult(response);
    }
}
