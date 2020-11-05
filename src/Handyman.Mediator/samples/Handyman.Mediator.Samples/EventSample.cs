using System;
using System.Threading;
using System.Threading.Tasks;

namespace Handyman.Mediator.Samples
{
    public class EventSample : ISample
    {
        public int Order => 30;
        public string Name => "Event";

        public async Task Run(IMediator mediator)
        {
            await mediator.Publish(new Event(), CancellationToken.None);
        }

        public class Event : IEvent { }

        public class FirstEventHandler : SyncEventHandler<Event>
        {
            protected override void Handle(Event @event, CancellationToken cancellationToken)
            {
                Console.WriteLine("Handled by first handler");
            }
        }

        public class SecondEventHandler : SyncEventHandler<Event>
        {
            protected override void Handle(Event @event, CancellationToken cancellationToken)
            {
                Console.WriteLine("Handled by second handler");
            }
        }
    }
}