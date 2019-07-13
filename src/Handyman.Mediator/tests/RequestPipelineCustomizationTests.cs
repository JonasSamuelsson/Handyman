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
                x.Add<IRequestPipelineHandler<Request, string[]>>().Instance(new RequestPipelineHandler { String = "1" });
                x.Add<IRequestPipelineHandler<Request, string[]>>().Instance(new RequestPipelineHandler { String = "2" });
                x.Add<IRequestPipelineHandler<Request, string[]>>().Instance(new RequestPipelineHandler { String = "3" });
            });

            var provider = new RequestPipelineHandlerProvider { Comparison = (a, b) => string.Compare(a.String, b.String, StringComparison.Ordinal) };
            var configuration = new Configuration { RequestPipelineHandlerProvider = provider };

            var s1 = await new Mediator(container.GetService, configuration).Send(new Request());
            s1.ShouldBe(new[] { "3", "2", "1" });

            provider.Comparison = (a, b) => string.Compare(b.String, a.String, StringComparison.Ordinal);

            var s2 = await new Mediator(container.GetService, configuration).Send(new Request());
            s2.ShouldBe(new[] { "1", "2", "3" });
        }

        private class RequestPipelineHandlerProvider : IRequestPipelineHandlerProvider
        {
            public Comparison<RequestPipelineHandler> Comparison { get; set; }

            public IEnumerable<IRequestPipelineHandler<TRequest, TResponse>> GetHandlers<TRequest, TResponse>(ServiceProvider serviceProvider) where TRequest : IRequest<TResponse>
            {
                var type = typeof(IEnumerable<IRequestPipelineHandler<TRequest, TResponse>>);
                var handlers = ((IEnumerable)serviceProvider.Invoke(type)).OfType<RequestPipelineHandler>().ToList();
                handlers.Sort(Comparison);
                return handlers.OfType<IRequestPipelineHandler<TRequest, TResponse>>();
            }
        }

        private class Request : IRequest<string[]> { }

        private class RequestHandler : SyncRequestHandler<Request, string[]>
        {
            protected override string[] Handle(Request request, CancellationToken cancellationToken)
            {
                return new string[0];
            }
        }

        private class RequestPipelineHandler : IRequestPipelineHandler<Request, string[]>
        {
            public string String { get; set; }

            public async Task<string[]> Handle(Request request, CancellationToken cancellationToken, Func<Request, CancellationToken, Task<string[]>> next)
            {
                return (await next(request, cancellationToken)).Append(String).ToArray();
            }
        }
    }
}
