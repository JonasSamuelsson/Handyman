using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Handyman.Mediator
{
    public class Mediator : IMediator
    {
        private readonly ServiceProvider _serviceProvider;
        private readonly Configuration _configuration;
        private readonly ConcurrentDictionary<Type, Func<ServiceProvider, object>> _requestHandlerFactories = new ConcurrentDictionary<Type, Func<ServiceProvider, object>>();

        public Mediator(IServiceProvider serviceProvider)
            : this(serviceProvider, new Configuration())
        {
        }

        public Mediator(IServiceProvider serviceProvider, Configuration configuration)
        {
            _serviceProvider = new ServiceProvider(serviceProvider);
            _configuration = configuration;
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
            var useRequestPipeline = _configuration.UseRequestPipeline;
            var factory = _requestHandlerFactories.GetOrAdd(requestType, t => RequestHandlerFactoryBuilder.Create<TResponse>(t, useRequestPipeline));
            return (IRequestHandler<IRequest<TResponse>, TResponse>)factory.Invoke(_serviceProvider);
        }
    }
}