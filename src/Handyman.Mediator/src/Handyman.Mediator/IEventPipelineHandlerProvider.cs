using System.Collections.Generic;

namespace Handyman.Mediator
{
    public interface IEventPipelineHandlerProvider
    {
        IEnumerable<IEventPipelineHandler<TEvent>> GetHandlers<TEvent>(ServiceProvider serviceProvider) where TEvent : IEvent;
    }
}