using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Handyman.Mediator.Tests.Pipeline
{
    public class RequestPipelineTests
    {
        [Fact]
        public async Task ShouldBePossibleToReRunPartsOfThePipeline()
        {
            var filter = new Filter();

            var serviceProviders = new ServiceCollection()
                .AddTransient<IRequestFilter<Request, Void>, ReRunFilter>()
                .AddSingleton<IRequestFilter<Request, Void>>(filter)
                .AddTransient<IRequestHandler<Request, Void>, Handler>()
                .BuildServiceProvider();

            await new Mediator(serviceProviders).Send(new Request(), CancellationToken.None);

            filter.Executions.ShouldBe(2);
        }

        public class Request : IRequest
        {
        }

        public class ReRunFilter : IRequestFilter<Request, Void>, IOrderedFilter
        {
            public int Order => 0;

            public async Task<Void> Execute(RequestContext<Request> requestContext, RequestFilterExecutionDelegate<Void> next)
            {
                await next();
                return await next();
            }
        }

        public class Filter : IRequestFilter<Request, Void>, IOrderedFilter
        {
            public int Executions { get; set; }
            public int Order => 1;

            public Task<Void> Execute(RequestContext<Request> requestContext, RequestFilterExecutionDelegate<Void> next)
            {
                Executions++;
                return next();
            }
        }

        public class Handler : IRequestHandler<Request, Void>
        {
            public Task<Void> Handle(Request request, CancellationToken cancellationToken)
            {
                return Task.FromResult(Void.Instance);
            }
        }
    }
}