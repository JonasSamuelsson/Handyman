using Handyman.AspNetCore.ApiVersioning.Abstractions;
using Handyman.AspNetCore.ApiVersioning.ModelBinding;
using Handyman.AspNetCore.ApiVersioning.Routing;
using Handyman.AspNetCore.ApiVersioning.Schemes.MajorMinorPreRelease;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Linq;

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
            if (services.Any(x => x.ServiceType == typeof(Sentinel)))
                throw new InvalidOperationException("Api versioning has already been added.");

            services.AddTransient<Sentinel>();

            var options = new ApiVersioningOptions { ApiVersionParserType = typeof(MajorMinorPreReleaseApiVersionParser) };

            configureAction.Invoke(options);

            services.AddSingleton<ApiVersionModelBinder>();
            services.AddSingleton<IActionDescriptorProvider, ApiVersionDescriptorProvider>();
            services.TryAddSingleton(typeof(IApiVersionParser), options.ApiVersionParserType);
            services.TryAddSingleton<IApiVersionReader, QueryStringApiVersionReader>();
            services.AddControllers(mvcOptions => mvcOptions.ModelBinderProviders.Insert(0, new ApiVersionModelBinderProvider()));
            services.AddSingleton<MatcherPolicy, ApiVersionEndpointMatcherPolicy>();

            return services;
        }

        private class Sentinel { }
    }
}