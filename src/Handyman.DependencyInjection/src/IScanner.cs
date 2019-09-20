using System;
using System.Collections.Generic;

namespace Handyman.DependencyInjection
{
    public interface IScanner
    {
        IScanner Types(IEnumerable<Type> types);
        IScanner Where(Func<Type, bool> filter);
        IScanner Using(IConvention convention);
    }
}