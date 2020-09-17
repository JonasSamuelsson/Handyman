using Handyman.Mediator.Pipeline;

namespace Handyman.Mediator
{
    public class MediatorOptions
    {
        internal static readonly MediatorOptions Default = new MediatorOptions();

        public IEventHandlerExecutionStrategy EventHandlerExecutionStrategy { get; set; } = new WhenAllEventHandlerExecutionStrategy();
    }
}