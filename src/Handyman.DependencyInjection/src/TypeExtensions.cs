using System;
using System.Linq;

namespace Handyman.DependencyInjection
{
    public static class TypeExtensions
    {
        internal static bool IsConcreteClass(this Type type)
        {
            return !type.IsAbstract && type.IsClass;
        }

        public static bool IsInNamespace(this Type type, string @namespace)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            if (@namespace == null) throw new ArgumentNullException(nameof(@namespace));

            return type.Namespace == @namespace
                   || type.Namespace?.StartsWith($"{@namespace}.", StringComparison.Ordinal) == true;
        }

        internal static string PrettyPrint(this Type type)
        {
            var name = type.FullName ?? type.Name;

            var index = name.IndexOf('`');

            if (index != -1)
                name = name.Substring(0, index);

            if (type.IsGenericType)
            {
                name += $"<{string.Join(", ", type.GetGenericArguments().Select(PrettyPrint))}>";
            }

            return name;
        }
    }
}