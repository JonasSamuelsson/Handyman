using System;

namespace Handyman.Mediator.RequestPipelineCustomization
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public abstract class RequestPipelineBuilderAttribute : Attribute
    {
        public virtual bool PipelineCanBeReused => false;

        public abstract void Configure(IRequestPipelineBuilder builder, IServiceProvider serviceProvider);
    }
}