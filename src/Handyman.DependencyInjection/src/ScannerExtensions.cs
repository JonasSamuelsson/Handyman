using Handyman.DependencyInjection.Conventions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Reflection;

namespace Handyman.DependencyInjection
{
    public static class ScannerExtensions
    {
        public static IScanner Assembly(this IScanner scanner, Assembly assembly)
        {
            return scanner.Types(assembly.GetTypes());
        }

        public static IScanner AssemblyContaining(this IScanner scanner, Type type)
        {
            return scanner.Assembly(type.Assembly);
        }

        public static IScanner AssemblyContaining<T>(this IScanner scanner)
        {
            return scanner.AssemblyContaining(typeof(T));
        }

        public static IScanner AssemblyContainingTypeOf(this IScanner scanner, object o)
        {
            return scanner.AssemblyContaining(o.GetType());
        }

        public static IScanner EntryAssembly(this IScanner scanner)
        {
            return scanner.Assembly(System.Reflection.Assembly.GetEntryAssembly());
        }

        public static IScanner ConfigureConcreteClassesOf<T>(this IScanner scanner)
        {
            return scanner.ConfigureConcreteClassesOf<T>(null);
        }

        public static IScanner ConfigureConcreteClassesOf<T>(this IScanner scanner, ServiceLifetime? serviceLifetime)
        {
            return scanner.Using(new ConfigureConcreteClassesOfConvention(typeof(T), serviceLifetime));
        }

        public static IScanner ConfigureConcreteClassesOf(this IScanner scanner, Type type)
        {
            return scanner.ConfigureConcreteClassesOf(type, null);
        }

        public static IScanner ConfigureConcreteClassesOf(this IScanner scanner, Type type, ServiceLifetime? serviceLifetime)
        {
            return scanner.Using(new ConfigureConcreteClassesOfConvention(type, serviceLifetime));
        }

        public static IScanner ConfigureDefaultImplementations(this IScanner scanner)
        {
            return scanner.ConfigureDefaultImplementations(null);
        }

        public static IScanner ConfigureDefaultImplementations(this IScanner scanner, ServiceLifetime? serviceLifetime)
        {
            return scanner.Using(new ConfigureDefaultImplementationsConvention(serviceLifetime));
        }

        public static IScanner ExecuteServiceConfigurationStrategies(this IScanner scanner)
        {
            return scanner.Using(new ExecuteServiceConfigurationStrategiesConvention());
        }

        public static IScanner Using<TConvention>(this IScanner scanner) where TConvention : IServiceConfigurationConvention, new()
        {
            return scanner.Using(new TConvention());
        }
    }
}