using System.Threading;

namespace Handyman.Mediator.EventPipelineCustomization
{
    internal class EventPipelineContext<TEvent> : IEventPipelineContext<TEvent>
    {
        public CancellationToken CancellationToken { get; set; }
        public TEvent Event { get; set; }
        public ServiceProvider ServiceProvider { get; set; }
    }
}