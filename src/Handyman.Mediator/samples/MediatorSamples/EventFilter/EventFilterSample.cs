using Handyman.Mediator;
using Microsoft.Extensions.DependencyInjection;

namespace MediatorSamples.EventFilter;

public class EventFilterSample : Sample
{
   public override async Task RunAsync(CancellationToken cancellationToken)
   {
      var mediator = ServiceProvider.GetRequiredService<IMediator>();

      var @event = new Event();

      await mediator.Publish(@event, cancellationToken);
   }
}

public class Event : IEvent
{
}

public class FirstHandler : IEventHandler<Event>
{
   public async Task Handle(Event @event, CancellationToken cancellationToken)
   {
      await Task.Yield();
      Console.WriteLine(GetType().Name);
   }
}

public class SecondHandler : IEventHandler<Event>
{
   public async Task Handle(Event @event, CancellationToken cancellationToken)
   {
      await Task.Yield();
      Console.WriteLine(GetType().Name);
   }
}

public class Filter : IEventFilter<Event>
{
   public async Task Execute(EventContext<Event> eventContext, EventFilterExecutionDelegate next)
   {
      Console.WriteLine("Filter pre process");
      await next();
      Console.WriteLine("Filter post process");
   }
}