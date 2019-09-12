using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Handyman.Mediator.Internals
{
    internal class DefaultEventHandlerProvider : IEventHandlerProvider
    {
        internal static IEventHandlerProvider Instance = new DefaultEventHandlerProvider();

        private DefaultEventHandlerProvider() { }

        public IEnumerable<IEventHandler<TEvent>> GetHandlers<TEvent>(ServiceProvider serviceProvider) where TEvent : IEvent
        {
            var attribute = typeof(TEvent).GetCustomAttributes<EventHandlerProviderAttribute>(true).SingleOrDefault();

            if (attribute == null)
                return GetDefaultHandlers<TEvent>(serviceProvider);

            var handlers = GetDefaultHandlers<TEvent>(serviceProvider).ToListOptimized();
            handlers.AddRange(attribute.GetHandlers<TEvent>(serviceProvider));
            return handlers;
        }

        private static IEnumerable<IEventHandler<TEvent>> GetDefaultHandlers<TEvent>(ServiceProvider serviceProvider) where TEvent : IEvent
        {
            return serviceProvider.GetServices<IEventHandler<TEvent>>();
        }
    }
}