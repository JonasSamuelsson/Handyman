using Handyman.AspNetCore.ApiVersioning;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;
using Shouldly;
using System.Linq;
using Xunit;

namespace Handyman.AspNetCore.Tests.ApiVersioning
{
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

            var validator = services.Single(x => x.ServiceType == typeof(IApiVersionValidator));
            validator.ImplementationType.ShouldBe(typeof(SemanticVersionValidator));
            validator.Lifetime.ShouldBe(ServiceLifetime.Singleton);
        }

        //[Fact]
        //public void ShouldHonorCustomServiceRegistrations()
        //{
        //    var services = new ServiceCollection();

        //    services.AddTransient<IApiVersionReader, TestReader>();
        //    services.AddTransient<IApiVersionValidator, TestValidator>();

        //    services.AddApiVersioning();

        //    services
        //        .Single(x => x.ServiceType == typeof(IApiVersionReader))
        //        .ImplementationType.ShouldBe(typeof(TestReader));

        //    services
        //        .Single(x => x.ServiceType == typeof(IApiVersionValidator))
        //        .ImplementationType.ShouldBe(typeof(TestValidator));
        //}

        //public class TestReader : IApiVersionReader
        //{
        //    public StringValues Read(HttpRequest request)
        //    {
        //        throw new System.NotImplementedException();
        //    }
        //}

        //public class TestValidator : IApiVersionValidator
        //{
        //    public bool Validate(string version, bool optional, StringValues validVersions, out string matchedVersion, out string error)
        //    {
        //        throw new System.NotImplementedException();
        //    }
        //}
    }
}