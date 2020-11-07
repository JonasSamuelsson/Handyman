using Handyman.Mediator.Pipeline;
using System.Collections.Generic;

namespace Handyman.Mediator
{
    public class MediatorOptions
    {
        internal static readonly MediatorOptions Default = new MediatorOptions();

        public IEventHandlerExecutionStrategy? EventHandlerExecutionStrategy { get; set; }
        public List<IEventPipelineBuilder> EventPipelineBuilders { get; } = new List<IEventPipelineBuilder>();
    }
}