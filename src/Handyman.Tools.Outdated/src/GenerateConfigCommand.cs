using Handyman.Tools.Outdated.Model;
using McMaster.Extensions.CommandLineUtils;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.IO.Abstractions;
using System.Text;

namespace Handyman.Tools.Outdated
{
    [Command("generate-config")]
    public class GenerateConfigCommand
    {
        private readonly IFileSystem _fileSystem;

        public GenerateConfigCommand(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }

        [Argument(0, "Output file")]
        public string OutputFile { get; set; }

        public void OnExecute()
        {
            var config = new ProjectConfig
            {
                Tags = new[] { "alpha", "bravo" },
                Frameworks = new[]
                {
                    new FrameworkConfig
                    {
                        Name = "netcoreapp3.*",
                        Packages = new[]
                        {
                            new PackageConfig
                            {
                                Name = "Handyman.*",
                                VersionLock = VersionLock.Major
                            }
                        }
                    }
                }
            };

            var path = OutputFile ?? ".handyman-outdated.json";
            var formatting = Formatting.Indented;
            var settings = new JsonSerializerSettings { Converters = { new StringEnumConverter() } };
            var json = JsonConvert.SerializeObject(config, formatting, settings);

            _fileSystem.File.WriteAllText(path, json, Encoding.UTF8);
        }
    }
}