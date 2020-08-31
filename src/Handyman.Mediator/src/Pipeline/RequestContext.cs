using System;
using System.Threading;

namespace Handyman.Mediator.Pipeline
{
    public class RequestContext<TRequest> : IPipelineContext
    {
        public CancellationToken CancellationToken { get; set; }
        public TRequest Request { get; set; }
        public IServiceProvider ServiceProvider { get; set; }

        object IPipelineContext.Message => Request;
    }
}