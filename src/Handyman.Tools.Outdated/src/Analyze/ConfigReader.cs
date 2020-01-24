using System;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
using Handyman.Tools.Outdated.Model;
using Newtonsoft.Json;

namespace Handyman.Tools.Outdated.Analyze
{
    public class ConfigReader
    {
        private readonly IFileSystem _fileSystem;

        public ConfigReader(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }

        public ProjectConfig GetConfig(string directory)
        {
            do
            {
                var files = _fileSystem.Directory.GetFiles(directory, ".handyman-outdatad.json",
                    SearchOption.TopDirectoryOnly);

                if (!files.Any())
                {
                    directory = _fileSystem.Path.GetDirectoryName(directory);
                    continue;
                }

                return Parse(_fileSystem.File.ReadAllText(files.Single(), Encoding.UTF8));
            } while (directory != null);

            return new ProjectConfig();
        }

        private static ProjectConfig Parse(string json)
        {
            var config = JsonConvert.DeserializeObject<ProjectConfig>(json);

            if (config.SchemaVersion != "1.0")
                throw new ApplicationException($"Unsupported config schema version '{config.SchemaVersion}'.");

            config.Tags ??= Enumerable.Empty<string>();
            config.TargetFrameworks ??= Enumerable.Empty<TargetFrameworkConfig>();

            return config;
        }
    }
}