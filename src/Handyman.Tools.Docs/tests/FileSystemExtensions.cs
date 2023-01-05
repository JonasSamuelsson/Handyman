using System.Collections.Generic;
using System.IO.Abstractions.TestingHelpers;
using System.Runtime.InteropServices;

namespace Handyman.Tools.Docs.Tests
{
    public static class FileSystemExtensions
    {
        public static void AddFile(this MockFileSystem fileSystem, string path, IEnumerable<string> lines)
        {
            var directory = fileSystem.Path.GetDirectoryName(path);

            if (!fileSystem.Directory.Exists(directory))
            {
                fileSystem.Directory.CreateDirectory(directory);
            }

            fileSystem.File.WriteAllLines(path, lines);
        }

        public static string FixOsNeutralPaths(this string s)
        {
            return RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? s : s.Replace("c:/", "/");
        }
    }
}