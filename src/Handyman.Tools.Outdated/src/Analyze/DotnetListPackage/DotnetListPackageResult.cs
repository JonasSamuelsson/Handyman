using System.Collections.Generic;

namespace Handyman.Tools.Outdated.Analyze.DotnetListPackage
{
    public class DotnetListPackageResult
    {
        public List<DotnetListPackageFramework> Frameworks { get; } = new List<DotnetListPackageFramework>();
    }
}