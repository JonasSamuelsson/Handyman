using System.Threading;
using System.Threading.Tasks;

namespace Handyman.Mediator.RequestPipelineCustomization
{
    public interface IExperimentToggle<TRequest>
    {
        Task<bool> IsEnabled(TRequest request, CancellationToken cancellationToken);
    }
}