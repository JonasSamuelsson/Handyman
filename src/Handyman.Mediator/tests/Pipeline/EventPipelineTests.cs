using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Handyman.Mediator.Tests.Pipeline
{
    public class EventPipelineTests
    {
        [Fact]
        public async Task ShouldBePossibleToReRunPartsOfThePipeline()
        {
            var filter = new Filter();

            var serviceProviders = new ServiceCollection()
                .AddTransient<IEventFilter<Event>, ReRunFilter>()
                .AddSingleton<IEventFilter<Event>>(filter)
                .AddTransient<IEventHandler<Event>, Handler>()
                .BuildServiceProvider();

            await new Mediator(serviceProviders).Publish(new Event(), CancellationToken.None);

            filter.Executions.ShouldBe(2);
        }

        public class Event : IEvent
        {
        }

        public class ReRunFilter : IEventFilter<Event>, IOrderedFilter
        {
            public int Order => 0;

            public async Task Execute(EventContext<Event> requestContext, EventFilterExecutionDelegate next)
            {
                await next();
                await next();
            }
        }

        public class Filter : IEventFilter<Event>, IOrderedFilter
        {
            public int Executions { get; set; }
            public int Order => 1;

            public Task Execute(EventContext<Event> requestContext, EventFilterExecutionDelegate next)
            {
                Executions++;
                return next();
            }
        }

        public class Handler : IEventHandler<Event>
        {
            public Task Handle(Event request, CancellationToken cancellationToken)
            {
                return Task.FromResult(Void.Instance);
            }
        }
    }
}