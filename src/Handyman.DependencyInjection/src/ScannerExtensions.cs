using System;
using System.Reflection;
using Handyman.DependencyInjection.Conventions;
using Microsoft.Extensions.DependencyInjection;

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

        public static IScanner RegisterConcreteClassesOf<T>(this IScanner scanner)
        {
            return scanner.RegisterConcreteClassesOf<T>(null);
        }

        public static IScanner RegisterConcreteClassesOf<T>(this IScanner scanner, ServiceLifetime? serviceLifetime)
        {
            return scanner.Use(new ConcreteClassesOfConvention(typeof(T), serviceLifetime));
        }

        public static IScanner RegisterConcreteClassesOf(this IScanner scanner, Type type)
        {
            return scanner.RegisterConcreteClassesOf(type, null);
        }

        public static IScanner RegisterConcreteClassesOf(this IScanner scanner, Type type, ServiceLifetime? serviceLifetime)
        {
            return scanner.Use(new ConcreteClassesOfConvention(type, serviceLifetime));
        }

        public static IScanner RegisterDefaultImplementations(this IScanner scanner)
        {
            return scanner.RegisterDefaultImplementations(null);
        }

        public static IScanner RegisterDefaultImplementations(this IScanner scanner, ServiceLifetime? serviceLifetime)
        {
            return scanner.Use(new DefaultImplementationsConvention(serviceLifetime));
        }

        public static IScanner UseRegistrationPolicies(this IScanner scanner)
        {
            return scanner.Use(new RegistrationPolicyConvention());
        }

        public static IScanner Use<TConvention>(this IScanner scanner) where TConvention : IConvention, new()
        {
            return scanner.Use(new TConvention());
        }
    }
}