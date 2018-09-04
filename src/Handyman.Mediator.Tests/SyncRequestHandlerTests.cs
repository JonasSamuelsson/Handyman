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
        public async Task UseSynchRequestHandler()
        {
            var container = new Container();
            container.Configure(x => x.Add<IRequestHandler<Request, Response>>().Type<SynchRequestHandler>());
            var mediator = new Mediator(container.GetService);
            var request = new Request();
            (await mediator.Send(request, CancellationToken.None)).Value.ShouldBe("success");
        }

        private class SynchRequestHandler : SynchRequestHandler<Request, Response>
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