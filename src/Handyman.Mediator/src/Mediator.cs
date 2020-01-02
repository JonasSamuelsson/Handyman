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
        private readonly MediatorOptions _options;

        public Mediator(IServiceProvider serviceProvider)
        : this(serviceProvider, MediatorOptions.Default)
        {
        }

        public Mediator(IServiceProvider serviceProvider, MediatorOptions options)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _options = options ?? throw new ArgumentNullException(nameof(options));
        }

        public Task Publish(IEvent @event, CancellationToken cancellationToken)
        {
            var pipeline = EventPipelineFactory.GetPipeline(@event, _serviceProvider, _options);
            return pipeline.Execute(@event, _serviceProvider, cancellationToken);
        }

        public Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken)
        {
            var pipeline = RequestPipelineFactory.GetPipeline(request, _serviceProvider);
            return pipeline.Execute(request, _serviceProvider, cancellationToken);
        }
    }
}