using Handyman.AspNetCore.ApiVersioning.Abstractions;
using Handyman.AspNetCore.ApiVersioning.Filters;
using Handyman.AspNetCore.ApiVersioning.Schemes.MajorMinorPreRelease;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;

namespace Handyman.AspNetCore.ApiVersioning
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApiVersioning(this IServiceCollection services)
        {
            return services.AddApiVersioning(delegate { });
        }

        public static IServiceCollection AddApiVersioning(this IServiceCollection services, Action<ApiVersioningOptions> configureAction)
        {
            var options = new ApiVersioningOptions { ApiVersionParserType = typeof(MajorMinorPreReleaseApiVersionParser) };

            configureAction.Invoke(options);

            services.TryAddSingleton<IActionDescriptorProvider, ApiVersionDescriptorProvider>();
            services.TryAddSingleton(typeof(IApiVersionParser), options.ApiVersionParserType);
            services.TryAddSingleton<IApiVersionReader, QueryStringApiVersionReader>();

#if NETCOREAPP3_0
            services.TryAddSingleton<MatcherPolicy, Routing.ApiVersionEndpointMatcherPolicy>();
#else
            services.TryAddSingleton<ApiVersionValidatorFilter>();
#endif

            return services;
        }
    }
}