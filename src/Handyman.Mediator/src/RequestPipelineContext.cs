using System;
using System.Threading;

namespace Handyman.Mediator
{
    public class RequestPipelineContext<TRequest>
    {
        public CancellationToken CancellationToken { get; set; }
        public TRequest Request { get; set; }
        public IServiceProvider ServiceProvider { get; set; }
    }
}