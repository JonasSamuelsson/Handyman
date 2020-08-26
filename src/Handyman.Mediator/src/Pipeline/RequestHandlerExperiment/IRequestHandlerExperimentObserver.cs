using System.Threading.Tasks;

namespace Handyman.Mediator.Pipeline.RequestHandlerExperiment
{
    public interface IRequestHandlerExperimentObserver
    {
        Task Observe<TRequest, TResponse>(RequestHandlerExperiment<TRequest, TResponse> experiment)
            where TRequest : IRequest<TResponse>;
    }
}