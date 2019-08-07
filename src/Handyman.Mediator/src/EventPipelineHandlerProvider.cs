using System.Collections.Generic;
using System.Linq;

namespace Handyman.Mediator
{
    internal class EventPipelineHandlerProvider : IEventPipelineHandlerProvider
    {
        internal static readonly IEventPipelineHandlerProvider Instance = new EventPipelineHandlerProvider();

        public virtual IEnumerable<IEventPipelineHandler<TEvent>> GetHandlers<TEvent>(ServiceProvider serviceProvider) where TEvent : IEvent
        {
            var type = typeof(IEnumerable<IEventPipelineHandler<TEvent>>);

            var handlers = (IEnumerable<IEventPipelineHandler<TEvent>>)serviceProvider.Invoke(type);

            if (handlers is List<IEventPipelineHandler<TEvent>> list)
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