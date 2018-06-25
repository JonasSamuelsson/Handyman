using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Handyman.Mediator
{
    internal class DelegatingEventHandler<TEvent>
        where TEvent : IEvent
    {
        private readonly IReadOnlyList<IEventHandler<TEvent>> _handlers;

        public DelegatingEventHandler(IEnumerable<IEventHandler<TEvent>> handlers)
        {
            _handlers = (handlers as IReadOnlyList<IEventHandler<TEvent>> ?? handlers?.ToArray()) ?? throw new ArgumentNullException(nameof(handlers));
        }

        internal IEnumerable<Task> Handle(IEvent @event)
        {
            return Handle((TEvent)@event);
        }

        private IEnumerable<Task> Handle(TEvent @event)
        {
            var count = _handlers.Count;
            var tasks = new List<Task>(count);

            for (var i = 0; i < count; i++)
            {
                var handler = _handlers[i];
                var task = handler.Handle(@event);
                tasks.Add(task);
            }

            return tasks;
        }
    }
}