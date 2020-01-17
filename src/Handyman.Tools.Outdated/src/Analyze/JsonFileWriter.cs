using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Text;
using Handyman.Tools.Outdated.Model;
using Newtonsoft.Json;

namespace Handyman.Tools.Outdated.Analyze
{
    public class JsonFileWriter : IFileWriter
    {
        private readonly IFileSystem _fileSystem;

        public JsonFileWriter(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }

        public bool CanHandle(string extension)
        {
            return extension == ".json";
        }

        public void Write(string path, IEnumerable<Project> projects)
        {
            var o = new
            {
                Projects = projects,
                Timestamp = DateTimeOffset.UtcNow.ToString("yyyy-MM-dd HH:mm UTC")
            };
            var json = JsonConvert.SerializeObject(o, Formatting.Indented);

            _fileSystem.File.WriteAllText(path, json, Encoding.UTF8);
        }
    }
}