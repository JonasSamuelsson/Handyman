using Handyman.Mediator;
using Handyman.Mediator.Pipeline;
using Microsoft.Extensions.DependencyInjection;

namespace MediatorSamples.PipelineCustomization;

public class PipelineCustomizationSample : Sample
{
   public override async Task RunAsync(CancellationToken cancellationToken)
   {
      var mediator = ServiceProvider.GetRequiredService<IMediator>();

      for (var i = 0; i < 5; i++)
      {
         await mediator.Publish(new Event(), cancellationToken);
      }
   }
}

[HandlerRoundRobin]
public class Event : IEvent
{
}

public class HandlerRoundRobinAttribute : EventPipelineBuilderAttribute
{
   private static int _counter;

   public override Task Execute<TEvent>(EventPipelineBuilderContext<TEvent> pipelineBuilderContext, EventContext<TEvent> eventContext)
   {
      var handlers = pipelineBuilderContext.Handlers.ToList();
      var index = _counter++ % handlers.Count;
      pipelineBuilderContext.Handlers.Clear();
      pipelineBuilderContext.Handlers.Add(handlers[index]);
      return Task.CompletedTask;
   }
}

public class Handler1 : IEventHandler<Event>
{
   public Task Handle(Event @event, CancellationToken cancellationToken)
   {
      Console.WriteLine("Handler 1");
      return Task.CompletedTask;
   }
}

public class Handler2 : IEventHandler<Event>
{
   public Task Handle(Event @event, CancellationToken cancellationToken)
   {
      Console.WriteLine("Handler 2");
      return Task.CompletedTask;
   }
}

public class Handler3 : IEventHandler<Event>
{
   public Task Handle(Event @event, CancellationToken cancellationToken)
   {
      Console.WriteLine("Handler 3");
      return Task.CompletedTask;
   }
}