using System.Threading;
using System.Threading.Tasks;

namespace Handyman.Mediator.Internals
{
    internal class AlwaysOnExperimentToggle<TRequest> : IExperimentToggle<TRequest>
    {
        internal static readonly IExperimentToggle<TRequest> Instance = new AlwaysOnExperimentToggle<TRequest>();

        public Task<bool> IsEnabled(TRequest request, CancellationToken cancellationToken)
        {
            return Task.FromResult(true);
        }
    }
}