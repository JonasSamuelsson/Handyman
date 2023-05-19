using Handyman.Mediator;
using Microsoft.Extensions.DependencyInjection;

namespace MediatorSamples.Publisher;

public class PublisherSample : Sample
{
   public override async Task RunAsync(CancellationToken cancellationToken)
   {
      var mediator = ServiceProvider.GetRequiredService<IPublisher<Event>>();

      var @event = new Event();

      await mediator.Publish(@event, cancellationToken);
   }
}

public class Event : IEvent
{
}

public class Handler : IEventHandler<Event>
{
   public async Task Handle(Event @event, CancellationToken cancellationToken)
   {
      await Task.Yield();

      Console.WriteLine(GetType().Name);
   }
}