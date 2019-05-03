using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Handyman.AspNetCore.ApiVersioning
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApiVersioning(this IServiceCollection services)
        {
            services.TryAddSingleton<IApiVersionReader, QueryStringApiVersionReader>();
            services.TryAddSingleton<IApiVersionValidator, SemanticVersionValidator>();

            return services;
        }
    }
}