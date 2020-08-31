using Handyman.Mediator.Pipeline;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Handyman.Mediator.Tests
{
    public class RequestFiltersTests
    {
        [Fact]
        public async Task ShouldExecuteFilters()
        {
            var handler = new RequestHandler();
            var filter1 = new RequestFilter();
            var filter2 = new RequestFilter();

            var services = new ServiceCollection();

            services.AddSingleton<IRequestHandler<Request, Response>>(handler);
            services.AddSingleton<IRequestFilter<Request, Response>>(filter1);
            services.AddSingleton<IRequestFilter<Request, Response>>(filter2);

            var mediator = new Mediator(services.BuildServiceProvider());
            var request = new Request();

            await mediator.Send(request, CancellationToken.None);

            handler.Executed.ShouldBeTrue();
            filter1.Executed.ShouldBeTrue();
            filter2.Executed.ShouldBeTrue();
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

        private class RequestFilter : IRequestFilter<Request, Response>
        {
            public bool Executed { get; set; }

            public Task<Response> Execute(RequestContext<Request> requestContext, RequestFilterExecutionDelegate<Response> next)
            {
                Executed = true;
                return next();
            }
        }
    }
}