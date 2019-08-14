using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Maestro;
using Shouldly;
using Xunit;

namespace Handyman.Mediator.Tests
{
    public class RequestFilterProviderAttributeTests
    {
        [Fact]
        public async Task ShouldGetRequestFilterViaAttribute()
        {
            var container = new Container(x => x.Add<IRequestHandler<Request, string>>().Type<RequestHandler>());

            var mediator = new Mediator(container.GetService);

            (await mediator.Send(new Request())).ShouldBe("success");
        }

        [CustomRequestFilterProvider]
        private class Request : IRequest<string> { }

        private class CustomRequestFilterProviderAttribute : RequestFilterProviderAttribute
        {
            public override IEnumerable<IRequestFilter<TRequest, TResponse>> GetFilters<TRequest, TResponse>(ServiceProvider serviceProvider)
            {
                return new[] { new RequestFilter<TRequest, TResponse>() };
            }
        }

        private class RequestFilter<TRequest, TResponse> : IRequestFilter<TRequest, TResponse> where TRequest : IRequest<TResponse>
        {
            public Task<TResponse> Execute(RequestFilterContext<TRequest> context, RequestFilterExecutionDelegate<TResponse> next)
            {
                return Task.FromResult((TResponse)(object)"success");
            }
        }

        private class RequestHandler : IRequestHandler<Request, string>
        {
            public Task<string> Handle(Request request, CancellationToken cancellationToken)
            {
                throw new NotImplementedException();
            }
        }
    }
}