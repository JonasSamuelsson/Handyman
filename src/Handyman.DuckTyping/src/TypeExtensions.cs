using System;
using System.Linq;

namespace Handyman.DuckTyping
{
    internal static class TypeExtensions
    {
        internal static string GetFullName(this Type type)
        {
            return $"{type.Namespace}.{GetName(type)}";
        }

        private static string GetName(Type type)
        {
            var name = string.Empty;

            for (var declaringType = type.DeclaringType; declaringType != null; declaringType = declaringType.DeclaringType)
            {
                name = $"{GetName(declaringType)}.{name}";
            }

            name += SanitizeName(type.Name);

            if (type.IsGenericType)
            {
                name += $"<{string.Join(", ", type.GetGenericArguments().Select(x => x.GetFullName()))}>";
            }

            return name;
        }

        private static string SanitizeName(string name)
        {
            var index = name.IndexOf('`');
            return index == -1 ? name : name.Substring(0, index);
        }
    }
}