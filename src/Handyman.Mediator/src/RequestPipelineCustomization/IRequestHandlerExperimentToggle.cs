using System.Threading;
using System.Threading.Tasks;

namespace Handyman.Mediator.RequestPipelineCustomization
{
    public interface IRequestHandlerExperimentToggle<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        Task<bool> IsEnabled(TRequest request, CancellationToken cancellationToken);
    }
}