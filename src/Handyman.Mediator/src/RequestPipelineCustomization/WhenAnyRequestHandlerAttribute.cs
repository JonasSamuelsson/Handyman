using System;
using Handyman.Mediator.Internals;

namespace Handyman.Mediator.RequestPipelineCustomization
{
    [AttributeUsage(AttributeTargets.Class)]
    public class WhenAnyRequestHandlerAttribute : RequestPipelineBuilderAttribute
    {
        public override void Configure<TRequest, TResponse>(IRequestPipelineBuilder<TRequest, TResponse> builder,
            IServiceProvider serviceProvider)
        {
            builder.UseHandlerExecutionStrategy(new WhenAnyRequestHandlerExecutionStrategy<TRequest, TResponse>());
        }
    }
}