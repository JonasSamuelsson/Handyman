using Handyman.Mediator.Internals;

namespace Handyman.Mediator
{
    public class Configuration
    {
        public IEventFilterProvider EventFilterProvider { get; set; }
        public IEventHandlerProvider EventHandlerProvider { get; set; }

        internal IEventFilterProvider GetEventFilterProvider() => EventFilterProvider ?? DefaultEventFilterProvider.Instance;
        internal IEventHandlerProvider GetEventHandlerProvider() => EventHandlerProvider ?? DefaultEventHandlerProvider.Instance;
    }
}