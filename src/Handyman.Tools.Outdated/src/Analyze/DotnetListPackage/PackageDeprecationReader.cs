using Handyman.Tools.Outdated.Model;
using System.Linq;

namespace Handyman.Tools.Outdated.Analyze.DotnetListPackage
{
    public class PackageDeprecationReader : PackageReaderBase
    {
        protected override void Read(string[] strings, Package package)
        {
            if (strings.Length == 0)
                return;

            package.Deprecation.Reason = strings[0];
            package.Deprecation.Alternative = strings.ElementAtOrDefault(1) ?? string.Empty;
        }
    }
}