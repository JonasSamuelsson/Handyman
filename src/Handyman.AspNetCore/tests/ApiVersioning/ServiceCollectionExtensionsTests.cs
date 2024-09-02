using Handyman.AspNetCore.ApiVersioning;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System.Linq;
using Xunit;

namespace Handyman.AspNetCore.Tests.ApiVersioning;

public class ServiceCollectionExtensionsTests
{
    [Fact]
    public void ShouldConfigureDefaultServices()
    {
        var services = new ServiceCollection();

        services.AddApiVersioning();

        var reader = services.Single(x => x.ServiceType == typeof(IApiVersionReader));
        reader.ImplementationType.ShouldBe(typeof(QueryStringApiVersionReader));
        reader.Lifetime.ShouldBe(ServiceLifetime.Singleton);
    }
}