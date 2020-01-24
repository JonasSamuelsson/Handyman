using System.Collections.Generic;
using Handyman.Tools.Outdated.Analyze;
using Newtonsoft.Json;

namespace Handyman.Tools.Outdated.Model
{
    public class TargetFrameworkConfig
    {
        public string Name { get; set; }
        public IEnumerable<PackageConfig> Packages { get; set; }

        [JsonIgnore] public TargetFrameworkNameFilter Filter { get; set; }
    }
}