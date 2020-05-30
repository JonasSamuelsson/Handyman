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
            var needsAttention = projects.Where(x => x.TargetFrameworks.Any()).ToList();
            var upToDate = projects.Where(x => !errors.Contains(x) && !needsAttention.Contains(x)).ToList();

            if (!projects.Any())
            {
                builder.AppendLine("No projects found");
            }
            else
            {
                builder.AppendLine("# Summary");

                if (errors.Any())
                {
                    builder.AppendLine($"* {errors.Count} projects could not be analyzed");
                }

                if (needsAttention.Any())
                {
                    builder.AppendLine($"* {needsAttention.Count} projects needs attention");
                }

                if (upToDate.Any())
                {
                    builder.AppendLine($"* {upToDate.Count} projects are up to date");
                }

                builder.AppendLine();

                if (errors.Any())
                {
                    builder.AppendLine("# Could not be analyzed");

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

                if (needsAttention.Any())
                {
                    builder.AppendLine("# Needs attention");

                    foreach (var project in needsAttention)
                    {
                        AppendProjectInfo(builder, project);

                        foreach (var framework in project.TargetFrameworks)
                        {
                            builder.AppendLine($"### {framework.Name}");
                            builder.AppendLine("| Package | Current | Major | Minor | Patch | Info |");
                            builder.AppendLine("| - | - | - | - | - | - | - |");

                            foreach (var package in framework.Packages)
                            {
                                var name = package.IsTransitive
                                    ? $"{package.Name} (transitive)"
                                    : package.Name;

                                builder.Append("|")
                                    .Append($" {name} |")
                                    .Append($" {package.CurrentVersion} |")
                                    .Append($" {FormatUpdate(package.Updates, UpdateSeverity.Major)} |")
                                    .Append($" {FormatUpdate(package.Updates, UpdateSeverity.Minor)} |")
                                    .Append($" {FormatUpdate(package.Updates, UpdateSeverity.Patch)} |")
                                    .Append($" {FormatInfoAndDeprecation(package)} |")
                                    .AppendLine();

                                static string FormatUpdate(Dictionary<UpdateSeverity, PackageUpdate> updates, UpdateSeverity severity)
                                {
                                    if (!updates.TryGetValue(severity, out var update))
                                        return string.Empty;

                                    return $"{update.Version} {update.Info}".Trim();
                                }

                                static string FormatInfoAndDeprecation(Package package)
                                {
                                    var result = string.Empty;

                                    if (!string.IsNullOrWhiteSpace(package.Info))
                                    {
                                        result += package.Info;
                                    }

                                    if (!string.IsNullOrWhiteSpace(package.Deprecation.Reason))
                                    {
                                        result += $" Deprecated: {package.Deprecation.Reason}";

                                        if (!string.IsNullOrWhiteSpace(package.Deprecation.Alternative))
                                        {
                                            result += $", alternative: {package.Deprecation.Alternative}";
                                        }
                                    }

                                    return result.Trim();
                                }
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