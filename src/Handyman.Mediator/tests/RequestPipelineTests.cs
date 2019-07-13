using Maestro;
using Shouldly;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Handyman.Mediator.Tests
{
    public class RequestPipelineTests
    {
        [Fact]
        public async Task ShouldNotExecutePipelineHandlersIfPipelineIsDisabled()
        {
            var container = new Container();
            var handler = new RequestHandler();
            var pipelineHandler = new RequestPipelineHandler();
            container.Configure(x =>
            {
                x.Add<IRequestHandler<Request, Response>>().Instance(handler);
                x.Add<IRequestPipelineHandler<Request, Response>>().Instance(pipelineHandler);
            });

            var mediator = new Mediator(container.GetService, new Configuration { RequestPipelineEnabled = false });
            var request = new Request();

            await mediator.Send(request, CancellationToken.None);

            handler.Executed.ShouldBeTrue();
            pipelineHandler.Executed.ShouldBeFalse();
        }

        [Fact]
        public async Task ShouldUseRequestPipelineHandlers()
        {
            var container = new Container();
            var handler = new RequestHandler();
            var pipelineHandler1 = new RequestPipelineHandler();
            var pipelineHandler2 = new RequestPipelineHandler();
            container.Configure(x =>
            {
                x.Add<IRequestHandler<Request, Response>>().Instance(handler);
                x.Add<IRequestPipelineHandler<Request, Response>>().Factory(() => pipelineHandler1);
                x.Add<IRequestPipelineHandler<Request, Response>>().Factory(() => pipelineHandler2);
            });

            var mediator = new Mediator(container.GetService, new Configuration { RequestPipelineEnabled = true });
            var request = new Request();

            await mediator.Send(request, CancellationToken.None);

            handler.Executed.ShouldBeTrue();
            pipelineHandler1.Executed.ShouldBeTrue();
            pipelineHandler2.Executed.ShouldBeTrue();
        }

        private class Request : IRequest<Response> { }

        private class Response { }

        private class RequestHandler : IRequestHandler<Request, Response>
        {
            public bool Executed { get; set; }

            public Task<Response> Handle(Request request, CancellationToken cancellationToken)
            {
                Executed = true;
                return Task.FromResult(new Response());
            }
        }

        private class RequestPipelineHandler : IRequestPipelineHandler<Request, Response>
        {
            public bool Executed { get; set; }

            public Task<Response> Handle(Request request, CancellationToken cancellationToken, Func<Request, CancellationToken, Task<Response>> next)
            {
                Executed = true;
                return next.Invoke(request, cancellationToken);
            }
        }
    }
}