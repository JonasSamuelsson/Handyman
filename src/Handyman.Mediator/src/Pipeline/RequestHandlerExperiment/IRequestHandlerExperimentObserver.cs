using System.Threading.Tasks;

namespace Handyman.Mediator.Pipeline.RequestHandlerExperiment
{
    public interface IRequestHandlerExperimentObserver<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        Task Observe(Experiment<TRequest, TResponse> experiment);
    }
}