using System;

namespace Handyman.Tools.Outdated.Analyze.DotnetListPackage
{
    public class TransitivePackageReader
    {
        public void Read(StringCollection collection, DotnetListPackageFramework framework)
        {
            if (!collection.Current.StartsWith("transitive package", StringComparison.InvariantCultureIgnoreCase))
                return;

            collection.MoveNext();

            while (collection.Current.StartsWith(">"))
            {
                var strings = collection.Current.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                var available = strings[3];
                var current = strings[2];
                var name = strings[1];

                framework.Dependencies.Add(new DotnetListPackageDependency
                {
                    Available = available,
                    Current = current,
                    Name = name,
                    IsTransitive = true
                });

                collection.MoveNext();
            }
        }
    }
}