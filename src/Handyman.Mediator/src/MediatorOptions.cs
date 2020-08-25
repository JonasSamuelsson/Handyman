using Handyman.Mediator.Pipeline;

namespace Handyman.Mediator
{
    public class MediatorOptions
    {
        internal static readonly MediatorOptions Default = new MediatorOptions
        {
            EventHandlerExecutionStrategy = new WhenAllEventHandlerExecutionStrategy()
        };

        public IEventHandlerExecutionStrategy EventHandlerExecutionStrategy { get; set; }
    }
}