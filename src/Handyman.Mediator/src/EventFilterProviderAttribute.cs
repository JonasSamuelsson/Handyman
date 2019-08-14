using System;
using System.Collections.Generic;

namespace Handyman.Mediator
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public abstract class EventFilterProviderAttribute : Attribute, IEventFilterProvider
    {
        public abstract IEnumerable<IEventFilter<TEvent>> GetFilters<TEvent>(ServiceProvider serviceProvider) where TEvent : IEvent;
    }
}