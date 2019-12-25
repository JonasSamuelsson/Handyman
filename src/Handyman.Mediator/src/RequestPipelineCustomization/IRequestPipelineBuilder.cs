namespace Handyman.Mediator.RequestPipelineCustomization
{
    public interface IRequestPipelineBuilder<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        void AddFilterSelector(IRequestFilterSelector<TRequest, TResponse> requestFilterSelector);
        void AddHandlerSelector(IRequestHandlerSelector requestHandlerSelector);
        void UseHandlerExecutionStrategy(IRequestHandlerExecutionStrategy requestHandlerExecutionStrategy);
    }
}