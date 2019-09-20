using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;

namespace Handyman.DependencyInjection.Tests
{
    public class DefaultImplementationsTests
    {
        [Fact]
        public void ShouldRegisterDefaultImplementations()
        {
            var serviceCollection = new ServiceCollection();

            serviceCollection.Scan(scanner => scanner.Types(GetType().GetNestedTypes(BindingFlags.NonPublic)).RegisterDefaultImplementations());

            serviceCollection.BuildServiceProvider().GetRequiredService<IFoo>().ShouldBeOfType<Foo>();
        }

        private interface IFoo { }

        private class Foo : IFoo { }
    }
}
