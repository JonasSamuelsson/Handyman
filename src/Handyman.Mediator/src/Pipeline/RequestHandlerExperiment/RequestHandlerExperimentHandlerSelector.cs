﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Handyman.Mediator.Pipeline.RequestHandlerExperiment
{
    internal class RequestHandlerExperimentHandlerSelector : IRequestHandlerSelector
    {
        private readonly RequestHandlerExperimentMetadata _metadata;
        private readonly IServiceProvider _serviceProvider;

        public RequestHandlerExperimentHandlerSelector(RequestHandlerExperimentMetadata metadata, IServiceProvider serviceProvider)
        {
            _metadata = metadata;
            _serviceProvider = serviceProvider;
        }

        public async Task SelectHandlers<TRequest, TResponse>(List<IRequestHandler<TRequest, TResponse>> handlers, RequestPipelineContext<TRequest> context)
            where TRequest : IRequest<TResponse>
        {
            if (handlers.Count <= 1)
                return;

            var toggle = _serviceProvider.GetRequiredService<IRequestHandlerExperimentToggle>();
            var isEnabled = await toggle.IsEnabled<TRequest, TResponse>(_metadata, context).ConfigureAwait();

            if (isEnabled == false)
            {
                handlers.RemoveAll(x => x.GetType() != _metadata.BaselineHandlerType);
                return;
            }

            var baselineHandler = GetBaselineHandler(handlers);
            var experimentalHandlers = handlers.Where(x => x != baselineHandler).ToList();
            var observer = _serviceProvider.GetRequiredService<IRequestHandlerExperimentObserver>();

            handlers.Clear();

            handlers.Add(new RequestHandlerExperimentHandler<TRequest, TResponse>(baselineHandler, experimentalHandlers, observer));
        }

        private IRequestHandler<TRequest, TResponse> GetBaselineHandler<TRequest, TResponse>(List<IRequestHandler<TRequest, TResponse>> handlers) where TRequest : IRequest<TResponse>
        {
            return handlers.Single(x => x.GetType() == _metadata.BaselineHandlerType);
        }
    }
}