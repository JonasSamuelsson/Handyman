using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Handyman.Mediator.RequestPipelineCustomization;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;

namespace Handyman.Mediator.Tests.RequestPipelineCustomization
{
    public class RequestPipelineCustomizationTests
    {
        [Fact]
        public async Task ShouldGetRequestFilterViaAttribute()
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
        private class Request : IRequest<Response> { }

        private class Response { }

        private class CustomizeRequestPipelineAttribute : RequestPipelineBuilderAttribute
        {
            public override void Configure<TRequest, TResponse>(IRequestPipelineBuilder<TRequest, TResponse> builder, IServiceProvider serviceProvider)
            {
                builder.AddFilterSelector(new RequestFilterSelector<TRequest, TResponse>());
                builder.AddHandlerSelector(new RequestHandlerSelector<TRequest, TResponse>());
                builder.UseHandlerExecutionStrategy(new RequestHandlerExecutionStrategy<TRequest, TResponse> { Action = serviceProvider.GetRequiredService<Action<string>>() });
            }
        }

        private class RequestFilterSelector<TRequest, TResponse> : IRequestFilterSelector<TRequest, TResponse>
            where TRequest : IRequest<TResponse>
        {
            public Task SelectFilters(List<IRequestFilter<TRequest, TResponse>> filters, RequestPipelineContext<TRequest> context)
            {
                filters.Add(new RequestFilter<TRequest, TResponse> { Action = context.ServiceProvider.GetRequiredService<Action<string>>() });
                return Task.CompletedTask;
                ;
            }
        }

        private class RequestHandlerSelector<TRequest, TResponse> : IRequestHandlerSelector<TRequest, TResponse>
            where TRequest : IRequest<TResponse>
        {
            public Task SelectHandlers(List<IRequestHandler<TRequest, TResponse>> handlers, RequestPipelineContext<TRequest> context)
            {
                handlers.Add(new RequestHandler<TRequest, TResponse> { Action = context.ServiceProvider.GetRequiredService<Action<string>>() });
                return Task.CompletedTask;
            }
        }

        private class RequestFilter<TRequest, TResponse> : IRequestFilter<TRequest, TResponse>
            where TRequest : IRequest<TResponse>
        {
            public Action<string> Action { get; set; }

            public Task<TResponse> Execute(RequestPipelineContext<TRequest> context, RequestFilterExecutionDelegate<TResponse> next)
            {
                Action.Invoke("filter");
                return next();
            }
        }

        private class RequestHandlerExecutionStrategy<TRequest, TResponse> : IRequestHandlerExecutionStrategy<TRequest, TResponse>
            where TRequest : IRequest<TResponse>
        {
            public Action<string> Action { get; set; }

            public Task<TResponse> Execute(List<IRequestHandler<TRequest, TResponse>> handlers, RequestPipelineContext<TRequest> context)
            {
                Action.Invoke("execution strategy");
                return handlers.Single().Handle(context.Request, context.CancellationToken);
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