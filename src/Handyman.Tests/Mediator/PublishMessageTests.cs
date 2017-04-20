using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Handyman.Mediator;
using Shouldly;
using Xunit;

namespace Handyman.Tests.Mediator
{
    public class PublishMessageTests
    {
        [Fact]
        public async Task ShouldPublishMessage()
        {
            var message = new TestMessage();
            var serviceProvider = new ServiceProvider(typeof(IMessageHandler<TestMessage>), typeof(TestMessageHandler1), typeof(TestMessageHandler2));
            var mediator = new Handyman.Mediator.Mediator(serviceProvider.GetService, serviceProvider.GetServices);

            await Task.WhenAll(mediator.Publish(message));

            message.HandlerTypes.Count.ShouldBe(2);
            message.HandlerTypes.ShouldContain(typeof(TestMessageHandler1));
            message.HandlerTypes.ShouldContain(typeof(TestMessageHandler2));
        }

        class TestMessage : IMessage
        {
            public ConcurrentBag<Type> HandlerTypes { get; } = new ConcurrentBag<Type>();
        }

        abstract class TestMessageHandler : IMessageHandler<TestMessage>
        {
            public Task Handle(TestMessage message)
            {
                message.HandlerTypes.Add(GetType());
                return Task.CompletedTask;
            }
        }

        class TestMessageHandler1 : TestMessageHandler { }
        class TestMessageHandler2 : TestMessageHandler { }
    }
}