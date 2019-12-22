using System;

namespace Handyman.Mediator.EventPipelineCustomization
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public abstract class EventPipelineBuilderAttribute : Attribute
    {
        public virtual bool PipelineCanBeReused => false;

        public abstract void Configure(IEventPipelineBuilder builder, IServiceProvider serviceProvider);
    }
}