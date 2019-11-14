using System;
using System.Threading;
using System.Threading.Tasks;
using Handyman.Mediator.Internals;

namespace Handyman.Mediator
{
    public class Mediator : IMediator
    {
        private readonly ServiceProvider _serviceProvider;

        public Mediator(IServiceProvider serviceProvider)
            : this(serviceProvider.GetService)
        { }

        public Mediator(ServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public Task Publish<TEvent>(TEvent @event, CancellationToken cancellationToken)
            where TEvent : IEvent
        {
            var pipeline = EventPipelineFactory.CreatePipeline(@event, _serviceProvider);
            return pipeline.Execute(@event, _serviceProvider, cancellationToken);
        }

        public Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken)
        {
            var pipeline = RequestPipelineFactory.GetRequestPipeline(request, _serviceProvider);
            return pipeline.Execute(request, _serviceProvider, cancellationToken);
        }
    }
}