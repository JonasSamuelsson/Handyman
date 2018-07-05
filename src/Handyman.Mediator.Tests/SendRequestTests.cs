using Shouldly;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Handyman.Mediator.Tests
{
    public class SendRequestTests
    {
        [Fact]
        public async Task ShouldSendRequest()
        {
            var serviceProvider = new ServiceProvider();
            serviceProvider.Add<IRequestHandler<Request, Response>, RequestHandler>();
            var mediator = new Mediator(serviceProvider);
            var request = new Request();
            (await mediator.Send(request)).Request.ShouldBe(request);
        }

        class Request : IRequest<Response> { }

        class Response
        {
            public Request Request { get; set; }
        }

        class RequestHandler : IRequestHandler<Request, Response>
        {
            public Task<Response> Handle(Request request)
            {
                return Task.FromResult(new Response { Request = request });
            }
        }

        [Fact]
        public async Task UseSynchronousRequestHandler()
        {
            var serviceProvider = new ServiceProvider();
            serviceProvider.Add<IRequestHandler<Request, Response>, SynchronousRequestHandler>();
            var mediator = new Mediator(serviceProvider);
            var request = new Request();
            (await mediator.Send(request)).Request.ShouldBe(request);
        }

        class SynchronousRequestHandler : SynchronousRequestHandler<Request, Response>
        {
            protected override Response Handle(Request request)
            {
                return new Response { Request = request };
            }
        }

        [Fact]
        public async Task UseVoidRequestHandler()
        {
            var serviceProvider = new ServiceProvider();
            var handler = new VoidRequestHandler();
            serviceProvider.Add<IRequestHandler<VoidRequest, Void>>(() => handler);
            var mediator = new Mediator(serviceProvider);
            await mediator.Send(new VoidRequest());
            handler.Executed.ShouldBeTrue();
        }

        class VoidRequest : IRequest { }

        class VoidRequestHandler : VoidRequestHandler<VoidRequest>
        {
            public bool Executed { get; set; }

            protected override Task Handle(VoidRequest request)
            {
                Executed = true;
                return Task.CompletedTask;
            }
        }

        [Fact]
        public async Task UseSynchronousVoidRequestHandler()
        {
            var serviceProvider = new ServiceProvider();
            var handler = new SynchronousVoidRequestHandler();
            serviceProvider.Add<IRequestHandler<VoidRequest, Void>>(() => handler);
            var mediator = new Mediator(serviceProvider);
            await mediator.Send(new VoidRequest());
            handler.Executed.ShouldBeTrue();
        }

        class SynchronousVoidRequestHandler : SynchronousVoidRequestHandler<VoidRequest>
        {
            public bool Executed { get; set; }

            protected override void Handle(VoidRequest request)
            {
                Executed = true;
            }
        }

        [Fact]
        public async Task ShouldUseRequestPipelineHandlers()
        {
            var serviceProvider = new ServiceProvider();
            var pipelineHandler1 = new RequestPipelineHandler();
            var pipelineHandler2 = new RequestPipelineHandler();
            serviceProvider.Add<IRequestHandler<Request, Response>, RequestHandler>();
            serviceProvider.Add<IRequestPipelineHandler<Request, Response>>(() => pipelineHandler1);
            serviceProvider.Add<IRequestPipelineHandler<Request, Response>>(() => pipelineHandler2);
            var mediator = new Mediator(serviceProvider);
            var request = new Request();
            (await mediator.Send(request)).Request.ShouldBe(request);

            pipelineHandler1.Executed.ShouldBeTrue();
            pipelineHandler2.Executed.ShouldBeTrue();
        }

        class RequestPipelineHandler : IRequestPipelineHandler<Request, Response>
        {
            public bool Executed { get; set; }

            public Task<Response> Execute(Request request, Func<Request, Task<Response>> next)
            {
                Executed = true;
                return next.Invoke(request);
            }
        }

        [Fact]
        public async Task PipelineHandlersShouldNotBeUsedIfPipelineIsDisabled()
        {
            var serviceProvider = new ServiceProvider();
            var pipelineHandler = new RequestPipelineHandler();
            serviceProvider.Add<IRequestHandler<Request, Response>, RequestHandler>();
            serviceProvider.Add<IRequestPipelineHandler<Request, Response>>(() => pipelineHandler);
            var mediator = new Mediator(serviceProvider, new Configuration { UseRequestPipeline = false });
            var request = new Request();

            (await mediator.Send(request)).Request.ShouldBe(request);
            pipelineHandler.Executed.ShouldBeFalse();
        }
    }
}