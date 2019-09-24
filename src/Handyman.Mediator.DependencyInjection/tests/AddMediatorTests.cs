using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;

namespace Handyman.Mediator.DependencyInjection.Tests
{
    public class AddMediatorTests
    {
        [Fact]
        public void ShouldAddServices()
        {
            var services = new ServiceCollection();

            services.AddMediator(GetType().Assembly);

            services.Any(x => x.ServiceType == typeof(IMediator)).ShouldBeTrue();
        }
    }
}
