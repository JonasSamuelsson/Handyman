using Handyman.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Reflection;

namespace Handyman.Mediator.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMediator(this IServiceCollection services, Assembly assembly)
        {
            return services.AddMediator(assembly, delegate { });
        }

        public static IServiceCollection AddMediator(this IServiceCollection services, Assembly assembly, Action<Configuration> configure)
        {
            var config = new Configuration();

            configure(config);

            services.TryAddTransient<IMediator>(sp => new Mediator(sp.GetService, config));

            services.Scan(_ =>
            {
                _.Assembly(assembly);
                _.ConfigureConcreteClassesOf(typeof(IEventFilter<>));
                _.ConfigureConcreteClassesOf(typeof(IEventHandler<>));
                _.ConfigureConcreteClassesOf(typeof(IExperimentEvaluator<,>));
                _.ConfigureConcreteClassesOf(typeof(IExperimentToggle<>));
                _.ConfigureConcreteClassesOf(typeof(IRequestFilter<,>));
                _.ConfigureConcreteClassesOf(typeof(IRequestHandler<,>));
            });

            return services;
        }
    }
}
