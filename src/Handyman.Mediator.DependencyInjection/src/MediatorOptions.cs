using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Handyman.Mediator.DependencyInjection
{
    public class MediatorOptions
    {
        internal ISet<Assembly> Assemblies { get; } = new HashSet<Assembly>();

        public ServiceLifetime MediatorLifetime { get; set; } = ServiceLifetime.Scoped;

        public void ScanAssembly(Assembly assembly)
        {
            Assemblies.Add(assembly);
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
            ScanAssembly(System.Reflection.Assembly.GetEntryAssembly());
        }
    }
}