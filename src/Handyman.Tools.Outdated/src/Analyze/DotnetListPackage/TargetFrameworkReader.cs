using Handyman.Tools.Outdated.Model;
using System.Collections.Generic;

namespace Handyman.Tools.Outdated.Analyze.DotnetListPackage
{
    public class TargetFrameworkReader
    {
        public PackageReaderBase PackageReader { get; set; }

        public void Read(OutputEnumerator outputEnumerator, List<TargetFramework> targetFrameworks)
        {
            while (!outputEnumerator.Finished)
            {
                var current = outputEnumerator.Current;

                if ((current.StartsWith('[') && current.EndsWith("]:")) == false)
                    return;

                var frameworkName = current.Substring(1, current.Length - 3);
                var framework = targetFrameworks.GetOrAdd(x => x.Name == frameworkName, () => new TargetFramework { Name = frameworkName });

                outputEnumerator.MoveNext();

                PackageReader.Read(outputEnumerator, framework);
            }
        }
    }
}