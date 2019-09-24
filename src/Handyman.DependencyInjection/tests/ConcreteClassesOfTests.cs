using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Handyman.DependencyInjection.Tests
{
    public class ConcreteClassesOfTests
    {
        [Theory, MemberData(nameof(GetParams))]
        public void ShouldRegisterConcreteClassesOf(Type type, IReadOnlyCollection<Expectation> expectations)
        {
            var serviceCollection = new ServiceCollection();
            var types = GetType().GetNestedTypes(BindingFlags.NonPublic);

            serviceCollection.Scan(scanner => scanner.Types(types).ConfigureConcreteClassesOf(type));

            var errors = new List<(string error, Type serviceType, Type implementationType)>();

            foreach (var expectation in expectations)
            {
                var implementationType = expectation.ImplementationType;
                var serviceType = expectation.ServiceType;

                if (!serviceCollection.Any(x => x.ImplementationType == implementationType && x.ServiceType == serviceType))
                {
                    errors.Add((error: "missing registrations", serviceType: serviceType, implementationType: implementationType));
                }
            }

            foreach (var serviceDescriptor in serviceCollection)
            {
                var implementationType = serviceDescriptor.ImplementationType;
                var serviceType = serviceDescriptor.ServiceType;

                if (!expectations.Any(x => x.ImplementationType == implementationType && x.ServiceType == serviceType))
                {
                    errors.Add((error: "unexpected registrations", serviceType: serviceType, implementationType: implementationType));
                }
            }

            if (!errors.Any())
                return;

            var builder = new StringBuilder();
            builder.AppendLine($"{type.PrettyPrint()}");
            builder.AppendLine();
            builder.AppendLine($"{errors.Count} errors");
            builder.AppendLine();
            foreach (var g in errors.GroupBy(x => x.error))
            {
                builder.AppendLine(g.Key);
                foreach (var error in g)
                {
                    builder.AppendLine($"- service type: {error.serviceType.PrettyPrint()}");
                    builder.AppendLine($"  implementation type: {error.implementationType.PrettyPrint()}");
                }
                builder.AppendLine();
            }
            throw new Exception(builder.ToString());
        }

        public static IEnumerable<object[]> GetParams()
        {
            yield return new object[]
            {
                typeof(IFoo),
                new[]
                {
                    new Expectation {ServiceType = typeof(IFoo), ImplementationType = typeof(Foo)},
                    new Expectation {ServiceType = typeof(IFoo), ImplementationType = typeof(FooString)}
                }
            };

            yield return new object[]
            {
                typeof(IFoo<>),
                new[]
                {
                    new Expectation {ServiceType = typeof(IFoo<>), ImplementationType = typeof(Foo<>)},
                    new Expectation {ServiceType = typeof(IFoo<>), ImplementationType = typeof(Foobar<>)},
                    new Expectation {ServiceType = typeof(IFoo<string>), ImplementationType = typeof(FooString)}
                }
            };

            yield return new object[]
            {
                typeof(IFoo<int>),
                new Expectation[] { }
            };

            yield return new object[]
            {
                typeof(IFoo<string>),
                new[] {new Expectation {ServiceType = typeof(IFoo<string>), ImplementationType = typeof(FooString)}}
            };

            yield return new object[]
            {
                typeof(Foo),
                new[] {new Expectation {ServiceType = typeof(Foo), ImplementationType = typeof(FooString)}}
            };

            yield return new object[]
            {
                typeof(Foo<>),
                new[]
                {
                    new Expectation {ServiceType = typeof(Foo<>), ImplementationType = typeof(Foobar<>)},
                    new Expectation {ServiceType = typeof(Foo<string>), ImplementationType = typeof(FooString)}
                }
            };

            yield return new object[]
            {
                typeof(Foo<int>),
                new Expectation[] { }
            };

            yield return new object[]
            {
                typeof(Foo<string>),
                new[] {new Expectation {ServiceType = typeof(Foo<string>), ImplementationType = typeof(FooString)}}
            };
        }

        private interface IFoo { }
        private interface IFoo<T> : IFoo { }

        private class Foo : IFoo { }
        private class Foo<T> : Foo, IFoo<T> { }
        private class Foobar<T> : Foo<T> { }
        private class FooString : Foo<string> { }

        private abstract class Bar<T> : Foo<T> { }
        private class Bazz<T> : Foo<string> { }

        public class Expectation
        {
            public Type ServiceType { get; set; }
            public Type ImplementationType { get; set; }
        }

        // IFoo => Foo, FooString
        // IFoo<T> => Foo<T>, Foobar<T>, IFoo<string>:FooString
        // IFoo<int> => !
        // IFoo<string> => FooString
        // Foo => FooString
        // Foo<T> => Foobar<T>, Foo<string>:FooString
        // Foo<int> => !
        // Foo<string> => FooString
    }
}