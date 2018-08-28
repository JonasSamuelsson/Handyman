using Shouldly;
using Xunit;

namespace Handyman.Mediator.Tests
{
    public class ServiceProviderTests
    {
        [Fact]
        public void ShouldDelegateToProvidedFunc()
        {
            var type = typeof(ServiceProviderTests);
            new ServiceProvider(t => t).GetService(type).ShouldBe(type);
        }
    }
}