using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Handyman.Mediator.DependencyInjection
{
    public class MediatorOptions
    {
        internal ISet<Type> TypesToScan { get; } = new HashSet<Type>();

        public ServiceLifetime MediatorLifetime { get; set; } = ServiceLifetime.Scoped;

        public void ScanAssembly(Assembly assembly)
        {
            ScanTypes(assembly.GetTypes());
        }

        public void ScanAssemblyContaining(Type type)
        {
            ScanAssembly(type.Assembly);
        }

        public void ScanAssemblyContaining<T>()
        {
            ScanAssemblyContaining(typeof(T));
        }

        public void ScanAssemblyContainingTypeOf(object o)
        {
            ScanAssemblyContaining(o.GetType());
        }

        public void ScanEntryAssembly()
        {
            ScanAssembly(Assembly.GetEntryAssembly());
        }

        public void ScanTypes(IEnumerable<Type> types)
        {
            foreach (var type in types)
            {
                TypesToScan.Add(type);
            }
        }
    }
}