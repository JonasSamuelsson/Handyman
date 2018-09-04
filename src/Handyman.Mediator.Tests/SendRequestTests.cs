using Maestro;
using Shouldly;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Handyman.Mediator.Tests
{
    public class SendRequestTests
    {
        [Fact]
        public async Task ShouldSendRequest()
        {
            var container = new Container(x => x.Add<IRequestHandler<Request, Response>>().Type<RequestHandler>());
            (await new Mediator(container.GetService).Send(new Request(), CancellationToken.None)).Value.ShouldBe("success");
        }

        private class Request : IRequest<Response> { }

        private class Response
        {
            public string Value { get; set; }
        }

        private class RequestHandler : IRequestHandler<Request, Response>
        {
            public Task<Response> Handle(Request request, CancellationToken cancellationToken)
            {
                return Task.FromResult(new Response { Value = "success" });
            }
        }
    }
}