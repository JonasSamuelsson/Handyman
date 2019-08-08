using Maestro;
using Shouldly;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Handyman.Mediator.Tests
{
    public class RequestPipelineCustomizationTests
    {
        [Fact]
        public async Task ShouldCustomizeRequestPipeline()
        {
            var container = new Container(x =>
            {
                x.Add<IRequestHandler<Request, string[]>>().Instance(new RequestHandler());
                x.Add<IRequestFilter<Request, string[]>>().Instance(new RequestFilter { String = "1" });
                x.Add<IRequestFilter<Request, string[]>>().Instance(new RequestFilter { String = "2" });
                x.Add<IRequestFilter<Request, string[]>>().Instance(new RequestFilter { String = "3" });
            });

            var provider = new RequestFilterProvider { Comparison = (a, b) => string.Compare(a.String, b.String, StringComparison.Ordinal) };
            var configuration = new Configuration { RequestFilterProvider = provider };

            var s1 = await new Mediator(container.GetService, configuration).Send(new Request());
            s1.ShouldBe(new[] { "3", "2", "1" });

            provider.Comparison = (a, b) => string.Compare(b.String, a.String, StringComparison.Ordinal);

            var s2 = await new Mediator(container.GetService, configuration).Send(new Request());
            s2.ShouldBe(new[] { "1", "2", "3" });
        }

        private class RequestFilterProvider : IRequestFilterProvider
        {
            public Comparison<RequestFilter> Comparison { get; set; }

            public IEnumerable<IRequestFilter<TRequest, TResponse>> GetFilters<TRequest, TResponse>(ServiceProvider serviceProvider) where TRequest : IRequest<TResponse>
            {
                var type = typeof(IEnumerable<IRequestFilter<TRequest, TResponse>>);
                var handlers = ((IEnumerable)serviceProvider.Invoke(type)).OfType<RequestFilter>().ToList();
                handlers.Sort(Comparison);
                return handlers.OfType<IRequestFilter<TRequest, TResponse>>();
            }
        }

        private class Request : IRequest<string[]> { }

        private class RequestHandler : RequestHandler<Request, string[]>
        {
            protected override string[] Handle(Request request, CancellationToken cancellationToken)
            {
                return new string[0];
            }
        }

        private class RequestFilter : IRequestFilter<Request, string[]>
        {
            public string String { get; set; }

            public async Task<string[]> Execute(RequestFilterContext<Request> context,
                RequestFilterExecutionDelegate<string[]> next)
            {
                return (await next()).Append(String).ToArray();
            }
        }
    }
}
