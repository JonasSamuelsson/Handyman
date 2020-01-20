using Handyman.Tools.Outdated.Model;
using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;
using System.Text;

namespace Handyman.Tools.Outdated.Analyze
{
    public class MarkdownFileWriter : IFileWriter
    {
        private readonly IFileSystem _fileSystem;

        public MarkdownFileWriter(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }

        public bool CanHandle(string extension)
        {
            return extension == ".md";
        }

        public void Write(string path, IEnumerable<Project> projects)
        {
            var builder = new StringBuilder();

            foreach (var project in projects)
            {
                builder.AppendLine($"# {project.Name}");
                builder.AppendLine();
                builder.AppendLine($"Path: {project.RelativePath}  ");

                if (project.Config.Tags.Any())
                {
                    builder.AppendLine($"Tags: {string.Join(", ", project.Config.Tags)}");
                }

                builder.AppendLine();

                foreach (var framework in project.TargetFrameworks)
                {
                    builder.AppendLine($"## {framework.Name}");
                    builder.AppendLine();
                    builder.AppendLine("| Package | Current | Major | Minor | Patch | Transitive |");
                    builder.AppendLine("| - | - | - | - | - | - |");

                    foreach (var package in framework.Packages)
                    {
                        builder
                            .Append($"| {package.Name} |")
                            .Append($" {package.CurrentVersion} |")
                            .Append($" {package.AvailableVersions.GetValueOrDefault(Severity.Major)} |")
                            .Append($" {package.AvailableVersions.GetValueOrDefault(Severity.Minor)} |")
                            .Append($" {package.AvailableVersions.GetValueOrDefault(Severity.Patch)} |")
                            .Append($" {package.IsTransitive.ToString().ToLowerInvariant()} |")
                            .AppendLine();
                    }

                    builder.AppendLine();
                }
            }

            if (builder.Length == 0)
            {
                builder.AppendLine("All projects are up to date. =)");
            }

            builder.AppendLine("***");
            builder.AppendLine($"_{AppInfo.AppName} {DateTimeOffset.UtcNow:yyyy-MM-dd HH:mm UTC}_");

            var directory = _fileSystem.Path.GetDirectoryName(path);
            if (!_fileSystem.Directory.Exists(directory))
                _fileSystem.Directory.CreateDirectory(directory);

            _fileSystem.File.WriteAllText(path, builder.ToString(), Encoding.UTF8);
        }
    }
}