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
        public async Task ShouldNotInvokePipelineHandlerIfAlreadyCancelled()
        {
            var cts = new CancellationTokenSource();
            var pipelineHandler = new RequestPipelineHandler(cts);
            var handler = new RequestHandler();

            var container = new Container(x =>
            {
                x.Add<IRequestPipelineHandler<Request, Void>>().Instance(pipelineHandler);
                x.Add<IRequestHandler<Request, Void>>().Instance(handler);
            });

            var mediator = new Mediator(type => container.GetService(type), Configuration);

            cts.Cancel();

            (await Should.ThrowAsync<Exception>(Exec(() => mediator.Send(new Request(), cts.Token))))
                .Message.ShouldBe("oce");

            pipelineHandler.Executed.ShouldBeFalse();
            handler.Executed.ShouldBeFalse();
        }

        [Fact]
        public async Task ShouldStopProcessingIfCancelledAfterPipeline()
        {
            var cts = new CancellationTokenSource();
            var pipelineHandler = new RequestPipelineHandler(cts);
            var handler = new RequestHandler();

            var container = new Container(x =>
            {
                x.Add<IRequestPipelineHandler<Request, Void>>().Instance(pipelineHandler);
                x.Add<IRequestHandler<Request, Void>>().Instance(handler);
            });

            var mediator = new Mediator(type => container.GetService(type), Configuration);

            (await Should.ThrowAsync<Exception>(Exec(() => mediator.Send(new Request(), cts.Token))))
                .Message.ShouldBe("oce");

            pipelineHandler.Executed.ShouldBeTrue();
            handler.Executed.ShouldBeFalse();
        }

        [Fact]
        public async Task ShouldStopProcessingIfCancelledDuringPipeline()
        {
            var cts = new CancellationTokenSource();
            var pipelineHandler1 = new RequestPipelineHandler(cts);
            var pipelineHandler2 = new RequestPipelineHandler(cts);
            var handler = new RequestHandler();

            var container = new Container(x =>
            {
                x.Add<IRequestPipelineHandler<Request, Void>>().Instance(pipelineHandler1);
                x.Add<IRequestPipelineHandler<Request, Void>>().Instance(pipelineHandler2);
                x.Add<IRequestHandler<Request, Void>>().Instance(handler);
            });

            var mediator = new Mediator(type => container.GetService(type), Configuration);

            (await Should.ThrowAsync<Exception>(Exec(() => mediator.Send(new Request(), cts.Token))))
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

        private class Request : IRequest { }

        private class RequestPipelineHandler : IRequestPipelineHandler<Request, Void>
        {
            private readonly CancellationTokenSource _cancellationTokenSource;

            public RequestPipelineHandler(CancellationTokenSource cancellationTokenSource)
            {
                _cancellationTokenSource = cancellationTokenSource;
            }

            public bool Executed { get; set; }

            public Task<Void> Handle(Request request, CancellationToken cancellationToken, Func<Request, CancellationToken, Task<Void>> next)
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
