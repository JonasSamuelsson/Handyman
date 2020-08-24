using System;
using System.Threading;

namespace Handyman.Mediator.Pipelines
{
    public class RequestPipelineContext<TRequest> : IPipelineContext
    {
        public CancellationToken CancellationToken { get; set; }
        public TRequest Request { get; set; }
        public IServiceProvider ServiceProvider { get; set; }

        object IPipelineContext.Message => Request;
    }
}