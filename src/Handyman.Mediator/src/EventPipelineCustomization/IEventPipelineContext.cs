using System.Threading;

namespace Handyman.Mediator.EventPipelineCustomization
{
    public interface IEventPipelineContext<TEvent>
    {
        CancellationToken CancellationToken { get; set; }
        TEvent Event { get; set; }
        ServiceProvider ServiceProvider { get; set; }
    }
}