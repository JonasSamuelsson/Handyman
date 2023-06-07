using Handyman.Mediator.Pipeline;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Handyman.Mediator.Tests.Pipeline
{
    public class RequestPipelineCustomizationTests
    {
        [Fact]
        public async Task ShouldConstructEntirePipelineUsingCustomBuilder()
        {
            var strings = new List<string>();

            var services = new ServiceCollection();

            // no filters or handlers are registered, they will be provided by the selectors
            services.AddSingleton<Action<string>>(s => strings.Add(s));

            var mediator = new Mediator(services.BuildServiceProvider());

            await mediator.Send(new Request());

            strings.ShouldBe(new[] { "filter", "execution strategy", "handler" });
        }

        [CustomizeRequestPipeline]
        private class Request : IRequest<Response>
        {
        }

        private class Response
        {
        }

        private class CustomizeRequestPipelineAttribute : RequestPipelineBuilderAttribute
        {
            public override Task Execute<TRequest, TResponse>(RequestPipelineBuilderContext<TRequest, TResponse> pipelineBuilderContext, RequestContext<TRequest> requestContext)
            {
                var serviceProvider = requestContext.ServiceProvider;

                pipelineBuilderContext.Filters.Add(new RequestFilter<TRequest, TResponse> { Action = serviceProvider.GetRequiredService<Action<string>>() });
                pipelineBuilderContext.Handlers.Add(new RequestHandler<TRequest, TResponse> { Action = serviceProvider.GetRequiredService<Action<string>>() });
                pipelineBuilderContext.HandlerExecutionStrategy = new RequestHandlerExecutionStrategy { Action = serviceProvider.GetRequiredService<Action<string>>() };

                return Task.CompletedTask;
            }
        }

        private class RequestFilter<TRequest, TResponse> : IRequestFilter<TRequest, TResponse>
            where TRequest : IRequest<TResponse>
        {
            public Action<string> Action { get; set; }

            public Task<TResponse> Execute(RequestContext<TRequest> requestContext, RequestFilterExecutionDelegate<TResponse> next)
            {
                Action.Invoke("filter");
                return next();
            }
        }

        private class RequestHandlerExecutionStrategy : IRequestHandlerExecutionStrategy
        {
            public Action<string> Action { get; set; }

            public Task<TResponse> Execute<TRequest, TResponse>(IRequestHandler<TRequest, TResponse> handler, RequestContext<TRequest> requestContext)
                where TRequest : IRequest<TResponse>
            {
                Action.Invoke("execution strategy");
                return handler.Handle(requestContext.Request, requestContext.CancellationToken);
            }
        }

        private class RequestHandler<TRequest, TResponse> : IRequestHandler<TRequest, TResponse> where TRequest : IRequest<TResponse>
        {
            public Action<string> Action { get; set; }

            public Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken)
            {
                Action.Invoke("handler");
                return Task.FromResult(default(TResponse));
            }
        }
    }
}