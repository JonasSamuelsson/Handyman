using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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

        public IEnumerable<Task> Handle(IEvent @event, CancellationToken cancellationToken)
        {
            return Handle((TEvent)@event, cancellationToken);
        }

        private IEnumerable<Task> Handle(TEvent @event, CancellationToken cancellationToken)
        {
            var count = _handlers.Count;
            var tasks = new List<Task>(count);

            for (var i = 0; i < count; i++)
            {
                var handler = _handlers[i];
                var task = handler.Handle(@event, cancellationToken);
                tasks.Add(task);
            }

            return tasks;
        }
    }
}