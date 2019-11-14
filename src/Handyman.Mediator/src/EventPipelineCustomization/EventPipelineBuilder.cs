using System;
using System.Collections.Generic;

namespace Handyman.Mediator.EventPipelineCustomization
{
    internal class EventPipelineBuilder<TEvent> : IEventPipelineBuilder<TEvent> where TEvent : IEvent
    {
        public List<IEventFilterSelector<TEvent>> FilterSelectors { get; set; } = new List<IEventFilterSelector<TEvent>>();
        public List<IEventHandlerSelector<TEvent>> HandlerSelectors { get; set; } = new List<IEventHandlerSelector<TEvent>>();
        public IEventHandlerExecutionStrategy<TEvent> HandlerExecutionStrategy { get; set; }


        public void AddFilterSelector(IEventFilterSelector<TEvent> eventFilterSelector)
        {
            FilterSelectors.Add(eventFilterSelector);
        }

        public void AddHandlerSelector(IEventHandlerSelector<TEvent> eventHandlerSelector)
        {
            HandlerSelectors.Add(eventHandlerSelector);
        }

        public void UseHandlerExecutionStrategy(IEventHandlerExecutionStrategy<TEvent> eventHandlerExecutionStrategy)
        {
            if (HandlerExecutionStrategy != null)
            {
                throw new InvalidOperationException("Multiple execution strategies.");
            }

            HandlerExecutionStrategy = eventHandlerExecutionStrategy;
        }
    }
}