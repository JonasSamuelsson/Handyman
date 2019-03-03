using System.Threading;
using System.Threading.Tasks;
using Maestro;
using Shouldly;
using Xunit;

namespace Handyman.Mediator.Tests
{
    public class SyncRequestHandlerTests
    {
        [Fact]
        public async Task UseSyncRequestHandlerOf1()
        {
            var handler = new SyncRequestHandlerOf1();
            var container = new Container(x => x.Add<IRequestHandler<VoidRequest, Void>>().Instance(handler));
            var mediator = new Mediator(container.GetService);
            await mediator.Send(new VoidRequest(), CancellationToken.None);
            handler.Executed.ShouldBeTrue();
        }

        private class VoidRequest : IRequest { }

        private class SyncRequestHandlerOf1 : SyncRequestHandler<VoidRequest>
        {
            public bool Executed { get; set; }

            protected override void Handle(VoidRequest request, CancellationToken cancellationToken)
            {
                Executed = true;
            }
        }
 
        [Fact]
        public async Task UseSynchRequestHandlerOf2()
        {
            var container = new Container();
            container.Configure(x => x.Add<IRequestHandler<Request, Response>>().Type<SyncRequestHandlerOf2>());
            var mediator = new Mediator(container.GetService);
            var request = new Request();
            (await mediator.Send(request, CancellationToken.None)).Value.ShouldBe("success");
        }

        private class SyncRequestHandlerOf2 : SyncRequestHandler<Request, Response>
        {
            protected override Response Handle(Request request, CancellationToken cancellationToken)
            {
                return new Response { Value = "success" };
            }
        }

        private class Request : IRequest<Response> { }

        private class Response
        {
            public string Value { get; set; }
        }
   }
}