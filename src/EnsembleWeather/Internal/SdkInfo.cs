// SPDX-License-Identifier: BSD-2-Clause

using System.Runtime.InteropServices;

namespace EnsembleWeather;

/// <summary>The SDK's identity on the wire.</summary>
internal static class SdkInfo
{
    /// <summary>The SDK version (kept equal to the package version by a unit test).</summary>
    public const string Version = "0.1.0";

    /// <summary>
    /// <c>ensemble-weather-dotnet/0.1.0 (.NET/8.0.11)</c>, plus the application's own product token when it sets one.
    /// It never contains user data.
    /// </summary>
    public static string UserAgent(string? applicationId)
    {
        string agent = $"ensemble-weather-dotnet/{Version} ({Runtime()})";
        return string.IsNullOrWhiteSpace(applicationId) ? agent : agent + " " + applicationId!.Trim();
    }

    private static string Runtime()
    {
        // ".NET 8.0.11" -> ".NET/8.0.11"; ".NET Framework 4.8.9037.0" -> ".NET-Framework/4.8.9037.0".
        string description = RuntimeInformation.FrameworkDescription.Trim();
        int space = description.LastIndexOf(' ');
        return space <= 0
            ? description
            : description.Substring(0, space).Replace(' ', '-') + "/" + description.Substring(space + 1);
    }
}
