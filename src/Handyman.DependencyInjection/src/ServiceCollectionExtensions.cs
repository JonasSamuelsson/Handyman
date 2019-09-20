using System;
using Microsoft.Extensions.DependencyInjection;

namespace Handyman.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static void AddDiagnostics(this IServiceCollection services)
        {
            services.AddSingleton<IDiagnostics>(new Diagnostics(services));
        }

        public static void Scan(this IServiceCollection services, Action<IScanner> action)
        {
            var scanner = new Scanner();
            action(scanner);
            scanner.Execute(services);
        }
    }
}
