using System;
using System.Collections.Generic;
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

            services.TryAddTransient<IMediator>(sp => new Mediator(sp.GetService, config));

            var types = assembly.GetTypes();

            services.AddMediatorEventFilters(types);
            services.AddMediatorEventHandlers(types);
            services.AddMediatorRequestFilters(types);
            services.AddMediatorRequestHandlers(types);
        }

        public static void AddMediatorEventFilters(this IServiceCollection services, IEnumerable<Type> types)
        {
            services.Register(types, typeof(IEventFilter<>));
        }

        public static void AddMediatorEventHandlers(this IServiceCollection services, IEnumerable<Type> types)
        {
            services.Register(types, typeof(IEventHandler<>));
        }

        public static void AddMediatorRequestFilters(this IServiceCollection services, IEnumerable<Type> types)
        {
            services.Register(types, typeof(IRequestFilter<,>));
        }

        public static void AddMediatorRequestHandlers(this IServiceCollection services, IEnumerable<Type> types)
        {
            services.Register(types, typeof(IRequestHandler<,>));
        }

        private static void Register(this IServiceCollection services, IEnumerable<Type> types, Type baseTypeDefinition)
        {
            foreach (var type in types)
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
}
