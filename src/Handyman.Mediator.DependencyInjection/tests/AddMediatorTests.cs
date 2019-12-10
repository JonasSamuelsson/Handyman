using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System;
using System.Linq;
using Xunit;

namespace Handyman.Mediator.DependencyInjection.Tests
{
    public class AddMediatorTests
    {
        [Fact]
        public void ShouldAddServices()
        {
            var services = new ServiceCollection();

            services.AddMediator(delegate { });

            services.Any(x => x.ServiceType == typeof(IMediator)).ShouldBeTrue();
        }

        [Fact]
        public void ShouldThrowIfAddedMoreThanOnce()
        {
            var services = new ServiceCollection();

            services.AddMediator(delegate { });

            Should.Throw<InvalidOperationException>(() => services.AddMediator(delegate { }));
        }
    }
}
