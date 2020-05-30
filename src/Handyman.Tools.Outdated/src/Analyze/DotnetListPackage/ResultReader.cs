using Handyman.Tools.Outdated.Model;
using System.Collections.Generic;

namespace Handyman.Tools.Outdated.Analyze.DotnetListPackage
{
    public class ResultReader
    {
        public void Read(IEnumerable<string> lines, List<TargetFramework> targetFrameworks)
        {
            var enumerator = new OutputEnumerator(lines);

            while (!enumerator.Finished)
            {
                var current = enumerator.Current;

                enumerator.MoveNext();

                var dependencyResultReader = GetDependencyResultReaderOrNull(current);

                if (dependencyResultReader != null)
                {
                    new TargetFrameworkReader { PackageReader = dependencyResultReader }
                        .Read(enumerator, targetFrameworks);
                }
            }
        }

        private static PackageReaderBase GetDependencyResultReaderOrNull(string current)
        {
            if (current.Contains("has the following updates to its packages"))
                return new PackageUpdatesReader();

            if (current.Contains("has the following deprecated packages"))
                return new PackageDeprecationReader();

            return null;
        }
    }
}