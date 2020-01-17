using System;

namespace Handyman.Tools.Outdated.Analyze.DotnetListPackage
{
    public class TopLevelPackageReader
    {
        public void Read(StringCollection collection, DotnetListPackageFramework framework)
        {
            if (!collection.Current.StartsWith("top-level package", StringComparison.InvariantCultureIgnoreCase))
                return;

            collection.MoveNext();

            while (collection.Current.StartsWith(">"))
            {
                var strings = collection.Current.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                var available = strings[4];
                var current = strings[3];
                var name = strings[1];

                framework.Dependencies.Add(new DotnetListPackageDependency
                {
                    AvailableVersion = available,
                    CurrentVersion = current,
                    Name = name
                });

                collection.MoveNext();
            }
        }
    }
}