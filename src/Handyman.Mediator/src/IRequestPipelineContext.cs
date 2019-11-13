using System.Threading;

namespace Handyman.Mediator
{
    public interface IRequestPipelineContext<TRequest>
    {
        CancellationToken CancellationToken { get; set; }
        TRequest Request { get; set; }
        ServiceProvider ServiceProvider { get; set; }
    }
}