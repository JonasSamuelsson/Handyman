using Handyman.DependencyInjection.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;

namespace Handyman.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddServiceProviderInsights(this IServiceCollection services)
        {
            services.TryAddSingleton<IServiceProviderInsights>(new ServiceProviderInsights(services));
            return services;
        }

        public static IServiceCollection Scan(this IServiceCollection services, Action<IScanner> action)
        {
            var scanner = new Scanner();
            action(scanner);
            scanner.Execute(services);
            return services;
        }
    }
}
