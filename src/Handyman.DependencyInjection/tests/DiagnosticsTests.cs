using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;

namespace Handyman.DependencyInjection.Tests
{
    public class DiagnosticsTests
    {
        [Fact]
        public void ShouldGetServiceDescriptions()
        {
            var services = new ServiceCollection();

            services.AddDiagnostics();

            services.AddTransient<IFoo, Foo1>();
            services.AddScoped<IFoo, Foo2>(_ => new Foo2());
            services.AddSingleton<IFoo>(new Foo3());

            var serviceProvider = services.BuildServiceProvider();

            var diagnostics = serviceProvider.GetRequiredService<IDiagnostics>();

            var descriptions = diagnostics.GetServiceDescriptions().ToList();

            var serviceDescription = descriptions.Single(x => x.ImplementationType == typeof(Foo1));
            serviceDescription.Lifetime.ShouldBe(ServiceLifetime.Transient);
            serviceDescription.Kind.ShouldBe(ServiceKind.Type);
            serviceDescription.ServiceType.ShouldBe(typeof(IFoo));

            serviceDescription = descriptions.Single(x => x.ImplementationType == typeof(Foo2));
            serviceDescription.Lifetime.ShouldBe(ServiceLifetime.Scoped);
            serviceDescription.Kind.ShouldBe(ServiceKind.Factory);
            serviceDescription.ServiceType.ShouldBe(typeof(IFoo));

            serviceDescription = descriptions.Single(x => x.ImplementationType == typeof(Foo3));
            serviceDescription.Lifetime.ShouldBe(ServiceLifetime.Singleton);
            serviceDescription.Kind.ShouldBe(ServiceKind.Instance);
            serviceDescription.ServiceType.ShouldBe(typeof(IFoo));
        }

        private interface IFoo { }
        private class Foo1 : IFoo { }
        private class Foo2 : IFoo { }
        private class Foo3 : IFoo { }
    }
}