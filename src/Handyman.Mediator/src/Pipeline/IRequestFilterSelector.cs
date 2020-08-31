using System.Collections.Generic;
using System.Threading.Tasks;

namespace Handyman.Mediator.Pipeline
{
    public interface IRequestFilterSelector
    {
        Task SelectFilters<TRequest, TResponse>(List<IRequestFilter<TRequest, TResponse>> filters, RequestContext<TRequest> requestContext)
            where TRequest : IRequest<TResponse>;
    }
}