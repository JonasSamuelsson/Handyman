using Handyman.Tools.Outdated.Analyze.DotnetListPackage;
using Handyman.Tools.Outdated.IO;
using Handyman.Tools.Outdated.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using ProcessStartInfo = Handyman.Tools.Outdated.IO.ProcessStartInfo;

namespace Handyman.Tools.Outdated.Analyze
{
    public class ProjectAnalyzer
    {
        private readonly IProcessRunner _processRunner;

        public ProjectAnalyzer(IProcessRunner processRunner)
        {
            _processRunner = processRunner;
        }

        public async Task Analyze(Project project)
        {
            await InvokeExternalAnalyzer(project);
            ApplyConfiguration(project);
        }

        private async Task InvokeExternalAnalyzer(Project project)
        {
            var includeTransitive = project.Config.IncludeTransitive ? "--include-transitive" : "";

            var argumentsSuffixes = new[]
            {
                $"--outdated {includeTransitive}",
                $"--outdated --highest-minor {includeTransitive}",
                $"--outdated --highest-patch {includeTransitive}",
                $"--deprecated {includeTransitive}"
            };

            foreach (var argumentsSuffix in argumentsSuffixes)
            {
                var arguments = $"list {project.FullPath} package {argumentsSuffix}";

                var errors = new List<string>();
                var output = new List<string>();

                var info = new ProcessStartInfo
                {
                    Arguments = arguments,
                    FileName = "dotnet",
                    StandardErrorHandler = s => errors.Add(s),
                    StandardOutputHandler = s => output.Add(s)
                };

                await _processRunner.Start(info).Task;

                errors.RemoveAll(string.IsNullOrWhiteSpace);
                output.RemoveAll(string.IsNullOrWhiteSpace);

                if (errors.Any())
                {
                    project.Errors.Add(new Error
                    {
                        Message = string.Join(Environment.NewLine, errors),
                        Stage = "analyze"
                    });

                    break;
                }

                new ResultReader().Read(output, project.TargetFrameworks);
            }
        }

        private static void ApplyConfiguration(Project project)
        {
            foreach (var framework in project.TargetFrameworks.ToList())
            {
                var frameworkConfig = project.Config.TargetFrameworks.FirstOrDefault(x => x.Filter.IsMatch(framework.Name));

                foreach (var package in framework.Packages.ToList())
                {
                    if (project.Config.IncludeTransitive == false && package.IsTransitive)
                    {
                        framework.Packages.Remove(package);
                        continue;
                    }

                    var packageConfig = frameworkConfig?.Packages.FirstOrDefault(x => x.NameFilter.IsMatch(package.Name));

                    foreach (var (severity, update) in package.Updates.ToList())
                    {
                        if (packageConfig?.VersionFilter.IsMatch(update.Version) != true)
                            continue;

                        package.Updates.Remove(severity);
                    }

                    if (package.NeedsAttention == false)
                        framework.Packages.Remove(package);
                }

                if (framework.Packages.Any() == false)
                    project.TargetFrameworks.Remove(framework);
            }
        }
    }
}