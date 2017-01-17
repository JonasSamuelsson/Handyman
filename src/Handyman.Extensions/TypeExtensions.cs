using System;
using System.Collections.Generic;
using System.Linq;

namespace Handyman.Extensions
{
    public static class TypeExtensions
    {
        public static bool IsConcreteClass(this Type type)
        {
            return type.IsClass && !type.IsAbstract;
        }

        public static bool IsConcreteClosedClass(this Type type)
        {
            return type.IsConcreteClass() && !type.IsGenericTypeDefinition;
        }

        public static bool IsConcreteClassClosing(this Type type, Type genericTypeDefinition)
        {
            IReadOnlyCollection<Type> types;
            return type.IsConcreteClassClosing(genericTypeDefinition, out types);
        }

        public static bool IsConcreteClassClosing(this Type type, Type genericTypeDefinition, out IReadOnlyCollection<Type> genericTypes)
        {
            genericTypes = null;

            if (!genericTypeDefinition.IsGenericTypeDefinition)
                throw new ArgumentException();

            if (!type.IsConcreteClosedClass())
                return false;

            var supertypes = from supertype in type.GetSuperTypes()
                             where genericTypeDefinition.IsClass
                                       ? supertype.IsClass
                                       : supertype.IsInterface
                             select supertype;

            genericTypes = supertypes
                .Where(x => x.IsGenericType)
                .Where(x => !x.IsGenericTypeDefinition)
                .Where(x => x.GetGenericTypeDefinition() == genericTypeDefinition)
                .ToList();

            if (genericTypes.Count == 0)
            {
                genericTypes = null;
                return false;
            }

            return true;
        }

        public static bool IsConcreteSubClassOf(this Type subClassCandidate, Type type)
        {
            return subClassCandidate.IsConcreteClass() && subClassCandidate.IsSubTypeOf(type);
        }

        public static bool IsConcreteSubClassOf<T>(this Type subClassCandidate)
        {
            return subClassCandidate.IsConcreteSubClassOf(typeof(T));
        }

        public static bool IsOfType(this Type sameOrSubTypeCandidate, Type type)
        {
            return sameOrSubTypeCandidate == type || sameOrSubTypeCandidate.IsSubTypeOf(type);
        }

        public static bool IsOfType<T>(this Type sameOrSubTypeCandidate)
        {
            return sameOrSubTypeCandidate.IsOfType(typeof(T));
        }

        public static bool IsSubTypeOf(this Type subTypeCandidate, Type type)
        {
            if (subTypeCandidate == type)
                return false;

            if (type == typeof(object))
                return true;

            if (!subTypeCandidate.IsGenericTypeDefinition && !type.IsGenericTypeDefinition)
                return type.IsAssignableFrom(subTypeCandidate);

            return subTypeCandidate.GetSuperTypes()
                .Where(x => x != subTypeCandidate)
                .Contains(type);
        }

        public static bool IsSubTypeOf<T>(this Type subTypeCandidate)
        {
            return subTypeCandidate.IsSubTypeOf(typeof(T));
        }

        public static bool IsSuperTypeOf(this Type superTypeCandidate, Type type)
        {
            return type.IsSubTypeOf(superTypeCandidate);
        }

        public static bool IsSuperTypeOf<T>(this Type superTypeCandidate)
        {
            return superTypeCandidate.IsSuperTypeOf(typeof(T));
        }

        public static IEnumerable<Type> GetSuperTypes(this Type type)
        {
            var list = new List<Type>();
            GetAllSuperTypes(type, list);
            return list.Where(x => x != type).Where(x => x.FullName != null);
        }

        private static void GetAllSuperTypes(Type type, ICollection<Type> result)
        {
            for (var t = type; t != null; t = t.BaseType)
            {
                if (result.Contains(t)) continue;
                result.Add(t);
                t.GetInterfaces().ForEach(x => GetAllSuperTypes(x, result));
                if (t.IsGenericType && !t.IsGenericTypeDefinition) GetAllSuperTypes(t.GetGenericTypeDefinition(), result);
            }
        }
    }
}