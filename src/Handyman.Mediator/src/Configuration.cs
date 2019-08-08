using Handyman.Mediator.Internals;

namespace Handyman.Mediator
{
    public class Configuration
    {
        public IEventFilterProvider EventFilterProvider { get; set; }
        public IEventHandlerProvider EventHandlerProvider { get; set; }
        public IRequestFilterProvider RequestFilterProvider { get; set; }
        public IRequestHandlerProvider RequestHandlerProvider { get; set; }

        internal IEventFilterProvider GetEventFilterProvider() => EventFilterProvider ?? DefaultEventFilterProvider.Instance;
        internal IEventHandlerProvider GetEventHandlerProvider() => EventHandlerProvider ?? DefaultEventHandlerProvider.Instance;
        internal IRequestFilterProvider GetRequestFilterProvider() => RequestFilterProvider ?? DefaultRequestFilterProvider.Instance;
        internal IRequestHandlerProvider GetRequestHandlerProvider() => RequestHandlerProvider ?? DefaultRequestHandlerProvider.Instance;
    }
}