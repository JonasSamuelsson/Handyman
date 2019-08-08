using System.Collections.Generic;

namespace Handyman.Mediator.Internals
{
    internal class DefaultEventHandlerProvider : IEventHandlerProvider
    {
        internal static IEventHandlerProvider Instance = new DefaultEventHandlerProvider();

        private DefaultEventHandlerProvider() { }

        public IEnumerable<IEventHandler<TEvent>> GetHandlers<TEvent>(ServiceProvider serviceProvider) where TEvent : IEvent
        {
            var type = typeof(IEnumerable<IEventHandler<TEvent>>);
            return (IEnumerable<IEventHandler<TEvent>>)serviceProvider.Invoke(type);
        }
    }
}