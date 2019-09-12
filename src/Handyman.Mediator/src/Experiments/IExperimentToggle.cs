using System.Threading;
using System.Threading.Tasks;

namespace Handyman.Mediator.Experiments
{
    internal interface IExperimentToggle<TRequest>
    {
        Task<bool> IsEnabled(TRequest request, CancellationToken cancellationToken);
    }
}