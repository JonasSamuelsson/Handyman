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
        public IEnumerable<FrameworkConfig> Frameworks { get; set; }
        public bool? IncludeTransitive { get; set; }
        public IEnumerable<string> Tags { get; set; } = Enumerable.Empty<string>();
    }

    public class FrameworkConfig
    {
        public string Name { get; set; }
        public IEnumerable<PackageConfig> Packages { get; set; }
    }

    public class PackageConfig
    {
        public string Name { get; set; }
        public VersionLock VersionLock { get; set; }
    }

    public enum VersionLock
    {
        None,
        Major,
        Minor
    }
}