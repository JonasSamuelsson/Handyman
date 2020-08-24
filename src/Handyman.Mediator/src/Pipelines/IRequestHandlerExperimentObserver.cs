using System.Threading.Tasks;

namespace Handyman.Mediator.Pipelines
{
    public interface IRequestHandlerExperimentObserver
    {
        Task Observe<TRequest, TResponse>(RequestHandlerExperiment<TRequest, TResponse> experiment)
            where TRequest : IRequest<TResponse>;
    }
}