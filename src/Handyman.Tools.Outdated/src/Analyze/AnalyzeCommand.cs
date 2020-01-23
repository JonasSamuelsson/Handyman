using Handyman.Tools.Outdated.Model;
using McMaster.Extensions.CommandLineUtils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Handyman.Tools.Outdated.Analyze
{
    [Command("analyze")]
    public class AnalyzeCommand
    {
        private readonly IConsole _console;
        private readonly ProjectLocator _projectLocator;
        private readonly ProjectUtil _projectUtil;
        private readonly ProjectAnalyzer _projectAnalyzer;
        private readonly IEnumerable<IFileWriter> _fileWriters;

        public AnalyzeCommand(IConsole console, ProjectLocator projectLocator, ProjectUtil projectUtil, ProjectAnalyzer projectAnalyzer, IEnumerable<IFileWriter> fileWriters)
        {
            _console = console;
            _projectLocator = projectLocator;
            _projectUtil = projectUtil;
            _projectAnalyzer = projectAnalyzer;
            _fileWriters = fileWriters;
        }

        [Argument(0, "path", Description = "Path to folder or project")]
        public string Path { get; set; }

        [Option(ShortName = "of", Description = "Output file(s), supported format are .json & .md")]
        public IEnumerable<string> OutputFile { get; set; }

        [Option(Description = "Tags filter, start with ! to exclude")]
        public IEnumerable<string> Tags { get; set; }

        [Option(CommandOptionType.NoValue)]
        public bool NoRestore { get; set; }

        [Option]
        public Verbosity Verbosity { get; set; }

        public int OnExecute()
        {
            var projects = _projectLocator.GetProjects(Path);

            if (ShouldWriteToConsole(Analyze.Verbosity.Minimal))
            {
                _console.WriteLine();
                _console.WriteLine($"Discovered {projects.Count} projects.");
                _console.WriteLine();
            }

            if (projects.Count == 0)
            {
                return 0;
            }

            foreach (var project in projects)
            {
                if (!ShouldProcessProject(project))
                    continue;

                if (ShouldWriteToConsole(Analyze.Verbosity.Minimal))
                    _console.WriteLine($"Analyzing {project.RelativePath}");

                if (NoRestore == false)
                    _projectUtil.Restore(project);

                _projectAnalyzer.Analyze(project);
            }

            if (ShouldWriteToConsole(Verbosity.Minimal))
                WriteResultToConsole(projects);

            WriteResultToFile(projects);

            return 0;
        }

        private bool ShouldWriteToConsole(Verbosity required)
        {
            var current = Verbosity;
            return current != Verbosity.Quiet && (int)required <= (int)current;
        }

        private bool ShouldProcessProject(Project project)
        {
            if (Tags == null || !Tags.Any())
                return true;

            var includes = Tags
                .Where(x => !x.StartsWith("!"))
                .Select(x => x.ToLowerInvariant())
                .ToHashSet(StringComparer.InvariantCultureIgnoreCase);

            var excludes = Tags
                .Where(x => x.StartsWith("!"))
                .Select(x => x.Substring(1))
                .Select(x => x.ToLowerInvariant())
                .ToHashSet(StringComparer.InvariantCultureIgnoreCase);

            if (includes.Any() && !project.Tags.All(x => includes.Contains(x)))
                return false;

            if (project.Tags.Any(x => excludes.Contains(x)))
                return false;

            return true;
        }

        private void WriteResultToFile(IReadOnlyCollection<Project> projects)
        {
            foreach (var outputFile in OutputFile)
            {
                var extension = System.IO.Path.GetExtension(outputFile).ToLowerInvariant();
                var fileWriters = _fileWriters.Where(x => x.CanHandle(extension)).ToList();

                if (!fileWriters.Any())
                {
                    _console.WriteLine($"Unsupported output file format '{extension}'.");
                    continue;
                }

                fileWriters.ForEach(x => x.Write(outputFile, projects));
            }
        }

        private void WriteResultToConsole(IReadOnlyCollection<Project> projects)
        {
            if (!ShouldWriteToConsole(Verbosity.Minimal))
                return;

            _console.WriteLine();

            if (projects.Count == 0)
            {
                _console.WriteLine("No projects found.");
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
            _console.WriteLine($"  {updatedCount} have outdated dependencies");
            _console.WriteLine($"  {updatedCount} are up to date");

            if (!ShouldWriteToConsole(Verbosity.Normal))
                return;

            WriteErrorsToConsole(projects);
            WriteOutdatedToConsole(projects);
            WriteUpToDateToConsole(projects);
        }

        private void WriteErrorsToConsole(IReadOnlyCollection<Project> projects)
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
            }

            _console.WriteLine();
        }

        private void WriteOutdatedToConsole(IReadOnlyCollection<Project> projects)
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

        private void WriteUpToDateToConsole(IReadOnlyCollection<Project> projects)
        {
            projects = projects.Where(x => !x.Errors.Any() && !x.TargetFrameworks.Any()).ToList();

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