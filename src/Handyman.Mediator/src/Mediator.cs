using System.Threading;
using System.Threading.Tasks;
using Handyman.Mediator.Internals;

namespace Handyman.Mediator
{
    public class Mediator : IMediator
    {
        private readonly Providers _providers;

        public Mediator(ServiceProvider serviceProvider)
            : this(serviceProvider, new Configuration())
        {
        }

        public Mediator(ServiceProvider serviceProvider, Configuration configuration)
        {
            _providers = new Providers
            {
                EventFilterProvider = configuration.GetEventFilterProvider(),
                EventHandlerProvider = configuration.GetEventHandlerProvider(),
                ServiceProvider = serviceProvider
            };
        }

        public Task Publish<TEvent>(TEvent @event, CancellationToken cancellationToken)
            where TEvent : IEvent
        {
            return EventPipeline.Execute(_providers, @event, cancellationToken);
        }

        public Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken)
        {
            var pipeline = RequestPipelineFactory.GetRequestPipeline(request, _providers.ServiceProvider);
            return pipeline.Execute(request, _providers.ServiceProvider, cancellationToken);
        }
    }
}