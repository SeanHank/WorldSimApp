using System;
using System.IO;

namespace WorldSimApp.Services;

public static class DataPathHelper
{
    private static readonly string[] RelativePaths = { "", "..", "..", "..", "..", "bin/Debug/net8.0" };
    private static readonly string HardcodedBasePath = "/Users/admin/RiderProjects/WorldSimApp";

    public static string? FindFile(string fileName)
    {
        var baseDir = AppDomain.CurrentDomain.BaseDirectory;
        var currentDir = Directory.GetCurrentDirectory();

        foreach (var relative in RelativePaths)
        {
            var path = string.IsNullOrEmpty(relative)
                ? Path.Combine(baseDir, "Data", fileName)
                : Path.Combine(baseDir, relative, "Data", fileName);
            
            var full = Path.GetFullPath(path);
            if (File.Exists(full)) return full;
        }

        foreach (var relative in RelativePaths)
        {
            var path = string.IsNullOrEmpty(relative)
                ? Path.Combine(currentDir, "Data", fileName)
                : Path.Combine(currentDir, relative, "Data", fileName);
            
            var full = Path.GetFullPath(path);
            if (File.Exists(full)) return full;
        }

        var hardcodedPaths = new[]
        {
            Path.Combine(HardcodedBasePath, "bin", "Debug", "net8.0", "Data", fileName),
            Path.Combine(HardcodedBasePath, "Data", fileName),
        };

        foreach (var p in hardcodedPaths)
        {
            if (File.Exists(p)) return p;
        }

        return null;
    }
}
