using Handyman.Mediator.RequestPipelineCustomization;
using System;
using System.Collections.Generic;

namespace Handyman.Mediator.Internals
{
    internal class RequestPipelineBuilder<TRequest, TResponse> : IRequestPipelineBuilder<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        public List<IRequestFilterSelector<TRequest, TResponse>> FilterSelectors { get; set; } = new List<IRequestFilterSelector<TRequest, TResponse>>();
        public List<IRequestHandlerSelector<TRequest, TResponse>> HandlerSelectors { get; set; } = new List<IRequestHandlerSelector<TRequest, TResponse>>();
        public IRequestHandlerExecutionStrategy HandlerExecutionStrategy { get; set; }

        public void AddFilterSelector(IRequestFilterSelector<TRequest, TResponse> requestFilterSelector)
        {
            FilterSelectors.Add(requestFilterSelector);
        }

        public void AddHandlerSelector(IRequestHandlerSelector<TRequest, TResponse> requestHandlerSelector)
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