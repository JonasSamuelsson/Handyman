using System.Threading.Tasks;

namespace Handyman.Mediator.Pipelines
{
    public interface IRequestHandlerExperimentToggle
    {
        Task<bool> IsEnabled<TRequest, TResponse>(RequestHandlerExperimentMetadata experimentMetadata,
            RequestPipelineContext<TRequest> pipelineContext)
            where TRequest : IRequest<TResponse>;
    }
}