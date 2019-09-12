using System;
using System.Threading;
using System.Threading.Tasks;

namespace Handyman.Mediator.Experiments
{
    internal class PercentageExperimentToggle<TRequest> : IExperimentToggle<TRequest>
    {
        private static readonly Random Random = new Random();

        private readonly double _percentEnabled;

        public PercentageExperimentToggle(double percentEnabled)
        {
            _percentEnabled = percentEnabled;
        }

        public Task<bool> IsEnabled(TRequest request, CancellationToken cancellationToken)
        {
            var limit = Random.NextDouble() * 100;
            var value = 100 - _percentEnabled;
            return Task.FromResult(limit >= value);
        }
    }
}