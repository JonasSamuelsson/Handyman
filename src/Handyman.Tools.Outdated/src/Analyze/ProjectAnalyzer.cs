using Handyman.Tools.Outdated.Analyze.DotnetListPackage;
using Handyman.Tools.Outdated.IO;
using Handyman.Tools.Outdated.Model;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public void Analyze(Project project)
        {
            var results = CheckForUpdates(project).ToList();

            foreach (var frameworks in results.SelectMany(x => x.Frameworks).GroupBy(x => x.Name))
            {
                var framework = new TargetFramework { Name = frameworks.Key };

                foreach (var packages in frameworks.SelectMany(x => x.Dependencies).GroupBy(x => x.Name))
                {
                    var package = new Package
                    {
                        CurrentVersion = packages.First().CurrentVersion,
                        IsTransitive = packages.First().IsTransitive,
                        Name = packages.Key
                    };

                    foreach (var updates in packages.GroupBy(x => GetUpdateSeverity(x.CurrentVersion, x.AvailableVersion)))
                    {
                        package.AvailableVersions[updates.Key] = updates.First().AvailableVersion;
                    }

                    framework.Packages.Add(package);
                }

                project.TargetFrameworks.Add(framework);
            }
        }

        private IEnumerable<DotnetListPackageResult> CheckForUpdates(Project project)
        {
            foreach (var severity in new[] { "", " --highest-minor", " --highest-patch" })
            {
                var transitive = project.Config.IncludeTransitive ?? false ? " --include-transitive" : "";
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

                _processRunner.Start(info).Task.Wait();

                errors.RemoveAll(string.IsNullOrWhiteSpace);
                output.RemoveAll(string.IsNullOrWhiteSpace);

                if (errors.Any())
                {
                    throw new ApplicationException(string.Join(Environment.NewLine, errors));
                }

                var parser = new DotnetListPackageResultParser();

                var result = parser.Parse(output);

                if (!result.Frameworks.Any())
                    continue;

                yield return result;
            }
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