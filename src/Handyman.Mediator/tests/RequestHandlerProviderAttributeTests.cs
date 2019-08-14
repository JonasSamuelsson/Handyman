using Maestro;
using Shouldly;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Handyman.Mediator.Tests
{
    public class RequestHandlerProviderAttributeTests
    {
        [Fact]
        public async Task ShouldGetRequestHandlerViaAttribute()
        {
            var container = new Container();

            var mediator = new Mediator(container.GetService);

            (await mediator.Send(new Request())).ShouldBe("success");
        }

        [CustomRequestHandlerProvider]
        private class Request : IRequest<string> { }

        private class CustomRequestHandlerProviderAttribute : RequestHandlerProviderAttribute
        {
            public override IRequestHandler<TRequest, TResponse> GetHandler<TRequest, TResponse>(ServiceProvider serviceProvider)
            {
                return new RequestHandler<TRequest, TResponse>();
            }
        }

        private class RequestHandler<TRequest, TResponse> : IRequestHandler<TRequest, TResponse> where TRequest : IRequest<TResponse>
        {
            public Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken)
            {
                return Task.FromResult((TResponse)(object)"success");
            }
        }
    }
}
