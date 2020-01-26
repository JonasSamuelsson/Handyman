using System;
using System.Threading.Tasks;

namespace Handyman.Mediator.RequestPipelineCustomization
{
    public interface IRequestFilterToggle
    {
        Task<bool> IsEnabled<TRequest, TResponse>(RequestFilterToggleInfo toggleInfo,
            RequestPipelineContext<TRequest> context)
            where TRequest : IRequest<TResponse>;
    }
}