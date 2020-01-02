using Handyman.Mediator.Internals;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Handyman.Mediator
{
    public class Mediator : IMediator
    {

        private static readonly EventPipelineFactory EventPipelineFactory = new EventPipelineFactory();
        private static readonly RequestPipelineFactory RequestPipelineFactory = new RequestPipelineFactory();

        private readonly IServiceProvider _serviceProvider;

        public Mediator(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public Task Publish(IEvent @event, CancellationToken cancellationToken)
        {
            var pipeline = EventPipelineFactory.GetPipeline(@event, _serviceProvider);
            return pipeline.Execute(@event, _serviceProvider, cancellationToken);
        }

        public Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken)
        {
            var pipeline = RequestPipelineFactory.GetPipeline(request, _serviceProvider);
            return pipeline.Execute(request, _serviceProvider, cancellationToken);
        }
    }
}