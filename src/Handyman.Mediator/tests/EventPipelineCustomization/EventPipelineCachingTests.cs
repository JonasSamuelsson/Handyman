using System;
using System.Threading.Tasks;
using Handyman.Mediator.EventPipelineCustomization;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;

namespace Handyman.Mediator.Tests.EventPipelineCustomization
{
    public class EventPipelineCachingTests
    {
        [Fact]
        public async Task ShouldNotReusePipeline()
        {
            var mediator = new Mediator(new ServiceCollection().BuildServiceProvider());

            await mediator.Publish(new NonReusablePipelineEvent());
            await mediator.Publish(new NonReusablePipelineEvent());

            NonReusablePipelineAttribute.ExecutionCount.ShouldBe(2);
        }

        [NonReusablePipeline]
        private class NonReusablePipelineEvent : IEvent { }

        private class NonReusablePipelineAttribute : EventPipelineBuilderAttribute
        {
            public static int ExecutionCount { get; set; }

            public override bool PipelineCanBeReused => false;

            public override void Configure<TEvent>(IEventPipelineBuilder builder, IServiceProvider serviceProvider)
            {
                ExecutionCount++;
            }
        }

        [Fact]
        public async Task ShouldReusePipeline()
        {
            var mediator = new Mediator(new ServiceCollection().BuildServiceProvider());

            await mediator.Publish(new ReusablePipelineEvent());
            await mediator.Publish(new ReusablePipelineEvent());

            ReusablePipelineAttribute.ExecutionCount.ShouldBe(1);
        }

        [ReusablePipeline]
        private class ReusablePipelineEvent : IEvent { }

        private class ReusablePipelineAttribute : EventPipelineBuilderAttribute
        {
            public static int ExecutionCount { get; set; }

            public override bool PipelineCanBeReused => true;

            public override void Configure<TEvent>(IEventPipelineBuilder builder, IServiceProvider serviceProvider)
            {
                ExecutionCount++;
            }
        }
    }
}