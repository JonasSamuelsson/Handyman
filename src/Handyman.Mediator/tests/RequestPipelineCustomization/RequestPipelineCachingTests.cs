using System;
using System.Threading;
using System.Threading.Tasks;
using Handyman.Mediator.RequestPipelineCustomization;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;

namespace Handyman.Mediator.Tests.RequestPipelineCustomization
{
    public class RequestPipelineCachingTests
    {
        [Fact]
        public async Task ShouldNotReusePipeline()
        {
            var services = new ServiceCollection();

            services.AddTransient<IRequestHandler<NonReusablePipelineRequest, Void>, Handler>();

            var mediator = new Mediator(services.BuildServiceProvider());

            await mediator.Send(new NonReusablePipelineRequest());
            await mediator.Send(new NonReusablePipelineRequest());

            NonReusablePipelineBuilderAttribute.ExecutionCount.ShouldBe(2);
        }

        [Fact]
        public async Task ShouldReusePipeline()
        {
            var services = new ServiceCollection();

            services.AddTransient<IRequestHandler<ReusablePipelineRequest, Void>, Handler>();

            var mediator = new Mediator(services.BuildServiceProvider());

            await mediator.Send(new ReusablePipelineRequest());
            await mediator.Send(new ReusablePipelineRequest());

            ReusablePipelineBuilderAttribute.ExecutionCount.ShouldBe(1);
        }

        [NonReusablePipelineBuilder]
        private class NonReusablePipelineRequest : IRequest { }

        private class NonReusablePipelineBuilderAttribute : RequestPipelineBuilderAttribute
        {
            public static int ExecutionCount { get; set; }

            public override bool PipelineCanBeReused => false;

            public override void Configure<TRequest, TResponse>(IRequestPipelineBuilder<TRequest, TResponse> builder, IServiceProvider serviceProvider)
            {
                ExecutionCount++;
            }
        }

        [ReusablePipelineBuilder]
        private class ReusablePipelineRequest : IRequest { }

        private class ReusablePipelineBuilderAttribute : RequestPipelineBuilderAttribute
        {
            public static int ExecutionCount { get; set; }

            public override bool PipelineCanBeReused => true;

            public override void Configure<TRequest, TResponse>(IRequestPipelineBuilder<TRequest, TResponse> builder, IServiceProvider serviceProvider)
            {
                ExecutionCount++;
            }
        }

        private class Handler : IRequestHandler<NonReusablePipelineRequest, Void>, IRequestHandler<ReusablePipelineRequest, Void>
        {
            public Task<Void> Handle(NonReusablePipelineRequest request, CancellationToken cancellationToken)
            {
                return Task.FromResult(Void.Instance);
            }

            public Task<Void> Handle(ReusablePipelineRequest request, CancellationToken cancellationToken)
            {
                return Task.FromResult(Void.Instance);
            }
        }
    }
}