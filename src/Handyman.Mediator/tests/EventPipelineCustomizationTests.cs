using Maestro;
using Shouldly;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Handyman.Mediator.Tests
{
    public class EventPipelineCustomizationTests
    {
        [Fact]
        public async Task ShouldCustomizeFilterExecutionOrder()
        {
            var container = new Container(x =>
            {
                x.Add<IEventHandler<Event>>().Instance(new EventHandler());
                x.Add<IEventFilter<Event>>().Instance(new EventFilter { String = "1" });
                x.Add<IEventFilter<Event>>().Instance(new EventFilter { String = "2" });
                x.Add<IEventFilter<Event>>().Instance(new EventFilter { String = "3" });
            });

            var provider = new EventFilterProvider { Comparison = (a, b) => string.Compare(a.String, b.String, StringComparison.Ordinal) };

            var e1 = new Event();
            await new Mediator(container.GetService, new Configuration { EventFilterProvider = provider }).Publish(e1);
            e1.List.ShouldBe(new[] { "1", "2", "3" });

            provider.Comparison = (a, b) => string.Compare(b.String, a.String, StringComparison.Ordinal);

            var e2 = new Event();
            await new Mediator(container.GetService, new Configuration { EventFilterProvider = provider }).Publish(e2);
            e2.List.ShouldBe(new[] { "3", "2", "1" });
        }

        private class EventFilterProvider : IEventFilterProvider
        {
            public Comparison<EventFilter> Comparison { get; set; }

            public IEnumerable<IEventFilter<TEvent>> GetFilters<TEvent>(ServiceProvider serviceProvider) where TEvent : IEvent
            {
                var type = typeof(IEnumerable<IEventFilter<TEvent>>);
                var handlers = ((IEnumerable)serviceProvider.Invoke(type)).OfType<EventFilter>().ToList();
                handlers.Sort(Comparison);
                return handlers.OfType<IEventFilter<TEvent>>();
            }
        }

        private class Event : IEvent
        {
            public List<string> List { get; set; } = new List<string>();
        }

        private class EventHandler : EventHandler<Event>
        {
            protected override void Handle(Event @event) { }
        }

        private class EventFilter : IEventFilter<Event>
        {
            public string String { get; set; }

            public Task Execute(EventFilterContext<Event> context, EventFilterExecutionDelegate next)
            {
                context.Event.List.Add(String);
                return next();
            }
        }
    }
}
