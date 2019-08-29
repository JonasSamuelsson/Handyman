using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Handyman.Azure.Functions.Http.ApiVersioning
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApiVersioning(this IServiceCollection services)
        {
            services.TryAddSingleton<IApiVersionValidator, ApiVersionValidator>();
            services.TryAddSingleton<IApiVersionReader, QueryStringApiVersionReader>();
            services.TryAddSingleton<IValidationStrategy, SemanticVersionValidationStrategy>();
            return services;
        }
    }
}