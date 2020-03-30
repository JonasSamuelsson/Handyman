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

            projects = projects.ToList();

            var errors = projects.Where(x => x.Errors.Any()).ToList();
            var outdated = projects.Where(x => x.TargetFrameworks.Any()).ToList();
            var upToDate = projects.Where(x => !errors.Contains(x) && !outdated.Contains(x)).ToList();

            if (!projects.Any())
            {
                builder.AppendLine("No projects found");
            }
            else
            {
                builder.AppendLine("# Summary");

                if (errors.Any())
                {
                    builder.AppendLine($"* {errors.Count} projects failed");
                }

                builder.AppendLine($"* {outdated.Count} projects has outdated dependencies");
                builder.AppendLine($"* {upToDate.Count} projects are up to date");
                builder.AppendLine();

                if (errors.Any())
                {
                    builder.AppendLine("# Failed");

                    foreach (var project in errors)
                    {
                        AppendProjectInfo(builder, project);

                        foreach (var error in project.Errors)
                        {
                            builder.AppendLine($"### {error.Stage}");
                            builder.AppendLine(error.Message);
                        }
                    }

                    builder.AppendLine();
                }

                if (outdated.Any())
                {
                    builder.AppendLine("# Outdated");

                    foreach (var project in outdated)
                    {
                        AppendProjectInfo(builder, project);

                        foreach (var framework in project.TargetFrameworks)
                        {
                            builder.AppendLine($"### {framework.Name}");
                            builder.AppendLine("| Package | Current | Major | Minor | Patch |");
                            builder.AppendLine("| - | - | - | - | - | - |");

                            foreach (var package in framework.Packages)
                            {
                                var name = package.IsTransitive
                                    ? $"{package.Name} (transitive)"
                                    : package.Name;

                                builder
                                    .Append($"| {name} |")
                                    .Append($" {package.CurrentVersion} |")
                                    .Append($" {package.AvailableVersions.GetValueOrDefault(Severity.Major)} |")
                                    .Append($" {package.AvailableVersions.GetValueOrDefault(Severity.Minor)} |")
                                    .Append($" {package.AvailableVersions.GetValueOrDefault(Severity.Patch)} |")
                                    .AppendLine();
                            }
                        }
                    }

                    builder.AppendLine();
                }

                if (upToDate.Any())
                {
                    builder.AppendLine("# Up to date");

                    foreach (var project in upToDate)
                    {
                        AppendProjectInfo(builder, project);
                    }

                    builder.AppendLine();
                }
            }

            builder.AppendLine("***");
            builder.AppendLine($"_{AppInfo.AppName} {DateTimeOffset.UtcNow:yyyy-MM-dd HH:mm UTC}_");

            var directory = _fileSystem.Path.GetDirectoryName(path);
            if (!_fileSystem.Directory.Exists(directory))
                _fileSystem.Directory.CreateDirectory(directory);

            _fileSystem.File.WriteAllText(path, builder.ToString(), Encoding.UTF8);
        }

        private static void AppendProjectInfo(StringBuilder builder, Project project)
        {
            builder.AppendLine($"## {project.Name}");
            builder.AppendLine($"Path: {project.RelativePath}  ");

            if (project.Config.Tags.Any())
            {
                builder.AppendLine($"Tags: {string.Join(", ", project.Config.Tags)}");
            }
        }
    }
}