using McMaster.Extensions.CommandLineUtils;

namespace Handyman.Tools.Outdated.Analyze
{
    [Command("analyze")]
    public class AnalyzeCommand
    {
        private readonly IConsole _console;
        private readonly ProjectResolver _projectResolver;
        private readonly ProjectAnalyzer _projectAnalyzer;

        public AnalyzeCommand(IConsole console, ProjectResolver projectResolver, ProjectAnalyzer projectAnalyzer)
        {
            _console = console;
            _projectResolver = projectResolver;
            _projectAnalyzer = projectAnalyzer;
        }

        [Argument(0, "path", Description = "Path to folder or project.")]
        public string Path { get; set; }

        public int OnExecute()
        {
            var projects = _projectResolver.GetProjects(Path);

            _console.WriteLine($"Found {projects.Count} projects.");

            if (projects.Count == 0)
            {
                return 0;
            }

            foreach (var project in projects)
            {
                _console.WriteLine($"Analyzing {project.RelativePath}...");
                _projectAnalyzer.Analyze(project);
            }

            return 0;
        }
    }
}