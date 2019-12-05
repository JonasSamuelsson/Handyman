using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;

namespace Handyman.Mediator.Tests
{
    public class SendRequestTests
    {
        [Fact]
        public async Task ShouldSendRequest()
        {
            var services = new ServiceCollection().AddTransient<IRequestHandler<Request, Response>, RequestHandler>();
            var mediator = new Mediator(services.BuildServiceProvider());
            (await mediator.Send(new Request(), CancellationToken.None)).Value.ShouldBe("success");
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