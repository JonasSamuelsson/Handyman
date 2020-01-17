using System.Collections.Generic;

namespace Handyman.Tools.Outdated.Model
{
    public class Package
    {
        public string Name { get; set; }
        public string CurrentVersion { get; set; }
        public Dictionary<Severity, string> AvailableVersions { get; } = new Dictionary<Severity, string>();
        public bool IsTransitive { get; set; }
    }
}