using System.Collections.Generic;
using System.Linq;

namespace Handyman.Mediator
{
    internal class RequestPipelineHandlerProvider : IRequestPipelineHandlerProvider
    {
        internal static readonly IRequestPipelineHandlerProvider Instance = new RequestPipelineHandlerProvider();

        public virtual IEnumerable<IRequestPipelineHandler<TRequest, TResponse>> GetHandlers<TRequest, TResponse>(ServiceProvider serviceProvider) where TRequest : IRequest<TResponse>
        {
            var type = typeof(IEnumerable<IRequestPipelineHandler<TRequest, TResponse>>);

            var handlers = (IEnumerable<IRequestPipelineHandler<TRequest, TResponse>>)serviceProvider.Invoke(type);

            if (handlers is List<IRequestPipelineHandler<TRequest, TResponse>> list)
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