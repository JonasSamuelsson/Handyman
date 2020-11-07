namespace Handyman.Mediator.Pipeline
{
    internal static class Defaults
    {
        public static readonly IEventHandlerExecutionStrategy EventHandlerExecutionStrategy = new WhenAllEventHandlerExecutionStrategy();
        public static readonly IRequestHandlerExecutionStrategy RequestHandlerExecutionStrategy = new DefaultRequestHandlerExecutionStrategy();
    }
}