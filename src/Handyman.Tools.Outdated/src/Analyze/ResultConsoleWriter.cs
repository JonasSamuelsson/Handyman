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
            var errors = projects.Where(x => x.Errors.Any()).ToList();
            var needsAttention = projects.Where(x => x.TargetFrameworks.Any()).ToList();
            var upToDate = projects.Except(errors).Except(needsAttention).ToList();

            WriteSummary(totalCount, errors, needsAttention, upToDate);

            if (!ShouldWrite(Verbosity.Normal))
                return;

            WriteErrors(errors);
            WriteNeedsAttention(needsAttention);
            WriteUpToDate(upToDate);
        }

        private bool ShouldWrite(Verbosity required)
        {
            return _verbosity != Verbosity.Quiet && (int)required <= (int)_verbosity;
        }

        private void WriteSummary(int totalCount, List<Project> errors, List<Project> needsAttention, List<Project> upToDate)
        {
            _console.WriteLine($"Out of {totalCount} projects");

            if (errors.Any())
            {
                _console.WriteLine($"  {errors.Count} could not be analyzed");
            }

            if (needsAttention.Any())
            {
                _console.WriteLine($"  {needsAttention.Count} needs attention");
            }

            if (upToDate.Any())
            {
                _console.WriteLine($"  {upToDate.Count} are up to date");
            }

            _console.WriteLine();
        }

        private void WriteErrors(IReadOnlyCollection<Project> projects)
        {
            if (!projects.Any())
                return;

            _console.WriteLine(" Could not be analyzed");
            _console.WriteLine("=========================");
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

        private void WriteNeedsAttention(IReadOnlyCollection<Project> projects)
        {
            if (!projects.Any())
                return;

            _console.WriteLine(" Needs attention");
            _console.WriteLine("===================");
            _console.WriteLine();

            foreach (var project in projects)
            {
                _console.WriteLine(project.RelativePath);

                foreach (var framework in project.TargetFrameworks)
                {
                    _console.WriteLine($"  {framework.Name}");

                    foreach (var package in framework.Packages)
                    {
                        _console.Write($"    {package.Name} {package.CurrentVersion}");

                        if (package.IsTransitive)
                            _console.Write(" (transitive)");

                        _console.WriteLine();

                        if (!string.IsNullOrWhiteSpace(package.Info))
                            _console.WriteLine($"      Info: {package.Info}");

                        if (!string.IsNullOrWhiteSpace(package.Deprecation.Reason))
                        {
                            _console.Write($"      Deprecated: {(package.Deprecation.Reason)}");

                            if (!string.IsNullOrWhiteSpace(package.Deprecation.Alternative))
                                _console.Write($", alternative: {package.Deprecation.Alternative}");

                            _console.WriteLine();
                        }

                        if (package.Updates.TryGetValue(UpdateSeverity.Major, out var x))
                            _console.WriteLine($"      Major update: {FormatUpdate(x)}");

                        if (package.Updates.TryGetValue(UpdateSeverity.Minor, out x))
                            _console.WriteLine($"      Minor update: {FormatUpdate(x)}");

                        if (package.Updates.TryGetValue(UpdateSeverity.Patch, out x))
                            _console.WriteLine($"      Patch update: {FormatUpdate(x)}");

                        static string FormatUpdate(PackageUpdate update) => $"{update.Version} {update.Info}".Trim();
                    }
                }

                _console.WriteLine();
            }

            _console.WriteLine();
        }

        private void WriteUpToDate(IReadOnlyCollection<Project> projects)
        {
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