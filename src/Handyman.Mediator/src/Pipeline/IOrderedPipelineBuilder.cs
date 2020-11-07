namespace Handyman.Mediator.Pipeline
{
    public interface IOrderedPipelineBuilder
    {
        int ExecutionOrder { get; }
    }
}