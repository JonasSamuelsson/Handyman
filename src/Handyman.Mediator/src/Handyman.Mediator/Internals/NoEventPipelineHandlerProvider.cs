using System.Collections.Generic;
using System.Linq;

namespace Handyman.Mediator.Internals
{
    internal class NoEventPipelineHandlerProvider : IEventPipelineHandlerProvider
    {
        internal static IEventPipelineHandlerProvider Instance = new NoEventPipelineHandlerProvider();

        public IEnumerable<IEventPipelineHandler<TEvent>> GetHandlers<TEvent>(ServiceProvider serviceProvider) where TEvent : IEvent
        {
            return Enumerable.Empty<IEventPipelineHandler<TEvent>>();
        }
    }
}