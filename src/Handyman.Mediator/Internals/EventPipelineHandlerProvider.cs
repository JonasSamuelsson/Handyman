using System.Collections.Generic;

namespace Handyman.Mediator.Internals
{
    internal class EventPipelineHandlerProvider : IEventPipelineHandlerProvider
    {
        public static IEventPipelineHandlerProvider Instance = new EventPipelineHandlerProvider();

        public IEnumerable<IEventPipelineHandler<TEvent>> GetHandlers<TEvent>(ServiceProvider serviceProvider) where TEvent : IEvent
        {
            var type = typeof(IEnumerable<IEventPipelineHandler<TEvent>>);
            return (IEnumerable<IEventPipelineHandler<TEvent>>)serviceProvider.Invoke(type);
        }
    }
}