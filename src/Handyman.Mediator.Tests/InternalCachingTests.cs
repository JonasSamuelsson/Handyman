using Shouldly;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Handyman.Mediator.Tests
{
    public class InternalCachingTests
    {
        [Fact]
        public async Task ShouldJustWork()
        {
            var serviceProvider = new TestServiceProvider();

            serviceProvider.Add<IRequestHandler<Request, int>, RequestHandler>();

            for (var i = 1; i <= 3; i++)
            {
                serviceProvider.Add<IRequestPipelineHandler<Request, int>, RequestPipelineHandler>();

                // handler

                var mediatorWithoutPipeline = new Mediator(new Configuration
                {
                    RequestPipelineEnabled = false,
                    ServiceProvider = serviceProvider
                });

                (await mediatorWithoutPipeline.Send(new Request())).ShouldBe(0);

                // handler & pipeline

                var mediatorWithPipeline = new Mediator(new Configuration
                {
                    RequestPipelineEnabled = true,
                    ServiceProvider = serviceProvider
                });

                (await mediatorWithPipeline.Send(new Request())).ShouldBe(i);
            }
        }

        private class Request : IRequest<int>
        {
            public int Number { get; set; }
        }

        private class RequestHandler : IRequestHandler<Request, int>
        {
            public Task<int> Handle(Request request, CancellationToken cancellationToken)
            {
                return Task.FromResult(request.Number);
            }
        }

        private class RequestPipelineHandler : IRequestPipelineHandler<Request, int>
        {
            public Task<int> Execute(Request request, CancellationToken cancellationToken, Func<Request, CancellationToken, Task<int>> next)
            {
                request.Number++;
                return next(request, cancellationToken);
            }
        }
    }
}