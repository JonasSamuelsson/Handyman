using System;
using System.Linq;

namespace Handyman.DependencyInjection
{
    public static class TypeExtensions
    {
        public static bool IsInNamespace(this Type type, string @namespace)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            if (@namespace == null) throw new ArgumentNullException(nameof(@namespace));

            return type.Namespace == @namespace
                   || type.Namespace?.StartsWith($"{@namespace}.", StringComparison.Ordinal) == true;
        }
    }
}