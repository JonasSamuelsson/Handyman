using System.Threading.Tasks;

namespace Handyman.Mediator.RequestPipelineCustomization
{
    public interface IRequestHandlerToggle
    {
        Task<bool> IsEnabled<TRequest, TResponse>(RequestHandlerToggleMetadata toggleMetadata,
            RequestPipelineContext<TRequest> context)
            where TRequest : IRequest<TResponse>;
    }
}