using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Handyman.Mediator.Internals
{
    internal class DefaultRequestFilterProvider : IRequestFilterProvider
    {
        internal static readonly IRequestFilterProvider Instance = new DefaultRequestFilterProvider();

        private DefaultRequestFilterProvider() { }

        public virtual IEnumerable<IRequestFilter<TRequest, TResponse>> GetFilters<TRequest, TResponse>(ServiceProvider serviceProvider) where TRequest : IRequest<TResponse>
        {
            var attribute = typeof(TRequest).GetCustomAttributes<RequestFilterProviderAttribute>(true).SingleOrDefault();

            if (attribute == null)
                return GetDefaultFilters<TRequest, TResponse>(serviceProvider);

            var filters = GetDefaultFilters<TRequest, TResponse>(serviceProvider).ToListOptimized();
            filters.AddRange(attribute.GetFilters<TRequest, TResponse>(serviceProvider));
            return filters;
        }

        private static IEnumerable<IRequestFilter<TRequest, TResponse>> GetDefaultFilters<TRequest, TResponse>(ServiceProvider serviceProvider) where TRequest : IRequest<TResponse>
        {
            return serviceProvider.GetServices<IRequestFilter<TRequest, TResponse>>();
        }
    }
}