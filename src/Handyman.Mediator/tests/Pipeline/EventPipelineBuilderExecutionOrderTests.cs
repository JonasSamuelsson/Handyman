using Handyman.Mediator.Pipeline;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Handyman.Mediator.Tests.Pipeline
{
    public class EventPipelineBuilderExecutionOrderTests
    {
        [Fact]
        public async Task ShouldExecuteCustomizationsInSpecifiedOrder()
        {
            var strings = new List<string>();
            var services = new ServiceCollection();

            services.AddSingleton(strings);

            await new Mediator(services.BuildServiceProvider()).Publish(new MyEvent());

            strings.ShouldBe(new[] { "first", "middle", "last" });
        }

        [PipelineBuilder(ExecutionOrder = MediatorDefaults.Order.Last, Text = "last")]
        [PipelineBuilder(ExecutionOrder = MediatorDefaults.Order.First, Text = "first")]
        [PipelineBuilder(Text = "middle")]
        private class MyEvent : IEvent { }

        private class PipelineBuilderAttribute : EventPipelineBuilderAttribute
        {
            public string Text { get; set; }

            public override Task Execute<TEvent>(EventPipelineBuilderContext<TEvent> pipelineBuilderContext, EventContext<TEvent> eventContext)
            {
                eventContext.ServiceProvider.GetRequiredService<List<string>>().Add(Text);
                return Task.CompletedTask;
            }
        }
    }
}