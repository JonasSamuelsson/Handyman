using System;

namespace Handyman.Mediator.RequestPipelineCustomization
{
    [AttributeUsage(AttributeTargets.Class)]
    public class WhenAnyRequestHandlerAttribute : RequestPipelineBuilderAttribute
    {
        public override bool PipelineCanBeReused => true;

        public override void Configure(IRequestPipelineBuilder builder, IServiceProvider serviceProvider)
        {
            builder.UseHandlerExecutionStrategy(new WhenAnyRequestHandlerExecutionStrategy());
        }
    }
}