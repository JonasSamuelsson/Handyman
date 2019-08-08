﻿using Maestro;
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
            var container = new Container();

            container.Configure(x => x.Add<IRequestHandler<Request, int>>().Type<RequestHandler>());

            for (var i = 1; i <= 3; i++)
            {
                container.Configure(x => x.Add<IRequestFilter<Request, int>>().Type<RequestFilter>());

                // handler

                var mediatorWithoutPipeline = new Mediator(container.GetService, new Configuration
                {
                    RequestPipelineEnabled = false
                });

                (await mediatorWithoutPipeline.Send(new Request())).ShouldBe(0);

                // handler & Filter

                var mediatorWithPipeline = new Mediator(container.GetService, new Configuration
                {
                    RequestPipelineEnabled = true
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

        private class RequestFilter : IRequestFilter<Request, int>
        {
            public Task<int> Execute(RequestFilterContext<Request> context, Func<Task<int>> next)
            {
                context.Request.Number++;
                return next();
            }
        }
    }
}