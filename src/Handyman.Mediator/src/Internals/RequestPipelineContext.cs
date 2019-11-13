using System.Threading;

namespace Handyman.Mediator.Internals
{
    internal class RequestPipelineContext<TRequest> : IRequestFilterContext<TRequest>, IRequestPipelineContext<TRequest>
    {
        public CancellationToken CancellationToken { get; set; }
        public TRequest Request { get; set; }
        public ServiceProvider ServiceProvider { get; set; }
    }
}