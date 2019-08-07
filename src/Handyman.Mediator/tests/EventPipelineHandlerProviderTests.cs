using Shouldly;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Handyman.Mediator.Tests
{
    public class EventPipelineHandlerProviderTests
    {
        [Fact]
        public void ShouldProviderAnOrderedListOfHandlers()
        {
            var serviceProvider = new ServiceProvider(type => new IEventPipelineHandler<Event>[]
            {
                new HandlerC(),
                new HandlerB(),
                new HandlerA()
            });

            var handlers = new EventPipelineHandlerProvider()
                .GetHandlers<Event>(serviceProvider)
                .ToList();

            handlers[0].ShouldBeOfType<HandlerA>();
            handlers[1].ShouldBeOfType<HandlerB>();
            handlers[2].ShouldBeOfType<HandlerC>();
        }

        private class Event : IEvent { }

        private class HandlerA : IOrderedPipelineHandler, IEventPipelineHandler<Event>
        {
            public int Order => -1;

            public Task Handle(Event @event, CancellationToken cancellationToken, Func<Event, CancellationToken, Task> next)
            {
                throw new NotImplementedException();
            }
        }

        private class HandlerB : IEventPipelineHandler<Event>
        {
            public Task Handle(Event @event, CancellationToken cancellationToken, Func<Event, CancellationToken, Task> next)
            {
                throw new NotImplementedException();
            }
        }

        private class HandlerC : IOrderedPipelineHandler, IEventPipelineHandler<Event>
        {
            public int Order => 1;

            public Task Handle(Event @event, CancellationToken cancellationToken, Func<Event, CancellationToken, Task> next)
            {
                throw new NotImplementedException();
            }
        }
    }
}