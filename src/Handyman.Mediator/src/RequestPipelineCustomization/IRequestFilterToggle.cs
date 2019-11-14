using System.Threading.Tasks;

namespace Handyman.Mediator.RequestPipelineCustomization
{
    public interface IRequestFilterToggle<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        Task<bool> IsEnabled(IRequestPipelineContext<TRequest> context);
    }
}