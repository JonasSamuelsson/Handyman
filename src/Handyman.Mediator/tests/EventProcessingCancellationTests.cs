using Maestro;
using Shouldly;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Handyman.Mediator.Tests
{
    public class EventProcessingCancellationTests
    {
        private static readonly Configuration Configuration = new Configuration
        {
            EventPipelineEnabled = true
        };

        [Fact]
        public async Task ShouldNotInvokeHandlerIfAlreadyCancelled()
        {
            var cts = new CancellationTokenSource();
            var handler = new EventHandler();

            var container = new Container(x =>
            {
                x.Add<IEventHandler<Event>>().Instance(handler);
            });

            var mediator = new Mediator(type => container.GetService(type), Configuration);

            cts.Cancel();

            (await Should.ThrowAsync<Exception>(Exec(() => mediator.Publish(new Event(), cts.Token))))
                .Message.ShouldBe("oce");

            handler.Executed.ShouldBeFalse();
        }

        [Fact]
        public async Task ShouldNotInvokePipelineHandlerIfAlreadyCancelled()
        {
            var cts = new CancellationTokenSource();
            var pipelineHandler = new EventPipelineHandler(cts);
            var handler = new EventHandler();

            var container = new Container(x =>
            {
                x.Add<IEventPipelineHandler<Event>>().Instance(pipelineHandler);
                x.Add<IEventHandler<Event>>().Instance(handler);
            });

            var mediator = new Mediator(type => container.GetService(type), Configuration);

            cts.Cancel();

            (await Should.ThrowAsync<Exception>(Exec(() => mediator.Publish(new Event(), cts.Token))))
                .Message.ShouldBe("oce");

            pipelineHandler.Executed.ShouldBeFalse();
            handler.Executed.ShouldBeFalse();
        }

        [Fact]
        public async Task ShouldStopProcessingIfCancelledAfterPipeline()
        {
            var cts = new CancellationTokenSource();
            var pipelineHandler = new EventPipelineHandler(cts);
            var handler = new EventHandler();

            var container = new Container(x =>
            {
                x.Add<IEventPipelineHandler<Event>>().Instance(pipelineHandler);
                x.Add<IEventHandler<Event>>().Instance(handler);
            });

            var mediator = new Mediator(type => container.GetService(type), Configuration);

            (await Should.ThrowAsync<Exception>(Exec(() => mediator.Publish(new Event(), cts.Token))))
                .Message.ShouldBe("oce");

            pipelineHandler.Executed.ShouldBeTrue();
            handler.Executed.ShouldBeFalse();
        }

        [Fact]
        public async Task ShouldStopProcessingIfCancelledDuringPipeline()
        {
            var cts = new CancellationTokenSource();
            var pipelineHandler1 = new EventPipelineHandler(cts);
            var pipelineHandler2 = new EventPipelineHandler(cts);
            var handler = new EventHandler();

            var container = new Container(x =>
            {
                x.Add<IEventPipelineHandler<Event>>().Instance(pipelineHandler1);
                x.Add<IEventPipelineHandler<Event>>().Instance(pipelineHandler2);
                x.Add<IEventHandler<Event>>().Instance(handler);
            });

            var mediator = new Mediator(type => container.GetService(type), Configuration);

            (await Should.ThrowAsync<Exception>(Exec(() => mediator.Publish(new Event(), cts.Token))))
                .Message.ShouldBe("oce");

            pipelineHandler1.Executed.ShouldBeTrue();
            pipelineHandler2.Executed.ShouldBeFalse();
            handler.Executed.ShouldBeFalse();
        }

        private static async Task Exec(Func<Task> f)
        {
            try
            {
                await f();
            }
            catch (OperationCanceledException exception)
            {
                throw new Exception("oce", exception);
            }
        }

        private class Event : IEvent { }

        private class EventPipelineHandler : IEventPipelineHandler<Event>
        {
            private readonly CancellationTokenSource _cancellationTokenSource;

            public EventPipelineHandler(CancellationTokenSource cancellationTokenSource)
            {
                _cancellationTokenSource = cancellationTokenSource;
            }

            public bool Executed { get; set; }

            public Task Handle(Event @event, CancellationToken cancellationToken, Func<Event, CancellationToken, Task> next)
            {
                _cancellationTokenSource.Cancel();
                Executed = true;
                return next(@event, cancellationToken);
            }
        }

        private class EventHandler : SyncEventHandler<Event>
        {

            public bool Executed { get; set; }

            protected override void Handle(Event @event)
            {
                Executed = true;
            }
        }
    }
}