using Handyman.Tools.Outdated.Model;
using McMaster.Extensions.CommandLineUtils;
using System.Collections.Generic;
using System.Linq;

namespace Handyman.Tools.Outdated.Analyze
{
    [Command("analyze")]
    public class AnalyzeCommand
    {
        private readonly IConsole _console;
        private readonly ProjectLocator _projectLocator;
        private readonly ProjectAnalyzer _projectAnalyzer;
        private readonly IEnumerable<IFileWriter> _fileWriters;

        public AnalyzeCommand(IConsole console, ProjectLocator projectLocator, ProjectAnalyzer projectAnalyzer, IEnumerable<IFileWriter> fileWriters)
        {
            _console = console;
            _projectLocator = projectLocator;
            _projectAnalyzer = projectAnalyzer;
            _fileWriters = fileWriters;
        }

        [Argument(0, "path", Description = "Path to folder or project")]
        public string Path { get; set; }

        [Option(ShortName = "of", Description = "Output file(s)")]
        public IEnumerable<string> OutputFile { get; set; }

        public int OnExecute()
        {
            var projects = _projectLocator.GetProjects(Path);

            _console.WriteLine();
            _console.WriteLine($"Discovered {projects.Count} projects.");
            _console.WriteLine();

            if (projects.Count == 0)
            {
                return 0;
            }

            foreach (var project in projects)
            {
                _console.WriteLine($"Analyzing {project.RelativePath}");
                _projectAnalyzer.Analyze(project);
            }

            _console.WriteLine();

            projects = projects.Where(x => x.TargetFrameworks.Any()).ToList();

            if (!projects.Any())
            {
                _console.WriteLine("All projects are up to date. =)");
            }

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

            return 0;
        }
    }
}