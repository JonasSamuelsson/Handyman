using Handyman.AspNetCore.ApiVersioning.Abstractions;
using Handyman.AspNetCore.ApiVersioning.MajorMinorPreReleaseVersionScheme;
using Handyman.AspNetCore.ApiVersioning.ModelBinding;
using Handyman.AspNetCore.ApiVersioning.Routing;
using Microsoft.AspNetCore.Http;
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

        public static IServiceCollection AddApiVersioning(this IServiceCollection services, Action<ApiVersionOptions> configure)
        {
            var options = new ApiVersionOptions
            {
                InvalidApiVersionStatusCode = StatusCodes.Status400BadRequest
            };

            configure(options);

            if (services.Any(x => x.ServiceType == typeof(Sentinel)))
            {
                throw new InvalidOperationException("Api versioning has already been added.");
            }

            services.AddTransient<Sentinel>();

            services.AddSingleton<IActionDescriptorProvider, ApiVersionDescriptorProvider>();
            services.TryAddSingleton<IApiVersionParser, MajorMinorPreReleaseApiVersionParser>();
            services.TryAddSingleton<IApiVersionReader, QueryStringApiVersionReader>();
            services.AddSingleton<ApiVersionModelBinder>();
            services.AddControllers(mvcOptions => mvcOptions.ModelBinderProviders.Insert(0, new ApiVersionModelBinderProvider()));
            services.AddSingleton<MatcherPolicy, ApiVersionEndpointMatcherPolicy>();
            services.AddSingleton(options);

            return services;
        }

        private class Sentinel
        {
        }
    }
}