using Handyman.Mediator;
using Handyman.Mediator.Pipeline.EventFilterToggle;
using Microsoft.Extensions.DependencyInjection;

namespace MediatorSamples.EventFilterToggle;

public class EventFilterToggleSample : Sample
{
    public override async Task RunAsync(CancellationToken cancellationToken)
    {
        var mediator = ServiceProvider.GetRequiredService<IMediator>();

        await mediator.Publish(new Event(), cancellationToken);
        await mediator.Publish(new Event(), cancellationToken);
    }
}

[EventFilterToggle<Filter>]
public class Event : IEvent
{
}

public class Filter : IEventFilter<Event>
{
    public async Task Execute(EventContext<Event> eventContext, EventFilterExecutionDelegate next)
    {
        Console.WriteLine("Filter");
        await next();
    }
}

public class Handler : IEventHandler<Event>
{
    public Task Handle(Event @event, CancellationToken cancellationToken)
    {
        Console.WriteLine("Handler");
        return Task.CompletedTask;
    }
}

public class Toggle : IEventFilterToggle
{
    private static bool _enabled;

    public Task<bool> IsEnabled<TEvent>(EventFilterToggleMetadata toggleMetadata, EventContext<TEvent> eventContext) where TEvent : IEvent
    {
        _enabled = !_enabled;
        return Task.FromResult(_enabled);
    }
}