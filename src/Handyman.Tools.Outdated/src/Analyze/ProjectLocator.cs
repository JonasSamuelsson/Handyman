using Handyman.Extensions;
using Handyman.Tools.Outdated.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Text;

namespace Handyman.Tools.Outdated.Analyze
{
    public class ProjectLocator
    {
        private readonly IFileSystem _fileSystem;

        public ProjectLocator(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }
        public IReadOnlyCollection<Project> GetProjects(string path)
        {
            return FindProjects(path)
                .Visit(ApplyConfig)
                .ToList();
        }

        private IEnumerable<Project> FindProjects(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                path = Environment.CurrentDirectory;
            }

            path = _fileSystem.Path.GetFullPath(path);

            if (_fileSystem.File.Exists(path))
            {
                var extension = _fileSystem.Path.GetExtension(path).ToLowerInvariant();

                if (extension != ".csproj")
                {
                    throw new ApplicationException($"Unsupported file type ({extension}).");
                }

                return new[]
                {
                    new Project
                    {
                        FullPath = path,
                        Name = Path.GetFileNameWithoutExtension(path),
                        RelativePath = Path.GetFileName(path)
                    }
                };
            }

            if (_fileSystem.Directory.Exists(path))
            {
                return _fileSystem.Directory.GetFiles(path, "*.csproj", SearchOption.AllDirectories)
                    .Select(x => new Project
                    {
                        FullPath = x,
                        Name = _fileSystem.Path.GetFileNameWithoutExtension(x),
                        RelativePath = x.Remove(0, path.Length).TrimStart(_fileSystem.Path.PathSeparator)
                    });
            }

            throw new ApplicationException($"File or directory '{path}' was not found.");
        }

        private void ApplyConfig(Project project)
        {
            var directory = _fileSystem.Path.GetDirectoryName(project.FullPath);
            var config = GetConfig(directory);
            project.Config = config;
        }

        private ProjectConfig GetConfig(string directory)
        {
            do
            {
                var files = _fileSystem.Directory.GetFiles(directory, ".handyman-outdatad.json",
                    SearchOption.TopDirectoryOnly);

                if (!files.Any())
                {
                    directory = _fileSystem.Path.GetDirectoryName(directory);
                    continue;
                }

                var json = _fileSystem.File.ReadAllText(files.Single(), Encoding.UTF8);
                return JsonConvert.DeserializeObject<ProjectConfig>(json);
            } while (directory != null);

            return new ProjectConfig();
        }
    }
}