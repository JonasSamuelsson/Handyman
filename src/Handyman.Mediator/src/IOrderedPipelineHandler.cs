namespace Handyman.Mediator
{
    public interface IOrderedPipelineHandler
    {
        int Order { get; }
    }
}