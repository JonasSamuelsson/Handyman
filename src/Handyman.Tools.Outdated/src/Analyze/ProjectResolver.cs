using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Handyman.Tools.Outdated.Model;

namespace Handyman.Tools.Outdated.Analyze
{
    public class ProjectResolver
    {
        public IReadOnlyCollection<Project> GetProjects(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                path = Environment.CurrentDirectory;
            }

            path = Path.GetFullPath(path);

            if (File.Exists(path))
            {
                var extension = Path.GetExtension(path).ToLowerInvariant();

                if (extension != ".csproj")
                {
                    throw new ApplicationException($"Unsupported file type ({extension}).");
                }

                return new[] {new Project
                {
                    FullPath = path,
                    RelativePath = Path.GetFileName(path)
                }};
            }

            if (Directory.Exists(path))
            {
                return Directory.GetFiles(path, "*.csproj", SearchOption.AllDirectories)
                    .Select(x => new Project
                    {
                        FullPath = x,
                        RelativePath = x.Remove(0, path.Length + 1)
                    })
                    .ToList();
            }

            throw new ApplicationException($"File or directory '{path}' was not found.");
        }
    }
}