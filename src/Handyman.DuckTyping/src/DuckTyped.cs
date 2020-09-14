using System;
using System.Dynamic;
using System.Linq;

namespace Handyman.DuckTyping
{
    public static class DuckTyped
    {
        private static readonly ReadOptimizedLookup<Type, Type> InterfaceToClassLookup = new ReadOptimizedLookup<Type, Type>();

        public static T Object<T>() => Object<T>(delegate { });
        public static T Object<T>(object storage) => Object<T>(storage, delegate { });
        public static T Object<T>(Action<T> initializeDelegate) => Object(GetDefaultStorage(), initializeDelegate);

        public static T Object<T>(object storage, Action<T> initializeDelegate)
        {
            var instance = (T)CreateDuckTypedObject(typeof(T), storage);
            initializeDelegate.Invoke(instance);
            return instance;
        }

        internal static object GetDefaultStorage() => new ExpandoObject();

        internal static T CreateDuckTypedObject<T>(object storage) => (T)CreateDuckTypedObject(typeof(T), storage);

        internal static object CreateDuckTypedObject(Type type, object storage)
        {
            EnsureIsDuckTypedObject(type);

            if (type.IsInterface)
            {
                type = GetImplementationFor(type);
            }

            // todo is this fast enough or can it be optimized (ie compiled expression)?
            return Activator.CreateInstance(type, storage);
        }

        internal static bool IsDuckTypedObject(Type type)
        {
            return type.IsClass
                ? IsDuckTypedClass(type)
                : IsDuckTypedInterface(type);
        }

        private static bool IsDuckTypedClass(Type type)
        {
            do
            {
                if (type == typeof(DuckTypedObject))
                    return true;

                type = type.BaseType;
            } while (type != null);

            return false;
        }

        private static bool IsDuckTypedInterface(Type type)
        {
            // add caching for better performance
            return type.GetCustomAttributes(typeof(DuckTypedObjectContractAttribute), false).Any();
        }

        public static void EnsureIsDuckTypedObject<T>() => EnsureIsDuckTypedObject(typeof(T));

        private static void EnsureIsDuckTypedObject(Type type)
        {
            if (IsDuckTypedObject(type))
                return;

            var message = type.IsClass
                ? $"Class {type.FullName} does not inherit from {typeof(DuckTypedObject).FullName}."
                : type.IsInterface
                    ? $"Interface {type.FullName} must be decorated with {nameof(DuckTypedObjectContractAttribute)}."
                    : type.IsValueType
                        ? $"Type {type.FullName} is a value type, only classes and interfaces are supported."
                        : $"Unsupported duck typed object implementation {type.FullName}.";

            throw new InvalidOperationException(message);
        }

        private static Type GetImplementationFor(Type @interface)
        {
            if (InterfaceToClassLookup.TryGetValue(@interface, out var @class))
                return @class;

            lock (InterfaceToClassLookup)
            {
                if (!InterfaceToClassLookup.TryGetValue(@interface, out @class))
                {
                    @class = InterfaceToClassLookup[@interface] = TypeGenerator.CreateClassImplementing(new[] { @interface });
                }

                return @class;
            }
        }
    }
}
