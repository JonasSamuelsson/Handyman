using System;
using System.Linq;

namespace Handyman.Tools.Outdated.Analyze.DotnetListPackage
{
    public class FrameworkReader
    {
        public void Read(StringCollection collection, DotnetListPackageResult result)
        {
            while (collection.Current.StartsWith("["))
            {
                var index = collection.Current.IndexOf("]");
                var name = collection.Current.Substring(1, index - 1);
                var framework = new DotnetListPackageFramework { Name = name };

                collection.MoveNext();

                new TopLevelPackageReader().Read(collection, framework);
                new TransitivePackageReader().Read(collection, framework);

                if (framework.Dependencies.Any())
                {
                    framework.Dependencies.Sort((x, y) => string.Compare(x.Name, y.Name, StringComparison.InvariantCultureIgnoreCase));
                    result.Frameworks.Add(framework);
                }
            }
        }
    }
}