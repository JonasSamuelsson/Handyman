using Maestro;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Handyman.Mediator.Tests
{
    public class EventPipelineTests
    {
        [Fact]
        public async Task ShouldNotExecutePipelineHandlersIfPipelineIsDisabledAsync()
        {
            var handler = new EventHandler();
            var pipelineHandler = new EventPipelineHandler();

            var container = new Container(x =>
            {
                x.Add<IEventHandler<Event>>().Instance(handler);
                x.Add<IEventPipelineHandler<Event>>().Instance(pipelineHandler);
            });

            var mediator = new Mediator(container.GetService);

            await Task.WhenAll(mediator.Publish(new Event()));

            handler.Executed.ShouldBeTrue();
            pipelineHandler.Executed.ShouldBeFalse();
        }

        [Fact]
        public async Task ShouldExecuteEventPipelineHandlers()
        {
            var handler = new EventHandler();
            var pipelineHandler = new EventPipelineHandler();

            var container = new Container(x =>
            {
                x.Add<IEventHandler<Event>>().Instance(handler);
                x.Add<IEventPipelineHandler<Event>>().Instance(pipelineHandler);
            });

            var mediator = new Mediator(container.GetService, new Configuration { EventPipelineEnabled = true });

            await Task.WhenAll(mediator.Publish(new Event()));

            handler.Executed.ShouldBeTrue();
            pipelineHandler.Executed.ShouldBeTrue();
        }

        private class Event : IEvent { }

        private class EventPipelineHandler : IEventPipelineHandler<Event>
        {
            public bool Executed { get; set; }

            public IEnumerable<Task> Handle(Event @event, CancellationToken cancellationToken, Func<Event, CancellationToken, IEnumerable<Task>> next)
            {
                Executed = true;
                return next(@event, cancellationToken);
            }
        }

        private class EventHandler : IEventHandler<Event>
        {
            public bool Executed { get; set; }

            public Task Handle(Event @event, CancellationToken cancellationToken)
            {
                Executed = true;
                return Task.CompletedTask;
            }
        }
    }
}