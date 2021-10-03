using System;
using System.Linq;

namespace Handyman.DataContractValidator.Utils
{
    internal static class TypeExtensions
    {
        public static bool TryGetInterfaceClosing(this Type type, Type genericTypeDefinition, out Type @interface)
        {
            @interface = null;

            if (!genericTypeDefinition.IsGenericTypeDefinition)
            {
                throw new InvalidOperationException($"{type} is not a generic type definition.");
            }

            if (type.IsGenericType && type.GetGenericTypeDefinition() == genericTypeDefinition)
            {
                @interface = type;
                return true;
            }

            var interfaces = type.GetInterfaces()
                .Where(x => x.IsGenericType && x.GetGenericTypeDefinition() == genericTypeDefinition)
                .ToList();

            if (interfaces.Count == 0)
            {
                return false;
            }

            if (interfaces.Count > 1)
            {
                throw new InvalidOperationException($"{type} has multiple implementations of {genericTypeDefinition}");
            }

            @interface = interfaces.Single();
            return true;
        }
    }
}