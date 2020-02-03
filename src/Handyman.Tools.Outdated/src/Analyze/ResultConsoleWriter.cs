using Handyman.Tools.Outdated.Model;
using McMaster.Extensions.CommandLineUtils;
using System.Collections.Generic;
using System.Linq;

namespace Handyman.Tools.Outdated.Analyze
{
    public class ResultConsoleWriter
    {
        private readonly IConsole _console;
        private readonly Verbosity _verbosity;

        public ResultConsoleWriter(IConsole console, Verbosity verbosity)
        {
            _console = console;
            _verbosity = verbosity;
        }

        public void WriteResult(IReadOnlyCollection<Project> projects)
        {
            if (!ShouldWrite(Verbosity.Minimal))
                return;

            _console.WriteLine();

            if (projects.Count == 0)
            {
                _console.WriteLine("No projects found");
                return;
            }

            var totalCount = projects.Count;
            var errorCount = projects.Count(x => x.Errors.Any());
            var outdatedCount = projects.Count(x => x.TargetFrameworks.Any());
            var updatedCount = totalCount - (errorCount + outdatedCount);

            _console.WriteLine($"Out of {totalCount} projects");
            if (errorCount != 0)
            {
                _console.WriteLine($"  {errorCount} could not be analyzed");
            }
            _console.WriteLine($"  {outdatedCount} have outdated dependencies");
            _console.WriteLine($"  {updatedCount} are up to date");
            _console.WriteLine();

            if (!ShouldWrite(Verbosity.Normal))
                return;

            WriteErrors(projects);
            WriteOutdated(projects);
            WriteUpToDate(projects);
        }

        private bool ShouldWrite(Verbosity required)
        {
            return _verbosity != Verbosity.Quiet && (int)required <= (int)_verbosity;
        }

        private void WriteErrors(IReadOnlyCollection<Project> projects)
        {
            projects = projects.Where(x => x.Errors.Any()).ToList();

            if (!projects.Any())
                return;

            _console.WriteLine(" Errors");
            _console.WriteLine("==============");
            _console.WriteLine();

            foreach (var project in projects)
            {
                _console.WriteLine(project.RelativePath);

                foreach (var error in project.Errors)
                {
                    _console.WriteLine($"{error.Stage}: {error.Message}");
                }

                _console.WriteLine();
            }

            _console.WriteLine();
        }

        private void WriteOutdated(IReadOnlyCollection<Project> projects)
        {
            projects = projects.Where(x => x.TargetFrameworks.Any()).ToList();

            if (!projects.Any())
                return;

            _console.WriteLine(" Outdated");
            _console.WriteLine("==============");
            _console.WriteLine();

            foreach (var project in projects)
            {
                _console.WriteLine(project.RelativePath);

                foreach (var framework in project.TargetFrameworks)
                {
                    _console.WriteLine($"  {framework.Name}");

                    foreach (var dependency in framework.Packages)
                    {
                        _console.Write($"    {dependency.Name} {dependency.CurrentVersion}");

                        if (dependency.IsTransitive)
                            _console.Write(" (T)");

                        _console.WriteLine();

                        if (dependency.AvailableVersions.TryGetValue(Severity.Major, out var v))
                            _console.WriteLine($"      Major: {v}");

                        if (dependency.AvailableVersions.TryGetValue(Severity.Minor, out v))
                            _console.WriteLine($"      Minor: {v}");

                        if (dependency.AvailableVersions.TryGetValue(Severity.Patch, out v))
                            _console.WriteLine($"      Patch: {v}");
                    }
                }

                _console.WriteLine();
            }

            _console.WriteLine();
        }

        private void WriteUpToDate(IReadOnlyCollection<Project> projects)
        {
            projects = projects.Where(x => !x.Errors.Any() || !x.TargetFrameworks.Any()).ToList();

            if (!projects.Any())
                return;

            _console.WriteLine(" Up to date");
            _console.WriteLine("==============");
            _console.WriteLine();

            foreach (var project in projects)
            {
                _console.WriteLine(project.RelativePath);
            }

            _console.WriteLine();
        }
    }
}