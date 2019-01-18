using Handyman.Mediator.Internals;
using System.Threading;
using System.Threading.Tasks;

namespace Handyman.Mediator
{
    public class Mediator : IMediator
    {
        private readonly ServiceProvider _serviceProvider;
        private readonly IEventHandlerFactoryBuilder _eventHandlerFactoryBuilder;
        private readonly Providers _providers;

        public Mediator(ServiceProvider serviceProvider)
            : this(serviceProvider, new Configuration())
        {
        }

        public Mediator(ServiceProvider serviceProvider, Configuration configuration)
        {
            _serviceProvider = serviceProvider;

            _eventHandlerFactoryBuilder = configuration.EventPipelineEnabled
                ? (IEventHandlerFactoryBuilder)new PipelinedEventHandlerFactoryBuilder()
                : new EventHandlerFactoryBuilder();

            _providers = new Providers
            {
                ServiceProvider = serviceProvider,
                RequestHandlerProvider = configuration.GetRequestHandlerProvider(),
                RequestPipelineHandlerProvider = configuration.GetRequestPipelineHandlerProvider()
            };
        }

        public Task Publish<TEvent>(TEvent @event, CancellationToken cancellationToken)
            where TEvent : IEvent
        {
            var handler = GetEventHandler<TEvent>();
            return handler.Handle(@event, cancellationToken);
        }

        private Internals.EventHandler<TEvent> GetEventHandler<TEvent>() where TEvent : IEvent
        {
            var factory = _eventHandlerFactoryBuilder.BuildFactory(typeof(TEvent));
            return (Internals.EventHandler<TEvent>)factory.Invoke(_serviceProvider);
        }

        public Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken)
        {
            return RequestProcessorProvider.GetRequestProcessor(request).Process(request, _providers, cancellationToken);
        }
    }
}