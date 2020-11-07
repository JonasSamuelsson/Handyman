namespace Handyman.Mediator.Pipeline
{
    public interface IOrderedPipelineBuilder
    {
        int Order { get; }
    }
}