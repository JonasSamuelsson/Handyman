using System.Collections.Generic;

namespace Handyman.Tools.Outdated.Model
{
    public class TargetFrameworkConfig
    {
        public string Name { get; set; }
        public IEnumerable<PackageConfig> Packages { get; set; }
    }
}