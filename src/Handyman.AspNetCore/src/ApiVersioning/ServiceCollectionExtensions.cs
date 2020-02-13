using Handyman.AspNetCore.ApiVersioning.Abstractions;
using Handyman.AspNetCore.ApiVersioning.Filters;
using Handyman.AspNetCore.ApiVersioning.SemVer;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Handyman.AspNetCore.ApiVersioning
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApiVersioning(this IServiceCollection services)
        {
            services.TryAddSingleton<IActionDescriptorProvider, ApiVersionDescriptorProvider>();
            services.TryAddSingleton<IApiVersionParser, SemVerApiVersionParser>();
            services.TryAddSingleton<IApiVersionReader, QueryStringApiVersionReader>();

#if NETSTANDARD2_0
            services.TryAddSingleton<ApiVersionValidatorFilter>();
#else
            services.TryAddSingleton<MatcherPolicy, Routing.ApiVersionEndpointMatcherPolicy>();
#endif

            return services;
        }
    }
}