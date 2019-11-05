using System;
using Microsoft.Extensions.DependencyInjection;

namespace Handyman.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDiagnostics(this IServiceCollection services)
        {
            return services.AddSingleton<IDiagnostics>(new Diagnostics(services));
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
