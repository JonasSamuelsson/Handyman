﻿using System;
using System.Collections.Generic;

namespace Handyman.Mediator.Pipeline
{
    internal class RequestPipelineBuilder : IRequestPipelineBuilder
    {
        public List<IRequestFilterSelector> FilterSelectors { get; } = new List<IRequestFilterSelector>();
        public List<IRequestHandlerSelector> HandlerSelectors { get; } = new List<IRequestHandlerSelector>();
        public IRequestHandlerExecutionStrategy? HandlerExecutionStrategy { get; set; }

        public void AddFilterSelector(IRequestFilterSelector requestFilterSelector)
        {
            FilterSelectors.Add(requestFilterSelector);
        }

        public void AddHandlerSelector(IRequestHandlerSelector requestHandlerSelector)
        {
            HandlerSelectors.Add(requestHandlerSelector);
        }

        public void UseHandlerExecutionStrategy(IRequestHandlerExecutionStrategy requestHandlerExecutionStrategy)
        {
            if (HandlerExecutionStrategy != null)
            {
                throw new InvalidOperationException("Multiple execution strategies.");
            }

            HandlerExecutionStrategy = requestHandlerExecutionStrategy;
        }
    }
}