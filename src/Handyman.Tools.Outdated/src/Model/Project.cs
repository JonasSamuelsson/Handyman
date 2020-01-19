using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace Handyman.Tools.Outdated.Model
{
    public class Project
    {
        public string FullPath { get; set; }
        public string Name { get; set; }
        public string RelativePath { get; set; }
        public IEnumerable<string> Tags { get; set; } = Enumerable.Empty<string>();
        public List<TargetFramework> TargetFrameworks { get; } = new List<TargetFramework>();

        [JsonIgnore]
        public ProjectConfig Config { get; set; }
    }

    public class ProjectConfig
    {
        public bool? IncludeTransitive { get; set; }
        public IEnumerable<string> Tags { get; set; } = Enumerable.Empty<string>();
    }
}