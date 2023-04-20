using CliWrap;
using Handyman.Tools.Outdated.Analyze.DotnetListPackage;
using Handyman.Tools.Outdated.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Handyman.Tools.Outdated.Analyze
{
    public class ProjectAnalyzer
    {
        public async Task Analyze(Project project)
        {
            await InvokeExternalAnalyzer(project);
            ApplyConfiguration(project);
        }

        private async Task InvokeExternalAnalyzer(Project project)
        {
            var includeTransitive = project.Config.IncludeTransitive ? "--include-transitive" : "";

            var argumentsSuffixCollections = new[]
            {
                new[] { "--outdated", includeTransitive },
                new[] { "--outdated", "--highest-minor", includeTransitive },
                new[] { "--outdated", "--highest-patch", includeTransitive },
                new[] { "--deprecated", includeTransitive }
            };

            foreach (var argumentsSuffixCollection in argumentsSuffixCollections)
            {
                var arguments = new[] { "list", project.FullPath, "package" }.Concat(argumentsSuffixCollection.Where(x => x.Length != 0));

                var errors = new List<string>();
                var output = new List<string>();

                await Cli.Wrap("dotnet")
                    .WithArguments(arguments)
                    .WithStandardErrorPipe(PipeTarget.ToDelegate(s => errors.Add(s)))
                    .WithStandardOutputPipe(PipeTarget.ToDelegate(s => output.Add(s)))
                    .ExecuteAsync();

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