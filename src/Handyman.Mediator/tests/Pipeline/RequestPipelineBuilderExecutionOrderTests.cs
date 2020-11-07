using Handyman.Mediator.Pipeline;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Handyman.Mediator.Tests.Pipeline
{
    public class RequestPipelineBuilderExecutionOrderTests
    {
        [Fact]
        public async Task ShouldExecutePipelineBuildersInSpecifiedOrder()
        {
            var strings = new List<string>();

            var services = new ServiceCollection()
                .AddSingleton(strings)
                .AddTransient<IRequestHandler<Request, Void>, Handler>()
                .BuildServiceProvider();

            await new Mediator(services).Send(new Request());

            strings.ShouldBe(new[] { "first", "middle", "last" });
        }

        [PipelineBuilder(Text = "middle")]
        [PipelineBuilder(Order = MediatorDefaults.Order.Last, Text = "last")]
        [PipelineBuilder(Order = MediatorDefaults.Order.First, Text = "first")]
        private class Request : IRequest { }

        private class Handler : IRequestHandler<Request, Void>
        {
            public Task<Void> Handle(Request request, CancellationToken cancellationToken)
            {
                return Task.FromResult(Void.Instance);
            }
        }

        private class PipelineBuilderAttribute : RequestPipelineBuilderAttribute
        {
            public string Text { get; set; }

            public override Task Execute<TRequest, TResponse>(RequestPipelineBuilderContext<TRequest, TResponse> pipelineBuilderContext, RequestContext<TRequest> requestContext)
            {
                requestContext.ServiceProvider.GetRequiredService<List<string>>().Add(Text);
                return Task.CompletedTask;
            }
        }
    }
}