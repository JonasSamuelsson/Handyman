using Handyman.Mediator;
using Handyman.Mediator.Pipeline.EventHandlerToggle;
using Handyman.Mediator.Pipeline.RequestHandlerToggle;
using Microsoft.Extensions.DependencyInjection;

namespace MediatorSamples.EventHandlerToggle;

public class EventHandlerToggleSample : Sample
{
   public override async Task RunAsync(CancellationToken cancellationToken)
   {
      var mediator = ServiceProvider.GetRequiredService<IMediator>();

      await mediator.Publish(new Event(), cancellationToken);
   }
}

[RequestHandlerToggle(typeof(Handler1), ToggleDisabledHandlerTypes = new[] { typeof(Handler2) })]
public class Event : IEvent
{
}

public class Handler1 : IEventHandler<Event>
{
   public Task Handle(Event @event, CancellationToken cancellationToken)
   {
      Console.WriteLine("Handled by 1");
      return Task.CompletedTask;
   }
}

public class Handler2 : IEventHandler<Event>
{
   public Task Handle(Event @event, CancellationToken cancellationToken)
   {
      Console.WriteLine("Handled by 2");
      return Task.CompletedTask;
   }
}

public class Toggle : IEventHandlerToggle
{
   private static bool _enabled;

   public Task<bool> IsEnabled<TEvent>(EventHandlerToggleMetadata toggleMetadata, EventContext<TEvent> eventContext) where TEvent : IEvent
   {
      _enabled = !_enabled;
      return Task.FromResult(_enabled);
   }
}