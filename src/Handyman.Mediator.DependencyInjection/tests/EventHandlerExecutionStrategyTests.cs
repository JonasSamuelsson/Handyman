using Handyman.Mediator.Pipeline;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Handyman.Mediator.DependencyInjection.Tests
{
    public class EventHandlerExecutionStrategyTests
    {
        [Fact]
        public async Task ShouldUseCustomEventHandlerExecutionStrategy()
        {
            var executionStrategy = new EventHandlerExecutionStrategy();

            var services = new ServiceCollection();

            services.AddMediator(x =>
            {
                x.ScanTypes(GetType().GetNestedTypes());
                x.EventHandlerExecutionStrategy = executionStrategy;
            });

            await services.BuildServiceProvider().GetService<IMediator>().Publish(new Event(), CancellationToken.None);

            executionStrategy.Executed.ShouldBeTrue();
        }

        public class EventHandlerExecutionStrategy : IEventHandlerExecutionStrategy
        {
            public bool Executed { get; set; }

            public Task Execute<TEvent>(List<IEventHandler<TEvent>> handlers, EventContext<TEvent> eventContext) where TEvent : IEvent
            {
                Executed = true;
                return Task.CompletedTask;
            }
        }

        private class Event : IEvent { }
    }
}