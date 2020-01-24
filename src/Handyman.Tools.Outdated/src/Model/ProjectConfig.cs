using System.Collections.Generic;

namespace Handyman.Tools.Outdated.Model
{
    public class ProjectConfig
    {
        public string SchemaVersion { get; set; }
        public bool Skip { get; set; }
        public bool? IncludeTransitive { get; set; }
        public IEnumerable<string> Tags { get; set; }
        public IEnumerable<TargetFrameworkConfig> TargetFrameworks { get; set; }
    }
}