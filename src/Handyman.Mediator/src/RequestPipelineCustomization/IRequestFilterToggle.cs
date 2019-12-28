using System;
using System.Threading.Tasks;

namespace Handyman.Mediator.RequestPipelineCustomization
{
    public interface IRequestFilterToggle
    {
        Task<bool> IsEnabled<TRequest, TResponse>(Type requestFilterType, RequestPipelineContext<TRequest> context)
            where TRequest : IRequest<TResponse>;
    }
}