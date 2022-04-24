using Handyman.Tools.Outdated.Analyze;
using Handyman.Tools.Outdated.Model;
using Handyman.Tools.Outdated.Utils;
using McMaster.Extensions.CommandLineUtils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;
using System.Text;

namespace Handyman.Tools.Outdated.Report
{
    [Command("report")]
    public class ReportCommand
    {
        private readonly IConsole _console;
        private readonly IFileSystem _fileSystem;
        private readonly IEnumerable<IFileWriter> _fileWriters;

        public ReportCommand(IConsole console, IFileSystem fileSystem, IEnumerable<IFileWriter> fileWriters)
        {
            _console = console;
            _fileSystem = fileSystem;
            _fileWriters = fileWriters;
        }

        [Argument(0, "path", Description = "Path to json result file")]
        public string Path { get; set; }

        [Option(ShortName = "", Description = "Output file(s), supported formats are json/md")]
        public string[] OutputFile { get; set; } = { };

        [Option(ShortName = "", Description = "Tags filter, start with ! to exclude")]
        public string[] Tags { get; set; } = { };

        public void OnExecute()
        {
            var json = _fileSystem.File.ReadAllText(Path, Encoding.UTF8);

            var data = JsonConvert.DeserializeObject<JObject>(json);

            if (!HasCorrectSchema(data))
            {
                throw new Exception("Invalid input file content.");
            }

            var projects = data["Projects"]
                .ToObject<IEnumerable<Project>>()
                .ToList();

            projects = projects
                .Where(x => TagsFilter.IsMatch(x, Tags))
                .ToList();

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

        private bool HasCorrectSchema(JObject data)
        {
            if (!data.TryGetValue("schema", StringComparison.OrdinalIgnoreCase, out var schema))
                return false;

            if (JsonFileWriter.SchemaConstant != schema.Value<string>())
                return false;

            return true;
        }
    }
}