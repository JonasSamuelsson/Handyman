using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;

namespace Handyman.DependencyInjection.Tests
{
    public class ServiceConfigurationStrategyTests
    {
        [Fact]
        public void ShouldExecuteServiceConfigurationStrategies()
        {
            var serviceProvider = new ServiceCollection()
                .Scan(x => x.Types(GetType().GetNestedTypes()).ExecuteServiceConfigurationStrategies())
                .BuildServiceProvider();

            serviceProvider.GetService<object>().ShouldBe("success");
        }

        public class ServiceConfigurationStrategy : IServiceConfigurationStrategy
        {
            public void Execute(IServiceCollection services)
            {
                services.AddSingleton<object>("success");
            }
        }
    }
}
