using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Handyman.Mediator
{
    public class Mediator : IMediator
    {
        private readonly Func<Type, object> _getService;
        private readonly Func<Type, IEnumerable<object>> _getServices;
        private readonly ConcurrentDictionary<Type, CallContext> _contexts = new ConcurrentDictionary<Type, CallContext>();
        
        public Mediator(Func<Type, object> getService, Func<Type, IEnumerable<object>> getServices)
        {
            _getService = getService;
            _getServices = getServices;
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
            var handler = _getService.Invoke(context.HandlerInterface);
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
            foreach (var handler in _getServices.Invoke(context.HandlerInterface))
            {
                yield return (IMessageHandler<IMessage>)context.AdapterFactory.Invoke(handler);
            }
        }
    }
}