using System.Threading;
using System.Threading.Tasks;
using Maestro;
using Shouldly;
using Xunit;

namespace Handyman.Mediator.Tests
{
    public class RequestFilterOrderingTests
    {
        [Fact]
        public async Task ShouldExecuteFiltersInTheCorrectOrder()
        {
            var container = new Container(x =>
            {
                x.Add<IRequestFilter<Request, string>>().Instance(new Filter { Text = "b" });
                x.Add<IRequestFilter<Request, string>>().Instance(new Filter { Order = 1, Text = "c" });
                x.Add<IRequestFilter<Request, string>>().Instance(new Filter { Order = -1, Text = "a" });
                x.Add<IRequestHandler<Request, string>>().Type<Handler>();
            });

            var s = await new Mediator(container.GetService)
                .Send(new Request());

            s.ShouldBe("abc");
        }

        private class Request : IRequest<string>
        {
            public string Text { get; set; }
        }

        private class Filter : IOrderedFilter, IRequestFilter<Request, string>
        {
            public int Order { get; set; }
            public string Text { get; set; }

            public Task<string> Execute(RequestPipelineContext<Request> context,
                RequestFilterExecutionDelegate<string> next)
            {
                context.Request.Text += Text;
                return next();
            }
        }

        private class Handler : IRequestHandler<Request, string>
        {
            public Task<string> Handle(Request request, CancellationToken cancellationToken)
            {
                return Task.FromResult(request.Text);
            }
        }
    }
}