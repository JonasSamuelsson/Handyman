using System;
using System.Collections.Generic;

namespace Handyman.Mediator.EventPipelineCustomization
{
    internal class EventPipelineBuilder : IEventPipelineBuilder
    {
        public List<IEventFilterSelector> FilterSelectors { get; set; } = new List<IEventFilterSelector>();
        public List<IEventHandlerSelector> HandlerSelectors { get; set; } = new List<IEventHandlerSelector>();
        public IEventHandlerExecutionStrategy HandlerExecutionStrategy { get; set; }


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