using Maestro;
using Shouldly;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Handyman.Mediator.Tests
{
    public class RequestHandlerCustomizationTests
    {
        [Fact]
        public async Task ShouldCustomizeRequestHandling()
        {
            var container = new Container(x =>
            {
                x.Add<IRequestHandler<Request, string>>().Type<RequestHandler>();
                x.Add<IRequestHandler<Request, string>>().Type<RequestHandler>();
            });

            var provider = new RequestHandlerProvider();

            var s = await new Mediator(container.GetService, new Configuration { RequestHandlerProvider = provider }).Send(new Request());

            s.ShouldBe("success success");
        }

        private class Request : IRequest<string> { }

        private class RequestHandler : IRequestHandler<Request, string>
        {
            public Task<string> Handle(Request request, CancellationToken cancellationToken)
            {
                return Task.FromResult("success");
            }
        }

        private class RequestHandlerProvider : IRequestHandlerProvider
        {
            public IRequestHandler<TRequest, TResponse> GetHandler<TRequest, TResponse>(ServiceProvider serviceProvider) where TRequest : IRequest<TResponse>
            {
                var type = typeof(IEnumerable<IRequestHandler<TRequest, TResponse>>);
                var handlers = ((IEnumerable)serviceProvider.Invoke(type)).OfType<RequestHandler>();
                return (IRequestHandler<TRequest, TResponse>)new CustomRequestHandler(handlers);
            }
        }

        private class CustomRequestHandler : IRequestHandler<Request, string>
        {
            private readonly IEnumerable<RequestHandler> _handlers;

            public CustomRequestHandler(IEnumerable<RequestHandler> handlers)
            {
                _handlers = handlers;
            }

            public async Task<string> Handle(Request request, CancellationToken cancellationToken)
            {
                var tasks = _handlers.Select(x => x.Handle(request, cancellationToken));
                var strings = await Task.WhenAll(tasks);
                return string.Join(" ", strings);
            }
        }
    }
}