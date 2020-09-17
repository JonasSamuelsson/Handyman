using System;
using System.Collections.Generic;

namespace Handyman.Mediator.Pipeline
{
    internal class EventPipelineBuilder : IEventPipelineBuilder
    {
        internal List<IEventFilterSelector> FilterSelectors { get; } = new List<IEventFilterSelector>();
        internal List<IEventHandlerSelector> HandlerSelectors { get; } = new List<IEventHandlerSelector>();
        internal IEventHandlerExecutionStrategy? HandlerExecutionStrategy { get; set; }


        public void AddFilterSelector(IEventFilterSelector eventFilterSelector)
        {
            FilterSelectors.Add(eventFilterSelector);
        }

        public void AddHandlerSelector(IEventHandlerSelector eventHandlerSelector)
        {
            HandlerSelectors.Add(eventHandlerSelector);
        }

        public void UseHandlerExecutionStrategy(IEventHandlerExecutionStrategy eventHandlerExecutionStrategy)
        {
            if (HandlerExecutionStrategy != null)
            {
                throw new InvalidOperationException("Multiple execution strategies.");
            }

            HandlerExecutionStrategy = eventHandlerExecutionStrategy;
        }
    }
}