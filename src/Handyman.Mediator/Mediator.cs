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
        private readonly ConcurrentDictionary<Type, CallContext> _contexts = new ConcurrentDictionary<Type, CallContext>();

        public Mediator(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IEnumerable<Task> Publish(IEvent @event)
        {
            var eventType = @event.GetType();
            return GetEventHandlers(eventType)
                .Select(handler => handler.Handle(@event))
                .ToList();
        }

        public Task<TResponse> Send<TResponse>(IRequest<TResponse> request)
        {
            var requestType = request.GetType();
            var handler = GetRequestHandler<TResponse>(requestType);
            return handler.Handle(request);
        }

        private IEnumerable<IEventHandler<IEvent>> GetEventHandlers(Type eventType)
        {
            var context = _contexts.GetOrAdd(eventType, CallContextFactory.GetAsyncEventCallContext);
            foreach (var handler in _serviceProvider.GetServices(context.HandlerInterface))
            {
                yield return (IEventHandler<IEvent>)context.AdapterFactory.Invoke(handler);
            }
        }

        private IRequestHandler<IRequest<TResponse>, TResponse> GetRequestHandler<TResponse>(Type requestType)
        {
            var context = _contexts.GetOrAdd(requestType, CallContextFactory.GetRequestResponseCallContext<TResponse>);
            var handler = _serviceProvider.GetService(context.HandlerInterface);
            return (IRequestHandler<IRequest<TResponse>, TResponse>)context.AdapterFactory.Invoke(handler);
        }
    }
}