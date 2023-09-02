using Handyman.DependencyInjection.Conventions;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Handyman.DependencyInjection
{
    public static class ScannerExtensions
    {
        public static IScanner Assembly(this IScanner scanner, Assembly assembly)
        {
            return scanner.Types(assembly.GetTypes());
        }

        public static IScanner Assemblies(this IScanner scanner, IEnumerable<Assembly> assemblies)
        {
            foreach (var assembly in assemblies)
            {
                scanner.Assembly(assembly);
            }

            return scanner;
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

        public static IScanner ConfigureClassesWithServiceAttribute(this IScanner scanner)
        {
            return scanner.ConfigureClassesWithServiceAttribute(new ServiceConfigurationOptions());
        }

        public static IScanner ConfigureClassesWithServiceAttribute(this IScanner scanner, ServiceConfigurationOptions options)
        {
            return scanner.Using(new ConfigureClassesWithServiceAttributeConvention(options));
        }

        public static IScanner ConfigureConcreteClassesOf<T>(this IScanner scanner)
        {
            return scanner.ConfigureConcreteClassesOf<T>(new ServiceConfigurationOptions());
        }

        public static IScanner ConfigureConcreteClassesOf<T>(this IScanner scanner, ServiceConfigurationOptions options)
        {
            return scanner.Using(new ConfigureConcreteClassesOfConvention(typeof(T), options));
        }

        public static IScanner ConfigureConcreteClassesOf(this IScanner scanner, Type type)
        {
            return scanner.ConfigureConcreteClassesOf(type, new ServiceConfigurationOptions());
        }

        public static IScanner ConfigureConcreteClassesOf(this IScanner scanner, Type type, ServiceConfigurationOptions options)
        {
            return scanner.Using(new ConfigureConcreteClassesOfConvention(type, options));
        }

        public static IScanner ConfigureDefaultImplementations(this IScanner scanner)
        {
            return scanner.ConfigureDefaultImplementations(new ServiceConfigurationOptions());
        }

        public static IScanner ConfigureDefaultImplementations(this IScanner scanner, ServiceConfigurationOptions options)
        {
            return scanner.Using(new ConfigureDefaultImplementationsConvention(options));
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