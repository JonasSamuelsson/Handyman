using System;
using System.Threading;

namespace Handyman.Mediator
{
    public class EventPipelineContext<TEvent>
    {
        public CancellationToken CancellationToken { get; set; }
        public TEvent Event { get; set; }
        public IServiceProvider ServiceProvider { get; set; }
    }
}