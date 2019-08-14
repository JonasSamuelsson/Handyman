using System;
using System.Collections.Generic;

namespace Handyman.Mediator
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public abstract class EventHandlerProviderAttribute : Attribute, IEventHandlerProvider
    {
        public abstract IEnumerable<IEventHandler<TEvent>> GetHandlers<TEvent>(ServiceProvider serviceProvider) where TEvent : IEvent;
    }
}