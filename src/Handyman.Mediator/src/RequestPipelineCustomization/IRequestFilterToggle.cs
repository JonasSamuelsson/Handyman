using System.Threading.Tasks;

namespace Handyman.Mediator.RequestPipelineCustomization
{
    public interface IRequestFilterToggle
    {
        Task<bool> IsEnabled<TRequest, TResponse>(RequestFilterToggleMetadata toggleMetadata,
            RequestPipelineContext<TRequest> context)
            where TRequest : IRequest<TResponse>;
    }
}