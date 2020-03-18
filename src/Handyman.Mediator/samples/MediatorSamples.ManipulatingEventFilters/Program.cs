using Handyman.Mediator;
using Handyman.Mediator.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MediatorSamples.ManipulatingEventFilters
{
    public class Program
    {
        public static async Task Main()
        {
            var serviceProvider = new ServiceCollection()
                .AddMediator(options => options.ScanEntryAssembly())
                .BuildServiceProvider();

            var mediator = serviceProvider.GetRequiredService<IMediator>();

            await mediator.Publish(new Event { Message = "Hello world!" });
        }
    }

    internal class Event : IEvent
    {
        public string Message { get; set; }
    }

    internal class Filter : IEventFilter<Event>
    {
        public Task Execute(EventPipelineContext<Event> context, EventFilterExecutionDelegate next)
        {
            context.Event.Message += " =)";
            return next();
        }
    }

    internal class Handler : IEventHandler<Event>
    {
        public Task Handle(Event @event, CancellationToken cancellationToken)
        {
            Console.WriteLine(@event.Message);
            return Task.CompletedTask;
        }
    }
}
