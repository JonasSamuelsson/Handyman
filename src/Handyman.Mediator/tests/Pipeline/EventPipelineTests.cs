using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System;
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
            var executionCounterFilter1 = new ExecutionCounterFilter { Order = 0 };
            var executionCounterFilter2 = new ExecutionCounterFilter { Order = 2 };

            var serviceProviders = new ServiceCollection()
                .AddSingleton<IEventFilter<Event>>(executionCounterFilter1)
                .AddSingleton<IEventFilter<Event>>(executionCounterFilter2)
                .AddTransient<IEventFilter<Event>, RetryFilter>()
                .AddTransient<IEventHandler<Event>, Handler>()
                .BuildServiceProvider();

            var @event = new Event();

            await new Mediator(serviceProviders).Publish(@event, CancellationToken.None);

            executionCounterFilter1.Executions.ShouldBe(1);
            executionCounterFilter2.Executions.ShouldBe(2);
            @event.Value.ShouldBe(2);
        }

        public class Event : IEvent
        {
            public int Value { get; set; }
        }

        public class ExecutionCounterFilter : IEventFilter<Event>, IOrderedFilter
        {
            public int Executions { get; set; }
            public int Order { get; set; }

            public Task Execute(EventContext<Event> eventContext, EventFilterExecutionDelegate next)
            {
                Executions++;
                return next();
            }
        }

        public class RetryFilter : IEventFilter<Event>, IOrderedFilter
        {
            public int Order => 1;

            public async Task Execute(EventContext<Event> eventContext, EventFilterExecutionDelegate next)
            {
                try
                {
                    await next();
                }
                catch
                {
                    await next();
                }
            }
        }

        public class Handler : IEventHandler<Event>
        {
            private int _counter;

            public async Task Handle(Event @event, CancellationToken cancellationToken)
            {
                await Task.Yield();

                if (_counter++ == 0)
                {
                    throw new Exception();
                }

                @event.Value = _counter;
            }
        }
    }
}