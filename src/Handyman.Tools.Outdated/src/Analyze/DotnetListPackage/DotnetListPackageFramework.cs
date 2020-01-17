using System.Collections.Generic;

namespace Handyman.Tools.Outdated.Analyze.DotnetListPackage
{
    public class DotnetListPackageFramework
    {
        public string Name { get; set; }
        public List<DotnetListPackageDependency> Dependencies { get; } = new List<DotnetListPackageDependency>();
    }
}