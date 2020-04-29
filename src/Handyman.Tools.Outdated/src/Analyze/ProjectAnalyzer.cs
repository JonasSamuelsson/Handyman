using Handyman.Tools.Outdated.Analyze.DotnetListPackage;
using Handyman.Tools.Outdated.IO;
using Handyman.Tools.Outdated.Model;
using System;
using System.Collections.Generic;
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
            var results = (await CheckForUpdates(project)).ToList();

            foreach (var frameworks in results.SelectMany(x => x.Frameworks).GroupBy(x => x.Name))
            {
                var framework = new TargetFramework { Name = frameworks.Key };
                var frameworkConfig = project.Config.TargetFrameworks.FirstOrDefault(x => x.Filter.IsMatch(framework.Name));

                foreach (var packages in frameworks.SelectMany(x => x.Dependencies).GroupBy(x => x.Name))
                {
                    var package = new Package
                    {
                        CurrentVersion = packages.First().CurrentVersion,
                        IsTransitive = packages.First().IsTransitive,
                        Name = packages.Key
                    };

                    var packageConfig = frameworkConfig?.Packages.FirstOrDefault(x => x.NameFilter.IsMatch(package.Name));

                    foreach (var updates in packages.GroupBy(x => GetUpdateSeverity(x.CurrentVersion, x.AvailableVersion)))
                    {
                        var version = updates.First().AvailableVersion;

                        if (packageConfig?.VersionFilter.IsMatch(version) == true)
                            continue;

                        package.AvailableVersions[updates.Key] = version;
                    }

                    if (!package.AvailableVersions.Any())
                        continue;

                    framework.Packages.Add(package);
                }

                if (!framework.Packages.Any())
                    continue;

                project.TargetFrameworks.Add(framework);
            }
        }

        private async Task<IEnumerable<DotnetListPackageResult>> CheckForUpdates(Project project)
        {
            var results = new List<DotnetListPackageResult>();

            foreach (var severity in new[] { "", " --highest-minor", " --highest-patch" })
            {
                var transitive = project.Config.IncludeTransitive ? " --include-transitive" : "";
                var arguments = $"list {project.FullPath} package --outdated{transitive}{severity}";

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

                var parser = new DotnetListPackageResultParser();

                var result = parser.Parse(output);

                if (!result.Frameworks.Any())
                    continue;

                results.Add(result);
            }

            return results;
        }

        private static Severity GetUpdateSeverity(string current, string available)
        {
            var x = current.Split('.');
            var y = available.Split('.');

            return x[0] != y[0]
                ? Severity.Major
                : x[1] != y[1]
                    ? Severity.Minor
                    : Severity.Patch;
        }
    }
}