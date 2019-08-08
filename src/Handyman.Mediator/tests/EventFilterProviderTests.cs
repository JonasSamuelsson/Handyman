using Shouldly;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Handyman.Mediator.Tests
{
    public class EventFilterProviderTests
    {
        [Fact]
        public void ShouldProviderAnOrderedListOfFilters()
        {
            var serviceProvider = new ServiceProvider(type => new IEventFilter<Event>[]
            {
                new FilterC(),
                new FilterB(),
                new FilterA()
            });

            var handlers = new EventFilterProvider()
                .GetFilters<Event>(serviceProvider)
                .ToList();

            handlers[0].ShouldBeOfType<FilterA>();
            handlers[1].ShouldBeOfType<FilterB>();
            handlers[2].ShouldBeOfType<FilterC>();
        }

        private class Event : IEvent { }

        private class FilterA : IOrderedPipelineHandler, IEventFilter<Event>
        {
            public int Order => -1;

            public Task Execute(EventFilterContext<Event> context, EventFilterExecutionDelegate next)
            {
                throw new NotImplementedException();
            }
        }

        private class FilterB : IEventFilter<Event>
        {
            public Task Execute(EventFilterContext<Event> context, EventFilterExecutionDelegate next)
            {
                throw new NotImplementedException();
            }
        }

        private class FilterC : IOrderedPipelineHandler, IEventFilter<Event>
        {
            public int Order => 1;

            public Task Execute(EventFilterContext<Event> context, EventFilterExecutionDelegate next)
            {
                throw new NotImplementedException();
            }
        }
    }
}