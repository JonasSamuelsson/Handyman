using System.Threading.Tasks;

namespace Handyman.Mediator.RequestPipelineCustomization
{
    public interface IRequestHandlerExperimentToggle
    {
        Task<bool> IsEnabled<TRequest, TResponse>(RequestHandlerExperimentInfo experimentInfo, RequestPipelineContext<TRequest> context)
            where TRequest : IRequest<TResponse>;
    }
}