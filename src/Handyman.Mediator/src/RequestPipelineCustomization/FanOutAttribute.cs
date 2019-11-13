using System;
using Handyman.Mediator.Internals;

namespace Handyman.Mediator.RequestPipelineCustomization
{
    [AttributeUsage(AttributeTargets.Class)]
    public class FanOutAttribute : RequestPipelineBuilderAttribute
    {
        public override void Configure<TRequest, TResponse>(IRequestPipelineBuilder<TRequest, TResponse> builder, ServiceProvider serviceProvider)
        {
            builder.UseHandlerExecutionStrategy(new FanOutRequestHandlerExecutionStrategy<TRequest, TResponse>());
        }
    }
}