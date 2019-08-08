using System.Collections.Generic;
using System.Linq;

namespace Handyman.Mediator.Internals
{
    internal class NoRequestFiltersProvider : IRequestFilterProvider
    {
        internal static readonly NoRequestFiltersProvider Instance = new NoRequestFiltersProvider();

        public IEnumerable<IRequestFilter<TRequest, TResponse>> GetFilters<TRequest, TResponse>(ServiceProvider serviceProvider) where TRequest : IRequest<TResponse>
        {
            return Enumerable.Empty<IRequestFilter<TRequest, TResponse>>();
        }
    }
}