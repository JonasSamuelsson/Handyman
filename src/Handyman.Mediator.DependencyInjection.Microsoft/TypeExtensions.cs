using System;
using System.Collections.Generic;

namespace Handyman.Mediator.DependencyInjection.Microsoft
{
    internal static class TypeExtensions
    {
        internal static bool IsConcreteClassClosing(this Type type, Type baseTypeDefinition, out IEnumerable<Type> baseTypes)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            if (baseTypeDefinition == null)
                throw new ArgumentNullException(nameof(baseTypeDefinition));

            if (!baseTypeDefinition.IsGenericTypeDefinition)
                throw new ArgumentException();

            baseTypes = null;

            if (!type.IsConcreteClass())
                return false;

            // todo

            return false;
        }

        internal static bool IsConcreteClass(this Type type)
        {
            return type.IsClass && !type.IsAbstract && !type.IsGenericTypeDefinition;
        }
    }
}