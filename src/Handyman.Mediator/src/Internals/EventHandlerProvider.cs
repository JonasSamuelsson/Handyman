using System.Collections.Generic;

namespace Handyman.Mediator.Internals
{
    internal class EventHandlerProvider : IEventHandlerProvider
    {
        internal static IEventHandlerProvider Instance = new EventHandlerProvider();

        public IEnumerable<IEventHandler<TEvent>> GetHandlers<TEvent>(ServiceProvider serviceProvider) where TEvent : IEvent
        {
            var type = typeof(IEnumerable<IEventHandler<TEvent>>);
            return (IEnumerable<IEventHandler<TEvent>>)serviceProvider.Invoke(type);
        }
    }
}