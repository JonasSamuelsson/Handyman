using System.Collections.Generic;
using System.Threading.Tasks;

namespace Handyman.Mediator.RequestPipelineCustomization
{
    public interface IRequestHandlerExperimentObserver
    {
        Task Observe<TRequest, TResponse>(RequestHandlerExperiment<TRequest, TResponse> experiment)
            where TRequest : IRequest<TResponse>;
    }
}