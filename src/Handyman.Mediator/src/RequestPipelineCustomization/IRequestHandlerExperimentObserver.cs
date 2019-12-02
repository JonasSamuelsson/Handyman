using System.Collections.Generic;
using System.Threading.Tasks;

namespace Handyman.Mediator.RequestPipelineCustomization
{
    public interface IRequestHandlerExperimentObserver<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        Task Observe(RequestHandlerExperiment<TRequest, TResponse> experiment);
    }
}