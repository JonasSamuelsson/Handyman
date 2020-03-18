using System;
using System.Threading;
using System.Threading.Tasks;
using Handyman.Mediator;
using Handyman.Mediator.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace MediatorSamples.TerminatingEventFilters
{
    public static class Program
    {
        public static async Task Main()
        {
            var serviceProvider = new ServiceCollection()
                .AddMediator(options => options.ScanEntryAssembly())
                .BuildServiceProvider();

            var mediator = serviceProvider.GetRequiredService<IMediator>();

            await mediator.Publish(new Event { Terminate = false });
            await mediator.Publish(new Event { Terminate = true });
        }
    }

    internal class Event : IEvent
    {
        public bool Terminate { get; set; }
    }

    internal class Handler : IEventHandler<Event>
    {
        public Task Handle(Event @event, CancellationToken cancellationToken)
        {
            Console.WriteLine("handler executing");
            return Task.CompletedTask;
        }
    }

    internal class Filter : IEventFilter<Event>
    {
        public Task Execute(EventPipelineContext<Event> context, EventFilterExecutionDelegate next)
        {
            Console.WriteLine("filter executing");
            return context.Event.Terminate ? Task.CompletedTask : next();
        }
    }
}