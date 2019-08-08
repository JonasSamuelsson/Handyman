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
        public async Task ShouldNotInvokeFilterIfAlreadyCancelled()
        {
            var cts = new CancellationTokenSource();
            var filter = new EventFilter(cts);
            var handler = new EventHandler();

            var container = new Container(x =>
            {
                x.Add<IEventFilter<Event>>().Instance(filter);
                x.Add<IEventHandler<Event>>().Instance(handler);
            });

            var mediator = new Mediator(type => container.GetService(type), Configuration);

            cts.Cancel();

            (await Should.ThrowAsync<Exception>(Exec(() => mediator.Publish(new Event(), cts.Token))))
                .Message.ShouldBe("oce");

            filter.Executed.ShouldBeFalse();
            handler.Executed.ShouldBeFalse();
        }

        [Fact]
        public async Task ShouldStopProcessingIfCancelledAfterFilters()
        {
            var cts = new CancellationTokenSource();
            var filter = new EventFilter(cts);
            var handler = new EventHandler();

            var container = new Container(x =>
            {
                x.Add<IEventFilter<Event>>().Instance(filter);
                x.Add<IEventHandler<Event>>().Instance(handler);
            });

            var mediator = new Mediator(type => container.GetService(type), Configuration);

            (await Should.ThrowAsync<Exception>(Exec(() => mediator.Publish(new Event(), cts.Token))))
                .Message.ShouldBe("oce");

            filter.Executed.ShouldBeTrue();
            handler.Executed.ShouldBeFalse();
        }

        [Fact]
        public async Task ShouldStopProcessingIfCancelledDuringFilters()
        {
            var cts = new CancellationTokenSource();
            var filter1 = new EventFilter(cts);
            var filter2 = new EventFilter(cts);
            var handler = new EventHandler();

            var container = new Container(x =>
            {
                x.Add<IEventFilter<Event>>().Instance(filter1);
                x.Add<IEventFilter<Event>>().Instance(filter2);
                x.Add<IEventHandler<Event>>().Instance(handler);
            });

            var mediator = new Mediator(type => container.GetService(type), Configuration);

            (await Should.ThrowAsync<Exception>(Exec(() => mediator.Publish(new Event(), cts.Token))))
                .Message.ShouldBe("oce");

            filter1.Executed.ShouldBeTrue();
            filter2.Executed.ShouldBeFalse();
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

        private class EventFilter : IEventFilter<Event>
        {
            private readonly CancellationTokenSource _cancellationTokenSource;

            public EventFilter(CancellationTokenSource cancellationTokenSource)
            {
                _cancellationTokenSource = cancellationTokenSource;
            }

            public bool Executed { get; set; }

            public Task Execute(Event @event, CancellationToken cancellationToken, Func<Event, CancellationToken, Task> next)
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