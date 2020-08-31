using Handyman.Mediator.Pipeline;
using Lamar;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Handyman.Mediator.Tests
{
    public class RequestFilterTests
    {
        [Fact]
        public async Task ShouldSupportFiltersWithCustomTypeConstraints()
        {
            // using Lamar for dependency injection as it has support for constrained open generics

            var testContext = new TestContext();

            var container = new Container(services =>
            {
                services.AddSingleton(testContext);
                services.AddTransient(typeof(IRequestFilter<,>), typeof(RequestFilter1<,>));
                services.AddTransient(typeof(IRequestFilter<,>), typeof(RequestFilter2<,>));
                services.AddTransient(typeof(IRequestFilter<,>), typeof(ResponseFilter1<,>));
                services.AddTransient(typeof(IRequestFilter<,>), typeof(ResponseFilter2<,>));
                services.AddTransient(typeof(IRequestHandler<,>), typeof(Handler<,>));
            });

            await new Mediator(container).Send(new Request());

            testContext.ExecutedFilters.ShouldBe(new[] { "RequestFilter1", "ResponseFilter1" }, ignoreOrder: true);
        }

        private interface IRequest1 { }

        private interface IRequest2 { }

        private class Request : IRequest<Response>, IRequest1 { }

        private interface IResponse1 { }

        private interface IResponse2 { }

        private class Response : IResponse1 { }

        private class TestContext
        {
            public List<string> ExecutedFilters { get; } = new List<string>();
        }

        private abstract class Filter<TRequest, TResponse> : IRequestFilter<TRequest, TResponse> where TRequest : IRequest<TResponse>
        {
            private readonly TestContext _testContext;

            protected Filter(TestContext testContext)
            {
                _testContext = testContext;
            }

            protected abstract string Text { get; }

            public Task<TResponse> Execute(RequestContext<TRequest> requestContext, RequestFilterExecutionDelegate<TResponse> next)
            {
                _testContext.ExecutedFilters.Add(Text);
                return next();
            }
        }

        private class RequestFilter1<TRequest, TResponse> : Filter<TRequest, TResponse>
            where TRequest : IRequest1, IRequest<TResponse>
        {
            public RequestFilter1(TestContext testContext) : base(testContext) { }

            protected override string Text => "RequestFilter1";
        }

        private class RequestFilter2<TRequest, TResponse> : Filter<TRequest, TResponse>
            where TRequest : IRequest2, IRequest<TResponse>
        {
            public RequestFilter2(TestContext testContext) : base(testContext) { }

            protected override string Text => "RequestFilter2";
        }

        private class ResponseFilter1<TRequest, TResponse> : Filter<TRequest, TResponse>
             where TRequest : IRequest<TResponse>
             where TResponse : IResponse1
        {
            public ResponseFilter1(TestContext testContext) : base(testContext) { }

            protected override string Text => "ResponseFilter1";
        }

        private class ResponseFilter2<TRequest, TResponse> : Filter<TRequest, TResponse>
            where TRequest : IRequest<TResponse>
            where TResponse : IResponse2
        {
            public ResponseFilter2(TestContext testContext) : base(testContext) { }

            protected override string Text => "ResponseFilter2";
        }

        private class Handler<TRequest, TResponse> : IRequestHandler<TRequest, TResponse>
            where TRequest : IRequest<TResponse>
        {
            public Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken)
            {
                return Task.FromResult(default(TResponse));
            }
        }
    }
}