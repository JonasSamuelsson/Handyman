using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Handyman.Mediator.Tests
{
    public class EventProcessingCancellationTests
    {
        [Fact]
        public async Task ShouldNotInvokeHandlerIfAlreadyCancelled()
        {
            var cts = new CancellationTokenSource();
            var handler = new EventHandler();

            var services = new ServiceCollection();

            services.AddSingleton<IEventHandler<Event>>(handler);

            var mediator = new Mediator(services.BuildServiceProvider());

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

            var services = new ServiceCollection();

            services.AddSingleton<IEventFilter<Event>>(filter);
            services.AddSingleton<IEventHandler<Event>>(handler);

            var mediator = new Mediator(services.BuildServiceProvider());

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

            var services = new ServiceCollection();

            services.AddSingleton<IEventFilter<Event>>(filter);
            services.AddSingleton<IEventHandler<Event>>(handler);

            var mediator = new Mediator(services.BuildServiceProvider());

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

            var services = new ServiceCollection();

            services.AddSingleton<IEventFilter<Event>>(filter1);
            services.AddSingleton<IEventFilter<Event>>(filter2);
            services.AddSingleton<IEventHandler<Event>>(handler);

            var mediator = new Mediator(services.BuildServiceProvider());

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

            public Task Execute(EventContext<Event> eventContext, EventFilterExecutionDelegate next)
            {
                _cancellationTokenSource.Cancel();
                Executed = true;
                return next();
            }
        }

        private class EventHandler : SyncEventHandler<Event>
        {
            public bool Executed { get; set; }

            protected override void Handle(Event @event, CancellationToken cancellationToken)
            {
                Executed = true;
            }
        }
    }
}