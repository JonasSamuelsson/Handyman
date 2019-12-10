using Handyman.DependencyInjection;
using Handyman.Mediator.EventPipelineCustomization;
using Handyman.Mediator.RequestPipelineCustomization;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Reflection;

namespace Handyman.Mediator.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMediator(this IServiceCollection services, Assembly assembly)
        {
            return services.AddMediator(x => x.ScanAssembly(assembly));
        }

        public static IServiceCollection AddMediator(this IServiceCollection services, Action<MediatorOptions> configure)
        {
            if (services.Any(x => x.ServiceType == typeof(IMediator)))
            {
                throw new InvalidOperationException("Mediator has already been added.");
            }

            var options = new MediatorOptions();

            configure(options);

            services.AddServiceProviderInsights();

            services.AddScoped<IMediator>(sp => new Mediator(sp));

            services.Scan(_ =>
            {
                foreach (var assembly in options.Assemblies)
                {
                    _.Assembly(assembly);
                }

                _.ConfigureConcreteClassesOf(typeof(IEventFilter<>));
                _.ConfigureConcreteClassesOf(typeof(IEventFilterToggle<>));
                _.ConfigureConcreteClassesOf(typeof(IEventHandler<>));
                _.ConfigureConcreteClassesOf(typeof(IEventHandlerToggle<>));
                _.ConfigureConcreteClassesOf(typeof(IExceptionHandler));
                _.ConfigureConcreteClassesOf(typeof(IRequestFilter<,>));
                _.ConfigureConcreteClassesOf(typeof(IRequestFilterToggle<,>));
                _.ConfigureConcreteClassesOf(typeof(IRequestHandler<,>));
                _.ConfigureConcreteClassesOf(typeof(IRequestHandlerExperimentToggle<,>));
                _.ConfigureConcreteClassesOf(typeof(IRequestHandlerToggle<,>));
            });

            return services;
        }
    }
}
