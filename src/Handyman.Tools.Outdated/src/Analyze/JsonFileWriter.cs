using Handyman.Tools.Outdated.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Text;

namespace Handyman.Tools.Outdated.Analyze
{
    public class JsonFileWriter : FileWriter
    {
        public JsonFileWriter(IFileSystem fileSystem) : base(fileSystem)
        {
        }

        public override bool CanHandle(string extension)
        {
            return string.Equals(extension, ".json", StringComparison.OrdinalIgnoreCase);
        }

        public override void Write(string path, IEnumerable<Project> projects)
        {
            var data = new
            {
                Schema = "handyman-outdated/1.0",
                Projects = projects
            };

            var json = JsonConvert.SerializeObject(data);

            WriteToDisk(path, json, Encoding.UTF8);
        }
    }
}