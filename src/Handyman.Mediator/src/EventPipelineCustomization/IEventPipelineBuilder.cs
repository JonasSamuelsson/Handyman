namespace Handyman.Mediator.EventPipelineCustomization
{
    public interface IEventPipelineBuilder<TEvent> where TEvent : IEvent
    {
        void AddFilterSelector(IEventFilterSelector<TEvent> eventFilterSelector);
        void AddHandlerSelector(IEventHandlerSelector eventHandlerSelector);
        void UseHandlerExecutionStrategy(IEventHandlerExecutionStrategy eventHandlerExecutionStrategy);
    }
}