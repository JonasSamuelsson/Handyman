using Handyman.Mediator;
using Handyman.Mediator.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MediatorSamples.EventFilters
{
    public static class Program
    {
        public static async Task Main()
        {
            var serviceProvider = new ServiceCollection()
                .AddMediator(options => options.ScanEntryAssembly())
                .BuildServiceProvider();

            var mediator = serviceProvider.GetRequiredService<IMediator>();

            await mediator.Publish(new Event());
        }
    }

    internal class Event : IEvent { }

    internal class Handler : IEventHandler<Event>
    {
        public Task Handle(Event @event, CancellationToken cancellationToken)
        {
            Console.WriteLine("handler");
            return Task.CompletedTask;
        }
    }

    internal class Filter : IEventFilter<Event>
    {
        public Task Execute(EventPipelineContext<Event> context, EventFilterExecutionDelegate next)
        {
            try
            {
                Console.WriteLine("pre processing");
                return next();
            }
            finally
            {
                Console.WriteLine("post processing");
            }
        }
    }
}