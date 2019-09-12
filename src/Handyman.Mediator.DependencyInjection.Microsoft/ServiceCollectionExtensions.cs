using System;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Handyman.Mediator.DependencyInjection.Microsoft
{
    public static class ServiceCollectionExtensions
    {
        public static void AddMediator(this IServiceCollection services, Assembly assembly)
        {
            services.AddMediator(assembly, delegate { });
        }

        public static void AddMediator(this IServiceCollection services, Assembly assembly, Action<Configuration> configure)
        {
            var config = new Configuration();

            configure(config);

            var types = assembly.GetTypes();

            services.TryAddTransient<IMediator>(sp => new Mediator(sp.GetService, config));

            foreach (var type in types)
            {
                services.AddEventFilters(type);
                services.AddEventHandlers(type);
                services.AddRequestFilters(type);
                services.AddRequestHandlers(type);
            }
        }

        private static void AddEventFilters(this IServiceCollection services, Type type)
        {
            Register(type, typeof(IEventFilter<>), services);
        }

        private static void AddEventHandlers(this IServiceCollection services, Type type)
        {
            Register(type, typeof(IEventHandler<>), services);
        }

        private static void AddRequestFilters(this IServiceCollection services, Type type)
        {
            Register(type, typeof(IRequestFilter<,>), services);
        }

        private static void AddRequestHandlers(this IServiceCollection services, Type type)
        {
            Register(type, typeof(IRequestHandler<,>), services);
        }

        private static void Register(Type type, Type baseTypeDefinition, IServiceCollection services)
        {
            if (!type.IsConcreteClassClosing(baseTypeDefinition, out var baseTypes))
                return;

            foreach (var baseType in baseTypes)
            {
                services.AddTransient(baseType, type);
            }
        }
    }
}
