using System;

namespace Handyman.Mediator.Pipeline
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public abstract class RequestPipelineBuilderAttribute : Attribute
    {
        public int ExecutionOrder { get; set; } = Handyman.Mediator.ExecutionOrder.Default;

        public abstract void Configure(IRequestPipelineBuilder builder, IServiceProvider serviceProvider);
    }
}