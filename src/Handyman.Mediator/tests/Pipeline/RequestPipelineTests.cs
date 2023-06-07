using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Handyman.Mediator.Tests.Pipeline
{
    public class RequestPipelineTests
    {
        [Fact]
        public async Task ShouldBePossibleToRetryPartsOfThePipeline()
        {
            var executionCounterFilter1 = new ExecutionCounterFilter { Order = 0 };
            var executionCounterFilter2 = new ExecutionCounterFilter { Order = 2 };

            var serviceProviders = new ServiceCollection()
                .AddSingleton<IRequestFilter<Request, int>>(executionCounterFilter1)
                .AddSingleton<IRequestFilter<Request, int>>(executionCounterFilter2)
                .AddTransient<IRequestFilter<Request, int>, RetryFilter>()
                .AddTransient<IRequestHandler<Request, int>, FailThenSucceedHandler>()
                .BuildServiceProvider();

            var result = await new Mediator(serviceProviders).Send(new Request(), CancellationToken.None);

            executionCounterFilter1.Executions.ShouldBe(1);
            executionCounterFilter2.Executions.ShouldBe(2);
            result.ShouldBe(2);
        }

        public class Request : IRequest<int>
        {
        }

        public class ExecutionCounterFilter : IRequestFilter<Request, int>, IOrderedFilter
        {
            public int Executions { get; set; }
            public int Order { get; set; }

            public Task<int> Execute(RequestContext<Request> requestContext, RequestFilterExecutionDelegate<int> next)
            {
                Executions++;
                return next();
            }
        }

        public class RetryFilter : IRequestFilter<Request, int>, IOrderedFilter
        {
            public int Order => 1;

            public async Task<int> Execute(RequestContext<Request> requestContext, RequestFilterExecutionDelegate<int> next)
            {
                try
                {
                    return await next();
                }
                catch
                {
                    return await next();
                }
            }
        }

        public class FailThenSucceedHandler : IRequestHandler<Request, int>
        {
            private int _counter;

            public async Task<int> Handle(Request request, CancellationToken cancellationToken)
            {
                await Task.Yield();

                if (_counter++ == 0)
                {
                    throw new Exception();
                }

                return _counter;
            }
        }
    }
}