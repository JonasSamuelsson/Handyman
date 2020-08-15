using System.Threading.Tasks;

namespace Handyman.Mediator.RequestPipelineCustomization
{
    public interface IRequestHandlerExperimentToggle
    {
        Task<bool> IsEnabled<TRequest, TResponse>(RequestHandlerExperimentMetaData experimentMetaData, RequestPipelineContext<TRequest> context)
            where TRequest : IRequest<TResponse>;
    }
}