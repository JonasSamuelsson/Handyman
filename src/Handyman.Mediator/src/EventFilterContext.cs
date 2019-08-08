using System.Threading;

namespace Handyman.Mediator
{
    public class EventFilterContext<TEvent>
    {
        public CancellationToken CancellationToken { get; set; }
        public TEvent Event { get; set; }
    }
}