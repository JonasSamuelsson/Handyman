using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;

namespace Handyman.Tools.Docs.Shared;

public static class FileSystemExtensions
{
    public static IReadOnlyList<string> GetMarkdownFilePaths(this IFileSystem fileSystem, string targetPath)
    {
        targetPath = string.IsNullOrWhiteSpace(targetPath)
            ? fileSystem.Directory.GetCurrentDirectory()
            : fileSystem.Path.GetFullPath(targetPath);

        if (fileSystem.File.Exists(targetPath))
        {
            return new[] { targetPath };
        }

        if (fileSystem.Directory.Exists(targetPath))
        {
            return fileSystem.Directory.GetFiles(targetPath, "*.md", SearchOption.AllDirectories);
        }

        throw new NotImplementedException();
    }

    public static string GetGitRepoDirectory(this IFileSystem fileSystem, string path)
    {
        if (!fileSystem.Path.IsPathFullyQualified(path))
        {
            throw new ArgumentException("Provided path is not fully qualified.");
        }

        while (true)
        {
            var gitDirectory = fileSystem.Path.Combine(path, ".git");

            if (fileSystem.Directory.Exists(gitDirectory))
            {
                return path;
            }

            path = fileSystem.Path.GetDirectoryName(path);

            if (path == null)
            {
                throw new AppException("Parent git repo directory not found.");
            }
        }
    }

    public static string ConstructPathRelativeToFile(this IFileSystem fileSystem, string referencePath, string relativePath)
    {
        if (!fileSystem.Path.IsPathFullyQualified(referencePath))
        {
            throw new Exception("Reference path is not fully qualified.");
        }

        if (fileSystem.Path.IsPathFullyQualified(relativePath))
        {
            throw new Exception("Relative path is fully qualified.");
        }

        var separator = fileSystem.Path.DirectorySeparatorChar;
        var altSeparator = fileSystem.Path.AltDirectorySeparatorChar;

        referencePath = referencePath.Replace(altSeparator, separator);
        relativePath = relativePath.Replace(altSeparator, separator);

        var directoryPath = relativePath.StartsWith(separator)
            ? fileSystem.GetGitRepoDirectory(referencePath)
            : fileSystem.Path.GetDirectoryName(referencePath);

        relativePath = relativePath.TrimStart(separator);

        var path = fileSystem.Path.Combine(directoryPath, relativePath);

        var segments = path
            .Replace(altSeparator, separator)
            .Split(separator, StringSplitOptions.RemoveEmptyEntries)
            .ToList();

        segments.RemoveAll(s => s == ".");

        for (var i = segments.Count - 1; i > 0; i--)
        {
            var segment = segments[i];

            if (segment != "..")
            {
                continue;
            }

            if (i == 1)
            {
                throw new AppException("Resulting path is outside of the root directory.");
            }

            segments.RemoveAt(i);

            var parent = segments[i - 1];

            if (parent == "..")
            {
                continue;
            }

            segments.RemoveAt(i - 1);
            i--;
        }

        return string.Join(separator, segments);
    }
}