using System.Collections.Generic;
using System.Linq;

namespace Handyman.Mediator
{
    internal class RequestFilterProvider : IRequestFilterProvider
    {
        internal static readonly IRequestFilterProvider Instance = new RequestFilterProvider();

        public virtual IEnumerable<IRequestFilter<TRequest, TResponse>> GetFilters<TRequest, TResponse>(ServiceProvider serviceProvider) where TRequest : IRequest<TResponse>
        {
            var type = typeof(IEnumerable<IRequestFilter<TRequest, TResponse>>);

            var handlers = (IEnumerable<IRequestFilter<TRequest, TResponse>>)serviceProvider.Invoke(type);

            if (handlers is List<IRequestFilter<TRequest, TResponse>> list)
            {
                list.Sort(CompareHandlers);
                return list;
            }

            return handlers.OrderBy(GetSortOrder);
        }

        private static int CompareHandlers(object x, object y)
        {
            return GetSortOrder(x).CompareTo(GetSortOrder(y));
        }

        private static int GetSortOrder(object x)
        {
            return (x as IOrderedPipelineHandler)?.Order ?? 0;
        }
    }
}