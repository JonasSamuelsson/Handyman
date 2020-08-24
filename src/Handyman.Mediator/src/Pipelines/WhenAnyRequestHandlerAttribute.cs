using System;

namespace Handyman.Mediator.Pipelines
{
    [AttributeUsage(AttributeTargets.Class)]
    public class WhenAnyRequestHandlerAttribute : RequestPipelineBuilderAttribute
    {
        public override void Configure(IRequestPipelineBuilder builder, IServiceProvider serviceProvider)
        {
            builder.UseHandlerExecutionStrategy(new WhenAnyRequestHandlerExecutionStrategy());
        }
    }
}