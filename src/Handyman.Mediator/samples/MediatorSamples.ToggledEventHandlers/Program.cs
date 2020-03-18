using Handyman.Mediator;
using Handyman.Mediator.DependencyInjection;
using Handyman.Mediator.EventPipelineCustomization;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MediatorSamples.ToggledEventHandlers
{
    public static class Program
    {
        public static async Task Main()
        {
            var serviceProvider = new ServiceCollection()
                .AddMediator(options => options.ScanEntryAssembly())
                .BuildServiceProvider();

            var mediator = serviceProvider.GetRequiredService<IMediator>();

            for (var i = 0; i < 4; i++)
            {
                await mediator.Publish(new Event());
                Toggle.Enabled = !Toggle.Enabled;
                Console.WriteLine();
            }
        }
    }

    [EventHandlerToggle(typeof(Handler1), ToggleName = "Handler1", ToggleDisabledHandlerType = typeof(Handler2))]
    [EventHandlerToggle(typeof(Handler3), ToggleName = "Handler3")]
    internal class Event : IEvent { }

    internal class Toggle : IEventHandlerToggle
    {
        public static bool Enabled { get; set; }

        public Task<bool> IsEnabled<TEvent>(EventHandlerToggleInfo toggleInfo, EventPipelineContext<TEvent> context)
            where TEvent : IEvent
        {
            Console.WriteLine($"evaluating {toggleInfo.ToggleName}");
            return Task.FromResult(Enabled);
        }
    }

    internal class Handler1 : IEventHandler<Event>
    {
        public Task Handle(Event @event, CancellationToken cancellationToken)
        {
            Console.WriteLine("handler 1");
            return Task.CompletedTask;
        }
    }

    internal class Handler2 : IEventHandler<Event>
    {
        public Task Handle(Event @event, CancellationToken cancellationToken)
        {
            Console.WriteLine("handler 2");
            return Task.CompletedTask;
        }
    }

    internal class Handler3 : IEventHandler<Event>
    {
        public Task Handle(Event @event, CancellationToken cancellationToken)
        {
            Console.WriteLine("handler 3");
            return Task.CompletedTask;
        }
    }
}