using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;

namespace Handyman.Mediator.Tests
{
    public class RequestFilterOrderingTests
    {
        [Fact]
        public async Task ShouldExecuteFiltersInTheCorrectOrder()
        {
            var services = new ServiceCollection();

            services.AddSingleton<IRequestFilter<Request, string>>(new Filter { Text = "b" });
            services.AddSingleton<IRequestFilter<Request, string>>(new Filter { Order = 1, Text = "c" });
            services.AddSingleton<IRequestFilter<Request, string>>(new Filter { Order = -1, Text = "a" });
            services.AddTransient<IRequestHandler<Request, string>, Handler>();

            var s = await new Mediator(services.BuildServiceProvider())
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