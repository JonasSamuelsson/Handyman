using Handyman.AspNetCore.ApiVersioning.Internals;
using Handyman.AspNetCore.ApiVersioning.Internals.ApiExplorer;
using Handyman.AspNetCore.ApiVersioning.Internals.AppModel;
using Handyman.AspNetCore.ApiVersioning.Internals.MajorMinorPreReleaseVersionScheme;
using Handyman.AspNetCore.ApiVersioning.Internals.ModelBinding;
using Handyman.AspNetCore.ApiVersioning.Internals.Routing;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Linq;

namespace Handyman.AspNetCore.ApiVersioning
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApiVersioning(this IServiceCollection services)
        {
            services.TryAddSingletonEnumerable<IActionDescriptorProvider, ApiVersionActionDescriptorProvider>();
            services.TryAddSingletonEnumerable<IApiDescriptionProvider, ApiVersionApiDescriptionProvider>();
            services.TryAddSingleton<IApiVersionParser, MajorMinorPreReleaseApiVersionParser>();
            services.TryAddSingleton<IApiVersionReader, QueryStringApiVersionReader>();
            services.TryAddSingleton<ApiVersionModelBinder>();
            services.TryAddSingletonEnumerable<MatcherPolicy, ApiVersionEndpointMatcherPolicy>();
            services.TryAddSingleton<ApiDescriptionGroupings>();

            services.AddControllers(options =>
            {
                if (options.ModelBinderProviders.OfType<ApiVersionModelBinderProvider>().Any() == false)
                {
                    options.ModelBinderProviders.Insert(0, new ApiVersionModelBinderProvider());
                }
            });

            return services;
        }

        private static void TryAddSingletonEnumerable<TService, TImplementation>(this IServiceCollection services)
        {
            var serviceType = typeof(TService);
            var implementationType = typeof(TImplementation);
            services.TryAddEnumerable(new ServiceDescriptor(serviceType, implementationType, ServiceLifetime.Singleton));
        }
    }
}