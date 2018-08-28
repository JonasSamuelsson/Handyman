using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Handyman.Mediator
{
    public class Mediator : IMediator
    {
        private readonly bool _requestPipelineEnabled;
        private readonly ServiceProviderAdapter _serviceProvider;
        private readonly ConcurrentDictionary<Type, Func<ServiceProviderAdapter, object>> _requestHandlerFactories = new ConcurrentDictionary<Type, Func<ServiceProviderAdapter, object>>();

        public Mediator(IServiceProvider serviceProvider)
            : this(new Configuration { ServiceProvider = serviceProvider })
        {
        }

        public Mediator(Configuration configuration)
        {
            _requestPipelineEnabled = configuration.RequestPipelineEnabled;
            _serviceProvider = new ServiceProviderAdapter(configuration.ServiceProvider);
        }

        public IEnumerable<Task> Publish<TEvent>(TEvent @event, CancellationToken cancellationToken)
            where TEvent : IEvent
        {
            var handlers = _serviceProvider.GetServices(typeof(IEventHandler<TEvent>));
            var handler = new DelegatingEventHandler<TEvent>(handlers.Cast<IEventHandler<TEvent>>());
            return handler.Handle(@event, cancellationToken);
        }

        public Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken)
        {
            var requestType = request.GetType();
            var handler = GetRequestHandler<TResponse>(requestType);
            return handler.Handle(request, cancellationToken);
        }

        private IRequestHandler<IRequest<TResponse>, TResponse> GetRequestHandler<TResponse>(Type requestType)
        {
            var factory = _requestHandlerFactories.GetOrAdd(requestType, t => RequestHandlerFactoryBuilder.Create<TResponse>(t, _requestPipelineEnabled));
            return (IRequestHandler<IRequest<TResponse>, TResponse>)factory.Invoke(_serviceProvider);
        }
    }
}