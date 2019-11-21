using System;
using System.Threading.Tasks;

namespace Handyman.Mediator.RequestPipelineCustomization
{
    public interface IRequestFilterToggle<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        Task<bool> IsEnabled(Type requestFilterType, RequestPipelineContext<TRequest> context);
    }
}