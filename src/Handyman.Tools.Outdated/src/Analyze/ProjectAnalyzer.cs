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
        }

        private IEnumerable<DotnetListPackageResult> CheckForUpdates(Project project)
        {
            foreach (var arg in new[] { "", " --highest-minor", " --highest-patch" })
            {
                var errors = new List<string>();
                var output = new List<string>();

                var info = new ProcessStartInfo
                {
                    Arguments = $"list {project.FullPath} package --outdated --include-transitive{arg}",
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

        internal enum Severity
        {
            Major,
            Minor,
            Patch
        }
    }
}