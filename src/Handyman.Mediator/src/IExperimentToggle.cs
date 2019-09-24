using System.Threading;
using System.Threading.Tasks;

namespace Handyman.Mediator
{
    public interface IExperimentToggle<TRequest>
    {
        Task<bool> IsEnabled(TRequest request, CancellationToken cancellationToken);
    }
}