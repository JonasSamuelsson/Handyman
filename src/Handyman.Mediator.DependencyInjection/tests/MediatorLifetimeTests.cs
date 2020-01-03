using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System.Linq;
using Xunit;

namespace Handyman.Mediator.DependencyInjection.Tests
{
    public class MediatorLifetimeTests
    {
        [Fact]
        public void ShouldDefaultToScopedLifetime()
        {
            new ServiceCollection()
                .AddMediator(delegate { })
                .Single(x => x.ServiceType == typeof(IMediator))
                .Lifetime
                .ShouldBe(ServiceLifetime.Scoped);
        }

        [Fact]
        public void ShouldSupportTransientLifetime()
        {
            var services = new ServiceCollection();

            services.AddMediator(options => options.MediatorLifetime = ServiceLifetime.Transient);

            var serviceProvider = services.BuildServiceProvider();

            var mediator1 = serviceProvider.GetRequiredService<IMediator>();
            var mediator2 = serviceProvider.GetRequiredService<IMediator>();

            mediator1.ShouldNotBe(mediator2);
        }

        [Fact]
        public void ShouldSupportScopedLifetime()
        {
            var services = new ServiceCollection();

            services.AddMediator(options => options.MediatorLifetime = ServiceLifetime.Scoped);

            var rootServiceProvider = services.BuildServiceProvider();
            var scopedServiceProvider = rootServiceProvider.CreateScope().ServiceProvider;

            var rootMediator1 = rootServiceProvider.GetRequiredService<IMediator>();
            var rootMediator2 = rootServiceProvider.GetRequiredService<IMediator>();
            var scopedMediator1 = scopedServiceProvider.GetRequiredService<IMediator>();
            var scopedMediator2 = scopedServiceProvider.GetRequiredService<IMediator>();

            rootMediator1.ShouldBe(rootMediator2);
            rootMediator1.ShouldNotBe(scopedMediator1);
            scopedMediator1.ShouldBe(scopedMediator2);
        }

        [Fact]
        public void ShouldSupportSingletonLifetime()
        {
            var services = new ServiceCollection();

            services.AddMediator(options => options.MediatorLifetime = ServiceLifetime.Singleton);

            var rootServiceProvider = services.BuildServiceProvider();
            var scopedServiceProvider = rootServiceProvider.CreateScope().ServiceProvider;

            var rootMediator1 = rootServiceProvider.GetRequiredService<IMediator>();
            var rootMediator2 = rootServiceProvider.GetRequiredService<IMediator>();
            var scopedMediator1 = scopedServiceProvider.GetRequiredService<IMediator>();
            var scopedMediator2 = scopedServiceProvider.GetRequiredService<IMediator>();

            rootMediator1.ShouldBe(rootMediator2);
            rootMediator1.ShouldBe(scopedMediator1);
            scopedMediator1.ShouldBe(scopedMediator2);
        }
    }
}