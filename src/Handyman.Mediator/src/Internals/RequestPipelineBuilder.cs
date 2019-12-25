using Handyman.Mediator.RequestPipelineCustomization;
using System;
using System.Collections.Generic;

namespace Handyman.Mediator.Internals
{
    internal class RequestPipelineBuilder<TRequest, TResponse> : IRequestPipelineBuilder<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        public List<IRequestFilterSelector> FilterSelectors { get; set; } = new List<IRequestFilterSelector>();
        public List<IRequestHandlerSelector> HandlerSelectors { get; set; } = new List<IRequestHandlerSelector>();
        public IRequestHandlerExecutionStrategy HandlerExecutionStrategy { get; set; }

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