using Shouldly;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Handyman.Mediator.Tests
{
    public class RequestPipelineHandlerProviderTests
    {
        [Fact]
        public void ShouldProviderAnOrderedListOfHandlers()
        {
            var serviceProvider = new ServiceProvider(type => new IRequestPipelineHandler<Request, object>[]
            {
                new HandlerC(),
                new HandlerB(),
                new HandlerA()
            });

            var handlers = new RequestPipelineHandlerProvider()
                .GetHandlers<Request, object>(serviceProvider)
                .ToList();

            handlers[0].ShouldBeOfType<HandlerA>();
            handlers[1].ShouldBeOfType<HandlerB>();
            handlers[2].ShouldBeOfType<HandlerC>();
        }

        private class Request : IRequest<object> { }

        private class HandlerA : IOrderedPipelineHandler, IRequestPipelineHandler<Request, object>
        {
            public int Order => -1;

            public Task<object> Handle(Request request, CancellationToken cancellationToken, Func<Request, CancellationToken, Task<object>> next)
            {
                throw new NotImplementedException();
            }
        }

        private class HandlerB : IRequestPipelineHandler<Request, object>
        {
            public Task<object> Handle(Request request, CancellationToken cancellationToken, Func<Request, CancellationToken, Task<object>> next)
            {
                throw new NotImplementedException();
            }
        }

        private class HandlerC : IOrderedPipelineHandler, IRequestPipelineHandler<Request, object>
        {
            public int Order => 1;

            public Task<object> Handle(Request request, CancellationToken cancellationToken, Func<Request, CancellationToken, Task<object>> next)
            {
                throw new NotImplementedException();
            }
        }
    }
}