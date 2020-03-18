using Handyman.Mediator;
using Handyman.Mediator.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MediatorSamples.OrderedEventFilters
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

    internal class FirstFilter : IEventFilter<Event>, IOrderedFilter
    {
        public int Order => int.MinValue;

        public Task Execute(EventPipelineContext<Event> context, EventFilterExecutionDelegate next)
        {
            try
            {
                Console.WriteLine("first filter pre processing");
                return next();
            }
            finally
            {
                Console.WriteLine("first filter post processing");
            }
        }
    }

    internal class LastFilter : IEventFilter<Event>, IOrderedFilter
    {
        public int Order => int.MaxValue;

        public Task Execute(EventPipelineContext<Event> context, EventFilterExecutionDelegate next)
        {
            try
            {
                Console.WriteLine("last filter pre processing");
                return next();
            }
            finally
            {
                Console.WriteLine("last filter post processing");
            }
        }
    }

    internal class UnorderedFilter : IEventFilter<Event>
    {
        public Task Execute(EventPipelineContext<Event> context, EventFilterExecutionDelegate next)
        {
            try
            {
                Console.WriteLine("unordered filter pre processing");
                return next();
            }
            finally
            {
                Console.WriteLine("unordered filter post processing");
            }
        }
    }
}