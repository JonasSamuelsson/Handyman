namespace Handyman.Mediator.RequestPipelineCustomization
{
    public interface IRequestPipelineBuilder
    {
        void AddFilterSelector(IRequestFilterSelector requestFilterSelector);
        void AddHandlerSelector(IRequestHandlerSelector requestHandlerSelector);
        void UseHandlerExecutionStrategy(IRequestHandlerExecutionStrategy requestHandlerExecutionStrategy);
    }
}