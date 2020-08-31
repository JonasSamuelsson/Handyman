using Handyman.Mediator.Pipeline;
using Lamar;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Handyman.Mediator.Tests
{
    public class EventFilterTests
    {
        [Fact]
        public async Task ShouldExecuteFilters()
        {
            // using Lamar for dependency injection as it has support for constrained open generics

            var testContext = new TestContext();

            var container = new Container(services =>
            {
                services.AddSingleton(testContext);
                services.AddTransient(typeof(IEventFilter<>), typeof(Filter1<>));
                services.AddTransient(typeof(IEventFilter<>), typeof(Filter2<>));
            });

            await new Mediator(container).Publish(new Event());

            testContext.ExecutedFilters.ShouldBe(new[] { "Filter1" });
        }

        private class Event : IEvent, IMarker1 { }

        private interface IMarker1 { }

        private interface IMarker2 { }

        private class TestContext
        {
            public List<string> ExecutedFilters { get; set; } = new List<string>();
        }

        private class Filter1<TEvent> : IEventFilter<TEvent>
            where TEvent : IMarker1
        {
            private readonly TestContext _testContext;

            public Filter1(TestContext testContext)
            {
                _testContext = testContext;
            }

            public Task Execute(EventContext<TEvent> eventContext, EventFilterExecutionDelegate next)
            {
                _testContext.ExecutedFilters.Add("Filter1");
                return next();
            }
        }

        private class Filter2<TEvent> : IEventFilter<TEvent>
            where TEvent : IMarker2
        {
            private readonly TestContext _testContext;

            public Filter2(TestContext testContext)
            {
                _testContext = testContext;
            }

            public Task Execute(EventContext<TEvent> eventContext, EventFilterExecutionDelegate next)
            {
                _testContext.ExecutedFilters.Add("Filter2");
                return next();
            }
        }
    }
}