using System.Collections.Generic;
using System.Threading.Tasks;

namespace Handyman.Mediator.Experiments
{
    public interface IExperimentEvaluator<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        Task Evaluate(TRequest request, Baseline<TRequest, TResponse> baseline, IEnumerable<Experiment<TRequest, TResponse>> experiments);
    }
}