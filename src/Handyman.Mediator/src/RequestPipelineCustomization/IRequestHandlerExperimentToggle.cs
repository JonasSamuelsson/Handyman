using System.Threading;
using System.Threading.Tasks;

namespace Handyman.Mediator.RequestPipelineCustomization
{
    public interface IRequestHandlerExperimentToggle
    {
        Task<bool> IsEnabled<TRequest, TResponse>(RequestHandlerExperimentInfo experimentInfo, CancellationToken cancellationToken)
            where TRequest : IRequest<TResponse>;
    }
}