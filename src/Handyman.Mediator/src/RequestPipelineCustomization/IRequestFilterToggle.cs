using System.Threading.Tasks;

namespace Handyman.Mediator.RequestPipelineCustomization
{
    public interface IRequestFilterToggle
    {
        Task<bool> IsEnabled<TRequest, TResponse>(RequestFilterToggleMetaData toggleMetaData,
            RequestPipelineContext<TRequest> context)
            where TRequest : IRequest<TResponse>;
    }
}