// SPDX-License-Identifier: BSD-2-Clause

using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Net.Sockets;
using System.Reflection;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace EnsembleWeather.Tests;

/// <summary>
/// TLS (SDK-STANDARD "Security"): certificate validation is never optional; only the revocation lookup is, through
/// <see cref="EnsembleWeatherOptions.CheckCertificateRevocation"/>.
/// </summary>
public sealed class TlsTests
{
    [Fact]
    public void NoOptionSuggestsTlsCanBeDisabled()
    {
        string[] forbidden = ["tls", "ssl", "insecure", "verify", "verification", "validat", "skip", "disable", "ignore", "trust", "callback", "handler"];
        foreach (PropertyInfo property in typeof(EnsembleWeatherOptions).GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            Assert.DoesNotContain(forbidden, f => property.Name.Contains(f, StringComparison.OrdinalIgnoreCase));
            if (property.Name.Contains("certificate", StringComparison.OrdinalIgnoreCase))
            {
                // The one certificate setting is about revocation only, and is a positive "check" switch.
                Assert.Equal(nameof(EnsembleWeatherOptions.CheckCertificateRevocation), property.Name);
                Assert.Equal(typeof(bool), property.PropertyType);
            }
        }
    }

    [Fact]
    public void RevocationCheckingIsOnByDefault()
    {
        Assert.True(new EnsembleWeatherOptions().CheckCertificateRevocation);

        SocketsHttpHandler handler = HandlerOf(new EnsembleWeatherOptions { Environment = _ => null });

        Assert.Equal(X509RevocationMode.Online, handler.SslOptions.CertificateRevocationCheckMode);
        Assert.Null(handler.SslOptions.RemoteCertificateValidationCallback);
        Assert.False(handler.AllowAutoRedirect);
    }

    [Fact]
    public void TheOptionTurnsOffOnlyTheRevocationCheck()
    {
        SocketsHttpHandler on = HandlerOf(new EnsembleWeatherOptions { Environment = _ => null });
        SocketsHttpHandler off = HandlerOf(new EnsembleWeatherOptions { Environment = _ => null, CheckCertificateRevocation = false });

        Assert.Equal(X509RevocationMode.NoCheck, off.SslOptions.CertificateRevocationCheckMode);
        Assert.Null(off.SslOptions.RemoteCertificateValidationCallback); // validation stays the platform's
        Assert.Equal(on.SslOptions.EnabledSslProtocols, off.SslOptions.EnabledSslProtocols);
        Assert.Equal(on.SslOptions.EncryptionPolicy, off.SslOptions.EncryptionPolicy);
        Assert.Equal(on.AllowAutoRedirect, off.AllowAutoRedirect);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void TheDependencyInjectionHandlerFollowsTheOption(bool check)
    {
        var services = new ServiceCollection();
        services.AddEnsembleWeather(o => o.CheckCertificateRevocation = check);
        using ServiceProvider provider = services.BuildServiceProvider();

        HttpMessageHandler handler = provider.GetRequiredService<IHttpMessageHandlerFactory>()
            .CreateHandler(EnsembleWeatherServiceCollectionExtensions.HttpClientName);
        while (handler is DelegatingHandler delegating)
        {
            handler = delegating.InnerHandler!;
        }

        var sockets = Assert.IsType<SocketsHttpHandler>(handler);
        Assert.Equal(check ? X509RevocationMode.Online : X509RevocationMode.NoCheck, sockets.SslOptions.CertificateRevocationCheckMode);
        Assert.Null(sockets.SslOptions.RemoteCertificateValidationCallback);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task AnUntrustedCertificateFailsEitherWay(bool checkRevocation)
    {
        // A real TLS server on loopback with a self-signed certificate: the handshake must fail, and the failure is
        // a TLS verification error, which is never retried.
        using X509Certificate2 certificate = SelfSigned();
        using var listener = new TcpListener(IPAddress.Loopback, 0);
        listener.Start();
        int port = ((IPEndPoint)listener.LocalEndpoint).Port;
#pragma warning disable CA2025 // The server task is awaited below, before the listener and certificate are disposed.
        Task server = ServeOnceAsync(listener, certificate);
#pragma warning restore CA2025

        using var client = new EnsembleWeatherClient(new EnsembleWeatherOptions
        {
            BaseUrl = new Uri($"https://127.0.0.1:{port}"),
            CheckCertificateRevocation = checkRevocation,
            Environment = _ => null,
            Timeout = TimeSpan.FromSeconds(10),
        });

        ServiceConnectionException ex = await Assert.ThrowsAsync<ServiceConnectionException>(() => client.GetAttributionAsync());

        Assert.Contains("could not be verified", ex.Message, StringComparison.Ordinal);
        await server;
    }

    private static SocketsHttpHandler HandlerOf(EnsembleWeatherOptions options)
    {
        using var client = new EnsembleWeatherClient(options);
        var http = (HttpClient)typeof(EnsembleWeatherClient).GetField("_http", BindingFlags.NonPublic | BindingFlags.Instance)!.GetValue(client)!;
        return (SocketsHttpHandler)typeof(HttpMessageInvoker).GetField("_handler", BindingFlags.NonPublic | BindingFlags.Instance)!.GetValue(http)!;
    }

    private static async Task ServeOnceAsync(TcpListener listener, X509Certificate2 certificate)
    {
        try
        {
            using TcpClient connection = await listener.AcceptTcpClientAsync();
            using var tls = new SslStream(connection.GetStream());
            await tls.AuthenticateAsServerAsync(certificate);
        }
        catch (Exception ex) when (ex is IOException or System.Security.Authentication.AuthenticationException or SocketException)
        {
            // The client aborts the handshake, as it must.
        }
    }

    private static X509Certificate2 SelfSigned()
    {
        using var key = RSA.Create(2048);
        var request = new CertificateRequest("CN=127.0.0.1", key, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        var san = new SubjectAlternativeNameBuilder();
        san.AddIpAddress(IPAddress.Loopback);
        request.CertificateExtensions.Add(san.Build());
        using X509Certificate2 ephemeral = request.CreateSelfSigned(DateTimeOffset.UtcNow.AddDays(-1), DateTimeOffset.UtcNow.AddDays(1));
        byte[] pfx = ephemeral.Export(X509ContentType.Pfx);
#if NET9_0_OR_GREATER
        return X509CertificateLoader.LoadPkcs12(pfx, null);
#else
        return new X509Certificate2(pfx);
#endif
    }
}
