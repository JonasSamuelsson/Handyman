using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Handyman.Dispatch;
using Shouldly;
using Xunit;

namespace Handyman.Tests.Dispatch
{
    public class PublishMessageTests
    {
        [Fact]
        public async Task ShouldPublishMessage()
        {
            var message = new TestMessage();
            var handlerProvider = new TestHandlerProvider(typeof(IMessageHandler<TestMessage>), typeof(TestMessageHandler1), typeof(TestMessageHandler2));
            var dispatcher = new Dispatcher(handlerProvider);

            await Task.WhenAll(dispatcher.Publish(message));

            message.HandlerTypes.Count.ShouldBe(2);
            message.HandlerTypes.ShouldContain(typeof(TestMessageHandler1));
            message.HandlerTypes.ShouldContain(typeof(TestMessageHandler2));
        }

        class TestMessage : IMessage
        {
            public ICollection<Type> HandlerTypes { get; } = new List<Type>();
        }

        abstract class TestMessageHandler : IMessageHandler<TestMessage>
        {
            public Task Handle(TestMessage message)
            {
                lock (message) message.HandlerTypes.Add(GetType());
                return Task.CompletedTask;
            }
        }

        class TestMessageHandler1 : TestMessageHandler { }
        class TestMessageHandler2 : TestMessageHandler { }
    }
}