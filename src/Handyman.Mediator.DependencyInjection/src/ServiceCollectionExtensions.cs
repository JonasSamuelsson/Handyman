using Handyman.DependencyInjection;
using Handyman.Mediator.Pipeline;
using Handyman.Mediator.Pipeline.EventFilterToggle;
using Handyman.Mediator.Pipeline.EventHandlerToggle;
using Handyman.Mediator.Pipeline.RequestFilterToggle;
using Handyman.Mediator.Pipeline.RequestHandlerExperiment;
using Handyman.Mediator.Pipeline.RequestHandlerToggle;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
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

            var mediatorOptions = new Handyman.Mediator.MediatorOptions
            {
                EventHandlerExecutionStrategy = options.EventHandlerExecutionStrategy
            };
            services.Add(new ServiceDescriptor(typeof(IMediator), sp => new Mediator(sp, mediatorOptions), options.MediatorLifetime));
            services.TryAddTransient<IPublisher>(sp => sp.GetRequiredService<IMediator>());
            services.TryAddTransient(typeof(IPublisher<>), typeof(Publisher<>));
            services.TryAddTransient<ISender>(sp => sp.GetRequiredService<IMediator>());
            services.TryAddTransient(typeof(ISender<>), typeof(Sender<>));
            services.TryAddTransient(typeof(ISender<,>), typeof(Sender<,>));
            services.TryAddTransient<IDynamicMediator, DynamicMediator>();
            services.TryAddTransient<IDynamicPublisher>(sp => sp.GetRequiredService<IDynamicMediator>());
            services.TryAddTransient<IDynamicSender>(sp => sp.GetRequiredService<IDynamicMediator>());

            services.Scan(_ =>
            {
                _.Types(options.TypesToScan);
                _.ConfigureConcreteClassesOf(typeof(IBackgroundExceptionHandler));
                _.ConfigureConcreteClassesOf(typeof(IEventFilter<>));
                _.ConfigureConcreteClassesOf(typeof(IEventFilterToggle));
                _.ConfigureConcreteClassesOf(typeof(IEventHandler<>));
                _.ConfigureConcreteClassesOf(typeof(IEventHandlerToggle));
                _.ConfigureConcreteClassesOf(typeof(IRequestFilter<,>));
                _.ConfigureConcreteClassesOf(typeof(IRequestFilterToggle));
                _.ConfigureConcreteClassesOf(typeof(IRequestHandler<,>));
                _.ConfigureConcreteClassesOf(typeof(IRequestHandlerExperimentObserver));
                _.ConfigureConcreteClassesOf(typeof(IRequestHandlerExperimentToggle));
                _.ConfigureConcreteClassesOf(typeof(IRequestHandlerToggle));
                _.ConfigureConcreteClassesOf(typeof(IToggle));
            });

            return services;
        }
    }
}
