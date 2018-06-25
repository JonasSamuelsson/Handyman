using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Handyman.Mediator
{
    public class Mediator : IMediator
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ConcurrentDictionary<Type, Func<IServiceProvider, object>> _requestHandlerFactories = new ConcurrentDictionary<Type, Func<IServiceProvider, object>>();

        public Mediator(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IEnumerable<Task> Publish<TEvent>(TEvent @event)
            where TEvent : IEvent
        {
            var handlers = _serviceProvider.GetServices(typeof(IEventHandler<TEvent>));
            var handler = new DelegatingEventHandler<TEvent>(handlers.Cast<IEventHandler<TEvent>>());
            return handler.Handle(@event);
        }

        public Task<TResponse> Send<TResponse>(IRequest<TResponse> request)
        {
            var requestType = request.GetType();
            var handler = GetRequestHandler<TResponse>(requestType);
            return handler.Handle(request);
        }

        private IRequestHandler<IRequest<TResponse>, TResponse> GetRequestHandler<TResponse>(Type requestType)
        {
            var factory = _requestHandlerFactories.GetOrAdd(requestType, RequestHandlerFactoryBuilder.Create<TResponse>);
            return (IRequestHandler<IRequest<TResponse>, TResponse>)factory.Invoke(_serviceProvider);
        }
    }
}