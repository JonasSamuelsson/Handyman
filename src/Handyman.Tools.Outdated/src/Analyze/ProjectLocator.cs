﻿using Handyman.Extensions;
using Handyman.Tools.Outdated.Model;
using Handyman.Tools.Outdated.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;

namespace Handyman.Tools.Outdated.Analyze
{
    public class ProjectLocator
    {
        private readonly IFileSystem _fileSystem;
        private readonly ConfigReader _configReader;

        public ProjectLocator(IFileSystem fileSystem, ConfigReader configReader)
        {
            _fileSystem = fileSystem;
            _configReader = configReader;
        }

        public IReadOnlyCollection<Project> GetProjects(string path, IReadOnlyCollection<string> tags)
        {
            return FindProjects(path)
                .Visit(ApplyConfig)
                .Where(x => x.Config.Skip == false)
                .Where(x => TagsFilter.IsMatch(x, tags))
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
                    .OrderBy(x => x.ToLowerInvariant())
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
            project.Config = _configReader.GetConfig(directory);
        }
    }
}