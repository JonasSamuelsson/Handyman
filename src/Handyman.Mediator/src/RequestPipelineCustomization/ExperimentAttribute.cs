using System;
using Handyman.Mediator.Internals;

namespace Handyman.Mediator.RequestPipelineCustomization
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ExperimentAttribute : RequestPipelineBuilderAttribute
    {
        private readonly Type _baselineHandlerType;

        public ExperimentAttribute(Type baselineHandlerType)
        {
            _baselineHandlerType = baselineHandlerType;
        }

        public override void Configure<TRequest, TResponse>(IRequestPipelineBuilder<TRequest, TResponse> builder, ServiceProvider serviceProvider)
        {
            builder.UseHandlerExecutionStrategy(new ExperimentRequestHandlerExecutionStrategy<TRequest, TResponse>(_baselineHandlerType));
        }
    }
}