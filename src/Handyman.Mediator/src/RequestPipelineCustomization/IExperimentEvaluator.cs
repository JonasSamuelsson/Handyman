using System.Collections.Generic;
using System.Threading.Tasks;

namespace Handyman.Mediator.RequestPipelineCustomization
{
    public interface IExperimentEvaluator<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        Task Evaluate(TRequest request, ExperimentBaseline<TRequest, TResponse> experimentBaseline, IEnumerable<Experiment<TRequest, TResponse>> experiments);
    }
}