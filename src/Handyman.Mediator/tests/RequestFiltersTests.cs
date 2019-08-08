using Handyman.Mediator.Internals;
using Maestro;
using Shouldly;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Handyman.Mediator.Tests
{
    public class RequestFiltersTests
    {
        [Fact]
        public async Task ShouldNotExecuteFiltersIfDisabled()
        {
            var container = new Container();
            var filter = new RequestFilter();
            var handler = new RequestHandler();
            container.Configure(x =>
            {
                x.Add<IRequestFilter<Request, Response>>().Instance(filter);
                x.Add<IRequestHandler<Request, Response>>().Instance(handler);
            });

            var mediator = new Mediator(container.GetService, new Configuration { RequestFilterProvider = RequestFiltersDisabled.Instance });
            var request = new Request();

            await mediator.Send(request, CancellationToken.None);

            filter.Executed.ShouldBeFalse();
            handler.Executed.ShouldBeTrue();
        }

        [Fact]
        public async Task ShouldUseFilters()
        {
            var container = new Container();
            var handler = new RequestHandler();
            var filter1 = new RequestFilter();
            var filter2 = new RequestFilter();
            container.Configure(x =>
            {
                x.Add<IRequestHandler<Request, Response>>().Instance(handler);
                x.Add<IRequestFilter<Request, Response>>().Factory(() => filter1);
                x.Add<IRequestFilter<Request, Response>>().Factory(() => filter2);
            });

            var mediator = new Mediator(container.GetService);
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

            public Task<Response> Execute(RequestFilterContext<Request> context,
                RequestFilterExecutionDelegate<Response> next)
            {
                Executed = true;
                return next();
            }
        }
    }
}