using Handyman.Mediator;
using Handyman.Mediator.DependencyInjection;
using Handyman.Mediator.EventPipelineCustomization;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace MediatorSamples.ToggledEventFilters
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

    [EventFilterToggle(typeof(Filter1), ToggleName = "Filter1", ToggleDisabledFilterType = typeof(Filter2))]
    [EventFilterToggle(typeof(Filter3), ToggleName = "Filter3")]
    internal class Event : IEvent { }

    internal class Toggle : IEventFilterToggle
    {
        public static bool Enabled { get; set; }

        public Task<bool> IsEnabled<TEvent>(EventFilterToggleInfo toggleInfo, EventPipelineContext<TEvent> context)
            where TEvent : IEvent
        {
            Console.WriteLine($"evaluating {toggleInfo.ToggleName}");
            return Task.FromResult(Enabled);
        }
    }

    internal class Filter1 : IEventFilter<Event>
    {
        public Task Execute(EventPipelineContext<Event> context, EventFilterExecutionDelegate next)
        {
            Console.WriteLine("filter 1");
            return next();
        }
    }

    internal class Filter2 : IEventFilter<Event>
    {
        public Task Execute(EventPipelineContext<Event> context, EventFilterExecutionDelegate next)
        {
            Console.WriteLine("filter 2");
            return next();
        }
    }

    internal class Filter3 : IEventFilter<Event>
    {
        public Task Execute(EventPipelineContext<Event> context, EventFilterExecutionDelegate next)
        {
            Console.WriteLine("filter 3");
            return next();
        }
    }
}