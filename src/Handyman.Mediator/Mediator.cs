using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Handyman.Mediator
{
    public class Mediator : IMediator
    {
        private readonly Func<Type, object> _handlerProvider;
        private readonly Func<Type, IEnumerable<object>> _handlersProvider;
        private readonly ConcurrentDictionary<Type, CallContext> _contexts = new ConcurrentDictionary<Type, CallContext>();

        public Mediator(IHandlerProvider handlerProvider)
        {
            _handlerProvider = handlerProvider.GetHandler;
            _handlersProvider = handlerProvider.GetHandlers;
        }

        public Mediator(Func<Type, object> handlerProvider, Func<Type, IEnumerable<object>> handlersProvider)
        {
            _handlerProvider = handlerProvider;
            _handlersProvider = handlersProvider;
        }

        public Task<TResponse> Send<TResponse>(IRequest<TResponse> request)
        {
            var requestType = request.GetType();
            var handler = GetRequestHandler<TResponse>(requestType);
            return handler.Handle(request);
        }

        private IRequestHandler<IRequest<TResponse>, TResponse> GetRequestHandler<TResponse>(Type requestType)
        {
            var context = _contexts.GetOrAdd(requestType, CallContextFactory.GetRequestCallContext<TResponse>);
            var handler = _handlerProvider.Invoke(context.HandlerInterface);
            return (IRequestHandler<IRequest<TResponse>, TResponse>)context.AdapterFactory.Invoke(handler);
        }

        public IEnumerable<Task> Publish(IMessage message)
        {
            var messageType = message.GetType();
            return GetMessageHandlers(messageType)
                .Select(handler => handler.Handle(message))
                .ToList();
        }

        private IEnumerable<IMessageHandler<IMessage>> GetMessageHandlers(Type messageType)
        {
            var context = _contexts.GetOrAdd(messageType, CallContextFactory.GetMessageCallContext);
            foreach (var handler in _handlersProvider.Invoke(context.HandlerInterface))
            {
                yield return (IMessageHandler<IMessage>)context.AdapterFactory.Invoke(handler);
            }
        }
    }
}