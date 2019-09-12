using System;
using System.Collections.Generic;
using Handyman.Mediator.Experiments;

namespace Handyman.Mediator
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ExperimentAttribute : RequestHandlerProviderAttribute
    {
        public ExperimentAttribute(Type baselineHandlerType)
        {
            BaselineHandlerType = baselineHandlerType;
        }

        public Type BaselineHandlerType { get; }

        /// <summary>
        /// Specifies how often the experiment handlers should be executed, 0 = never & 100 = always.
        /// </summary>
        public double? PercentEnabled { get; set; }

        public override IRequestHandler<TRequest, TResponse> GetHandler<TRequest, TResponse>(ServiceProvider serviceProvider)
        {
            var handlers = GetHandlers<TRequest, TResponse>(serviceProvider);
            var toggle = GetToggle<TRequest>(serviceProvider);
            var evaluator = GetEvaluator<TRequest, TResponse>(serviceProvider);

            return new ExperimentRequestHandler<TRequest, TResponse>(BaselineHandlerType, handlers, toggle, evaluator);
        }

        private static IEnumerable<IRequestHandler<TRequest, TResponse>> GetHandlers<TRequest, TResponse>(ServiceProvider serviceProvider) where TRequest : IRequest<TResponse>
        {
            return serviceProvider.GetServices<IRequestHandler<TRequest, TResponse>>();
        }

        private IExperimentToggle<TRequest> GetToggle<TRequest>(ServiceProvider serviceProvider)
        {
            if (PercentEnabled.HasValue)
                return new PercentageExperimentToggle<TRequest>(PercentEnabled.Value);

            return serviceProvider.GetService<IExperimentToggle<TRequest>>()
                   ?? AlwaysOnExperimentToggle<TRequest>.Instance;
        }

        private static IExperimentEvaluator<TRequest, TResponse> GetEvaluator<TRequest, TResponse>(ServiceProvider serviceProvider) where TRequest : IRequest<TResponse>
        {
            return serviceProvider.GetRequiredService<IExperimentEvaluator<TRequest, TResponse>>();
        }
    }
}