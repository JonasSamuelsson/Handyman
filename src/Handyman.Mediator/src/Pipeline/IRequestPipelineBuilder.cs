using System.Threading.Tasks;

namespace Handyman.Mediator.Pipeline
{
    public interface IRequestPipelineBuilder
    {
        Task Execute<TRequest, TResponse>(RequestPipelineBuilderContext<TRequest, TResponse> pipelineBuilderContext, RequestContext<TRequest> requestContext)
            where TRequest : IRequest<TResponse>;
    }
}