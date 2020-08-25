using System.Threading.Tasks;

namespace Handyman.Mediator.Pipeline
{
    public interface IRequestHandlerExperimentObserver
    {
        Task Observe<TRequest, TResponse>(RequestHandlerExperiment<TRequest, TResponse> experiment)
            where TRequest : IRequest<TResponse>;
    }
}