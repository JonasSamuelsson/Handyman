using System;
using System.Collections.Generic;
using System.IO.Abstractions.TestingHelpers;

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

        public static IEnumerable<string> GetLines(this MockFileData file)
        {
            return MockFileData.DefaultEncoding.GetString(file.Contents).Split(Environment.NewLine);
        }
    }
}