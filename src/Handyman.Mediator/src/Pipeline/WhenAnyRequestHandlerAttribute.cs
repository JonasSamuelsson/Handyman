using System;

namespace Handyman.Mediator.Pipeline
{
    [AttributeUsage(AttributeTargets.Class)]
    public class WhenAnyRequestHandlerAttribute : RequestPipelineBuilderAttribute
    {
        public override void Configure(IRequestPipelineBuilder builder, IServiceProvider serviceProvider)
        {
            builder.AddHandlerSelector(new WhenAnyRequestHandlerSelector());
        }
    }
}