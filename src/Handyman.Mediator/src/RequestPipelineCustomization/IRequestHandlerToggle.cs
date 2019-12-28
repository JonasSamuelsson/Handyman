using System;
using System.Threading.Tasks;

namespace Handyman.Mediator.RequestPipelineCustomization
{
    public interface IRequestHandlerToggle
    {
        Task<bool> IsEnabled<TRequest, TResponse>(Type requestHandlerType, RequestPipelineContext<TRequest> context)
            where TRequest : IRequest<TResponse>;
    }
}