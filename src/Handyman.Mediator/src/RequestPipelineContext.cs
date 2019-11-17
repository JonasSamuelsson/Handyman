using System.Threading;

namespace Handyman.Mediator
{
    public class RequestPipelineContext<TRequest>
    {
        public CancellationToken CancellationToken { get; set; }
        public TRequest Request { get; set; }
        public ServiceProvider ServiceProvider { get; set; }
    }
}