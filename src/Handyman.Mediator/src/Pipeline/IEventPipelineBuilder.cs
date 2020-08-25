namespace Handyman.Mediator.Pipeline
{
    public interface IEventPipelineBuilder
    {
        void AddFilterSelector(IEventFilterSelector eventFilterSelector);
        void AddHandlerSelector(IEventHandlerSelector eventHandlerSelector);
        void UseHandlerExecutionStrategy(IEventHandlerExecutionStrategy eventHandlerExecutionStrategy);
    }
}