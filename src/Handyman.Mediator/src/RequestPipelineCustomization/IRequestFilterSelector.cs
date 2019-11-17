using System.Collections.Generic;
using System.Threading.Tasks;

namespace Handyman.Mediator.RequestPipelineCustomization
{
    public interface IRequestFilterSelector<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        Task SelectFilters(List<IRequestFilter<TRequest, TResponse>> filters, RequestPipelineContext<TRequest> context);
    }
}