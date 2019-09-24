using System;
using System.Reflection;
using Handyman.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Handyman.Mediator.DependencyInjection
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

            services.Scan(_ =>
            {
                _.Assembly(assembly);
                _.RegisterConcreteClassesOf(typeof(IEventFilter<>));
                _.RegisterConcreteClassesOf(typeof(IEventHandler<>));
                _.RegisterConcreteClassesOf(typeof(IExperimentEvaluator<,>));
                _.RegisterConcreteClassesOf(typeof(IExperimentToggle<>));
                _.RegisterConcreteClassesOf(typeof(IRequestFilter<,>));
                _.RegisterConcreteClassesOf(typeof(IRequestHandler<,>));
            });
        }
    }
}
