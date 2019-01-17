using Handyman.Mediator.Internals;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Handyman.Mediator
{
    public class Mediator : IMediator
    {
        private readonly ServiceProvider _serviceProvider;
        private readonly IEventHandlerFactoryBuilder _eventHandlerFactoryBuilder;
        private readonly IRequestHandlerFactoryBuilder _requestHandlerFactoryBuilder;

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

            _requestHandlerFactoryBuilder = configuration.RequestPipelineEnabled
                ? (IRequestHandlerFactoryBuilder)new PipelinedRequestHandlerFactoryBuilder()
                : new RequestHandlerFactoryBuilder();
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
            var requestType = request.GetType();
            var handler = GetRequestHandler<TResponse>(requestType);
            return handler.Handle(request, cancellationToken);
        }

        private IRequestHandler<IRequest<TResponse>, TResponse> GetRequestHandler<TResponse>(Type requestType)
        {
            var factory = _requestHandlerFactoryBuilder.BuildFactory(requestType, typeof(TResponse));
            return (IRequestHandler<IRequest<TResponse>, TResponse>)factory.Invoke(_serviceProvider);
        }
    }
}