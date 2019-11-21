using System;
using System.Threading.Tasks;

namespace Handyman.Mediator.RequestPipelineCustomization
{
    public interface IRequestHandlerToggle<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        Task<bool> IsEnabled(Type requestHandlerType, RequestPipelineContext<TRequest> context);
    }
}