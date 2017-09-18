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

        public void Publish(IMessage message)
        {
            var messageType = message.GetType();
            foreach (var handler in GetMessageHandlers(messageType))
                handler.Handle(message);
        }

        public IEnumerable<Task> Publish(IAsyncMessage message)
        {
            var messageType = message.GetType();
            return GetAsyncMessageHandlers(messageType)
                .Select(handler => handler.Handle(message))
                .ToList();
        }

        public void Send(IRequest request)
        {
            var requestType = request.GetType();
            var handler = GetRequestHandler(requestType);
            handler.Handle(request);
        }

        public TResponse Send<TResponse>(IRequest<TResponse> request)
        {
            var requestType = request.GetType();
            var handler = GetRequestHandler<TResponse>(requestType);
            return handler.Handle(request);
        }

        private IEnumerable<IMessageHandler<IMessage>> GetMessageHandlers(Type messageType)
        {
            var context = _contexts.GetOrAdd(messageType, CallContextFactory.GetMessageCallContext);
            foreach (var handler in _getServices.Invoke(context.HandlerInterface))
            {
                yield return (IMessageHandler<IMessage>)context.AdapterFactory.Invoke(handler);
            }
        }

        private IEnumerable<IAsyncMessageHandler<IAsyncMessage>> GetAsyncMessageHandlers(Type messageType)
        {
            var context = _contexts.GetOrAdd(messageType, CallContextFactory.GetAsyncMessageCallContext);
            foreach (var handler in _getServices.Invoke(context.HandlerInterface))
            {
                yield return (IAsyncMessageHandler<IAsyncMessage>)context.AdapterFactory.Invoke(handler);
            }
        }

        private IRequestHandler<IRequest> GetRequestHandler(Type requestType)
        {
            var context = _contexts.GetOrAdd(requestType, CallContextFactory.GetRequestCallContext);
            var handler = _getService.Invoke(context.HandlerInterface);
            return (IRequestHandler<IRequest>)context.AdapterFactory.Invoke(handler);
        }

        private IRequestHandler<IRequest<TResponse>, TResponse> GetRequestHandler<TResponse>(Type requestType)
        {
            var context = _contexts.GetOrAdd(requestType, CallContextFactory.GetRequestResponseCallContext<TResponse>);
            var handler = _getService.Invoke(context.HandlerInterface);
            return (IRequestHandler<IRequest<TResponse>, TResponse>)context.AdapterFactory.Invoke(handler);
        }
    }
}