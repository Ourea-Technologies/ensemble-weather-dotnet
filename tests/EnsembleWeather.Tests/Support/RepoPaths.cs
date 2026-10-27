// SPDX-License-Identifier: BSD-2-Clause

using System;
using System.IO;

namespace EnsembleWeather.Tests.Support;

/// <summary>Finds the repository's pinned spec from the test output directory.</summary>
internal static class RepoPaths
{
    private static readonly Lazy<string> RootPath = new(() =>
    {
        for (var dir = new DirectoryInfo(AppContext.BaseDirectory); dir is not null; dir = dir.Parent)
        {
            if (File.Exists(Path.Combine(dir.FullName, "spec", "VERSION")))
            {
                return dir.FullName;
            }
        }

        throw new InvalidOperationException("Could not find the repository root (spec/VERSION).");
    });

    public static string Root => RootPath.Value;

    public static string Spec => Path.Combine(Root, "spec");

    public static string Vectors => Path.Combine(Spec, "vectors");

    public static string OpenApi => Path.Combine(Spec, "openapi", "v1", "openapi.yaml");
}
