using System.Threading.Tasks;

namespace Handyman.Mediator.Pipelines
{
    public interface IRequestFilterToggle
    {
        Task<bool> IsEnabled<TRequest, TResponse>(RequestFilterToggleMetadata toggleMetadata,
            RequestPipelineContext<TRequest> pipelineContext)
            where TRequest : IRequest<TResponse>;
    }
}