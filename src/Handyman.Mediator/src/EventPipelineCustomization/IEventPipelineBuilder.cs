namespace Handyman.Mediator.EventPipelineCustomization
{
    public interface IEventPipelineBuilder
    {
        void AddFilterSelector(IEventFilterSelector eventFilterSelector);
        void AddHandlerSelector(IEventHandlerSelector eventHandlerSelector);
        void UseHandlerExecutionStrategy(IEventHandlerExecutionStrategy eventHandlerExecutionStrategy);
    }
}