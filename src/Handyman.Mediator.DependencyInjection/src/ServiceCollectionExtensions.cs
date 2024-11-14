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
            services.AddServiceProviderInsights();

            var options = new MediatorOptions();

            configure(options);

            if (options.SkipCoreServices == false)
            {
                AddCoreServices(services, options);
            }

            AddUserServices(services, options);

            return services;
        }

        private static void AddCoreServices(IServiceCollection services, MediatorOptions options)
        {
            if (services.Any(x => x.ServiceType == typeof(IMediator)))
            {
                throw new InvalidOperationException("Mediator core services has already been added.");
            }

            var mediatorOptions = new Handyman.Mediator.MediatorOptions
            {
                EventHandlerExecutionStrategy = options.EventHandlerExecutionStrategy
            };

            services.TryAdd(new ServiceDescriptor(typeof(IMediator), sp => new Mediator(sp, mediatorOptions), options.MediatorLifetime));
            services.TryAddTransient<IPublisher>(sp => sp.GetRequiredService<IMediator>());
            services.TryAddTransient(typeof(IPublisher<>), typeof(Publisher<>));
            services.TryAddTransient<ISender>(sp => sp.GetRequiredService<IMediator>());
            services.TryAddTransient(typeof(ISender<>), typeof(Sender<>));
            services.TryAddTransient(typeof(ISender<,>), typeof(Sender<,>));
            services.TryAddTransient<IDynamicMediator, DynamicMediator>();
            services.TryAddTransient<IDynamicPublisher>(sp => sp.GetRequiredService<IDynamicMediator>());
            services.TryAddTransient<IDynamicSender>(sp => sp.GetRequiredService<IDynamicMediator>());
        }

        private static void AddUserServices(IServiceCollection services, MediatorOptions options)
        {
            var tryAdd = new ServiceConfigurationOptions
            {
                ConfigurationPolicy = ServiceConfigurationPolicy.TryAdd
            };

            var tryAddEnumerable = new ServiceConfigurationOptions
            {
                ConfigurationPolicy = ServiceConfigurationPolicy.TryAddEnumerable
            };

            services.Scan(_ =>
            {
                _.Types(options.TypesToScan);
                _.ConfigureConcreteClassesOf(typeof(IBackgroundExceptionHandler), tryAddEnumerable);
                _.ConfigureConcreteClassesOf(typeof(IEventFilter<>), tryAddEnumerable);
                _.ConfigureConcreteClassesOf(typeof(IEventFilterToggle), tryAdd);
                _.ConfigureConcreteClassesOf(typeof(IEventHandler<>), tryAddEnumerable);
                _.ConfigureConcreteClassesOf(typeof(IEventHandlerToggle), tryAdd);
                _.ConfigureConcreteClassesOf(typeof(IRequestFilter<,>), tryAddEnumerable);
                _.ConfigureConcreteClassesOf(typeof(IRequestFilterToggle), tryAdd);
                _.ConfigureConcreteClassesOf(typeof(IRequestHandler<,>), tryAddEnumerable);
                _.ConfigureConcreteClassesOf(typeof(IRequestHandlerExperimentObserver<,>), tryAddEnumerable);
                _.ConfigureConcreteClassesOf(typeof(IRequestHandlerExperimentToggle), tryAdd);
                _.ConfigureConcreteClassesOf(typeof(IRequestHandlerToggle), tryAdd);
                _.ConfigureConcreteClassesOf(typeof(IToggle), tryAdd);
            });
        }
    }
}