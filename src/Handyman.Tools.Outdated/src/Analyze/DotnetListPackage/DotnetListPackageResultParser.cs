using System;
using System.Collections.Generic;

namespace Handyman.Tools.Outdated.Analyze.DotnetListPackage
{
    public class DotnetListPackageResultParser
    {
        public DotnetListPackageResult Parse(IEnumerable<string> lines)
        {
            var stringCollection = new StringCollection(lines);
            var reader = new FrameworkReader();
            var result = new DotnetListPackageResult();

            while (!stringCollection.Finished)
            {
                reader.Read(stringCollection, result);
                stringCollection.MoveNext();
            }

            result.Frameworks.Sort((x,y) => string.Compare(x.Name, y.Name, StringComparison.InvariantCultureIgnoreCase));

            return result;
        }
    }
}