using System.Collections.Generic;

namespace Handyman.Tools.Outdated.Model
{
    public class Project
    {
        public string FullPath { get; set; }
        public string RelativePath { get; set; }
        public List<TargetFramework> TargetFrameworks { get; } = new List<TargetFramework>();
    }

    public class TargetFramework
    {
        public string Name { get; set; }
        public List<Dependency> Dependencies { get; } = new List<Dependency>();
    }

    public class Dependency
    {
        public string Name { get; set; }
        public string CurrentVersion { get; set; }
        public string MajorVersionUpdate { get; set; }
        public string MinorVersionUpdate { get; set; }
        public string PatchVersionUpdate { get; set; }
        public bool IsTransitive { get; set; }
    }
}