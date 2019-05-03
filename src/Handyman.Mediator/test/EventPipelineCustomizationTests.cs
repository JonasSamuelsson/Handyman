using Maestro;
using Shouldly;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Handyman.Mediator.Tests
{
    public class EventPipelineCustomizationTests
    {
        [Fact]
        public async Task ShouldCustomizePipelineExecutionOrder()
        {
            var container = new Container(x =>
            {
                x.Add<IEventHandler<Event>>().Instance(new EventHandler());
                x.Add<IEventPipelineHandler<Event>>().Instance(new EventPipelineHandler { String = "1" });
                x.Add<IEventPipelineHandler<Event>>().Instance(new EventPipelineHandler { String = "2" });
                x.Add<IEventPipelineHandler<Event>>().Instance(new EventPipelineHandler { String = "3" });
            });

            var provider = new EventPipelineHandlerProvider { Comparison = (a, b) => string.Compare(a.String, b.String, StringComparison.Ordinal) };

            var e1 = new Event();
            await new Mediator(container.GetService, new Configuration { EventPipelineHandlerProvider = provider }).Publish(e1);
            e1.List.ShouldBe(new[] { "1", "2", "3" });

            provider.Comparison = (a, b) => string.Compare(b.String, a.String, StringComparison.Ordinal);

            var e2 = new Event();
            await new Mediator(container.GetService, new Configuration { EventPipelineHandlerProvider = provider }).Publish(e2);
            e2.List.ShouldBe(new[] { "3", "2", "1" });
        }

        private class EventPipelineHandlerProvider : IEventPipelineHandlerProvider
        {
            public Comparison<EventPipelineHandler> Comparison { get; set; }

            public IEnumerable<IEventPipelineHandler<TEvent>> GetHandlers<TEvent>(ServiceProvider serviceProvider) where TEvent : IEvent
            {
                var type = typeof(IEnumerable<IEventPipelineHandler<TEvent>>);
                var handlers = ((IEnumerable)serviceProvider.Invoke(type)).OfType<EventPipelineHandler>().ToList();
                handlers.Sort(Comparison);
                return handlers.OfType<IEventPipelineHandler<TEvent>>();
            }
        }

        private class Event : IEvent
        {
            public List<string> List { get; set; } = new List<string>();
        }

        private class EventHandler : SyncEventHandler<Event>
        {
            protected override void Handle(Event @event) { }
        }

        private class EventPipelineHandler : IEventPipelineHandler<Event>
        {
            public string String { get; set; }

            public Task Handle(Event @event, CancellationToken cancellationToken, Func<Event, CancellationToken, Task> next)
            {
                @event.List.Add(String);
                return next(@event, cancellationToken);
            }
        }
    }
}
