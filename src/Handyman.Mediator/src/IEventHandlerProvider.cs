using System.Collections.Generic;

namespace Handyman.Mediator
{
    public interface IEventHandlerProvider
    {
        IEnumerable<IEventHandler<TEvent>> GetHandlers<TEvent>(ServiceProvider serviceProvider) where TEvent : IEvent;
    }
}