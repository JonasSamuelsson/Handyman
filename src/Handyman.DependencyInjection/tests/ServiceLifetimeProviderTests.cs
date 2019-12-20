using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;

namespace Handyman.DependencyInjection.Tests
{
    public class ServiceLifetimeProviderTests
    {
        [Fact]
        public void ShouldGetLifetime()
        {
            ServiceLifetimeProvider.GetLifetimeOrNullFromAttribute(typeof(Type1)).ShouldBeNull();
            ServiceLifetimeProvider.GetLifetimeOrNullFromAttribute(typeof(Type2)).ShouldBe(ServiceLifetime.Singleton);
        }

        private class Type1 { }

        [ServiceLifetime(ServiceLifetime.Singleton)]
        private class Type2 { }
    }
}