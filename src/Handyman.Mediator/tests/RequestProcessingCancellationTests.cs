using Maestro;
using Shouldly;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Handyman.Mediator.Tests
{
    public class RequestProcessingCancellationTests
    {
        private static readonly Configuration Configuration = new Configuration
        {
            RequestPipelineEnabled = true
        };

        [Fact]
        public async Task ShouldNotInvokeHandlerIfAlreadyCancelled()
        {
            var cts = new CancellationTokenSource();
            var handler = new RequestHandler();

            var container = new Container(x =>
            {
                x.Add<IRequestHandler<Request, Void>>().Instance(handler);
            });

            var mediator = new Mediator(type => container.GetService(type), Configuration);

            cts.Cancel();

            (await Should.ThrowAsync<Exception>(Exec(() => mediator.Send(new Request(), cts.Token))))
                .Message.ShouldBe("oce");

            handler.Executed.ShouldBeFalse();
        }

        [Fact]
        public async Task ShouldNotInvokeFiltersIfAlreadyCancelled()
        {
            var cts = new CancellationTokenSource();
            var filter = new RequestFilter(cts);
            var handler = new RequestHandler();

            var container = new Container(x =>
            {
                x.Add<IRequestFilter<Request, Void>>().Instance(filter);
                x.Add<IRequestHandler<Request, Void>>().Instance(handler);
            });

            var mediator = new Mediator(type => container.GetService(type), Configuration);

            cts.Cancel();

            (await Should.ThrowAsync<Exception>(Exec(() => mediator.Send(new Request(), cts.Token))))
                .Message.ShouldBe("oce");

            filter.Executed.ShouldBeFalse();
            handler.Executed.ShouldBeFalse();
        }

        [Fact]
        public async Task ShouldStopProcessingIfCancelledAfterFilters()
        {
            var cts = new CancellationTokenSource();
            var filter = new RequestFilter(cts);
            var handler = new RequestHandler();

            var container = new Container(x =>
            {
                x.Add<IRequestFilter<Request, Void>>().Instance(filter);
                x.Add<IRequestHandler<Request, Void>>().Instance(handler);
            });

            var mediator = new Mediator(type => container.GetService(type), Configuration);

            (await Should.ThrowAsync<Exception>(Exec(() => mediator.Send(new Request(), cts.Token))))
                .Message.ShouldBe("oce");

            filter.Executed.ShouldBeTrue();
            handler.Executed.ShouldBeFalse();
        }

        [Fact]
        public async Task ShouldStopProcessingIfCancelledDuringFilters()
        {
            var cts = new CancellationTokenSource();
            var filter1 = new RequestFilter(cts);
            var filter2 = new RequestFilter(cts);
            var handler = new RequestHandler();

            var container = new Container(x =>
            {
                x.Add<IRequestFilter<Request, Void>>().Instance(filter1);
                x.Add<IRequestFilter<Request, Void>>().Instance(filter2);
                x.Add<IRequestHandler<Request, Void>>().Instance(handler);
            });

            var mediator = new Mediator(type => container.GetService(type), Configuration);

            (await Should.ThrowAsync<Exception>(Exec(() => mediator.Send(new Request(), cts.Token))))
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

        private class Request : IRequest { }

        private class RequestFilter : IRequestFilter<Request, Void>
        {
            private readonly CancellationTokenSource _cancellationTokenSource;

            public RequestFilter(CancellationTokenSource cancellationTokenSource)
            {
                _cancellationTokenSource = cancellationTokenSource;
            }

            public bool Executed { get; set; }

            public Task<Void> Execute(Request request, CancellationToken cancellationToken, Func<Request, CancellationToken, Task<Void>> next)
            {
                _cancellationTokenSource.Cancel();
                Executed = true;
                return next(request, cancellationToken);
            }
        }

        private class RequestHandler : SyncRequestHandler<Request>
        {
            public bool Executed { get; set; }

            protected override void Handle(Request request, CancellationToken cancellationToken)
            {
                Executed = true;
            }
        }
    }
}
