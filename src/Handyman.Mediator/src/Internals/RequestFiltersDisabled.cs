using System.Collections.Generic;
using System.Linq;

namespace Handyman.Mediator.Internals
{
    public class RequestFiltersDisabled : IRequestFilterProvider
    {
        public static readonly RequestFiltersDisabled Instance = new RequestFiltersDisabled();

        private RequestFiltersDisabled() { }

        public IEnumerable<IRequestFilter<TRequest, TResponse>> GetFilters<TRequest, TResponse>(ServiceProvider serviceProvider) where TRequest : IRequest<TResponse>
        {
            return Enumerable.Empty<IRequestFilter<TRequest, TResponse>>();
        }
    }
}