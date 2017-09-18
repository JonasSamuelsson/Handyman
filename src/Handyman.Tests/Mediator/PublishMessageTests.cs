using Handyman.Mediator;
using Shouldly;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Xunit;

namespace Handyman.Tests.Mediator
{
    public class PublishMessageTests
    {
        [Fact]
        public void ShouldPublishMessage()
        {
            var message = new Message();
            var serviceProvider = new ServiceProvider(typeof(IMessageHandler<Message>), typeof(MessageHandler1), typeof(MessageHandler2));
            var mediator = new Handyman.Mediator.Mediator(serviceProvider.GetService, serviceProvider.GetServices);

            mediator.Publish(message);

            message.HandlerTypes.Count.ShouldBe(2);
            message.HandlerTypes.ShouldContain(typeof(MessageHandler1));
            message.HandlerTypes.ShouldContain(typeof(MessageHandler2));
        }

        class Message : IMessage
        {
            public ConcurrentBag<Type> HandlerTypes { get; } = new ConcurrentBag<Type>();
        }

        abstract class MessageHandler : IMessageHandler<Message>
        {
            public void Handle(Message message)
            {
                message.HandlerTypes.Add(GetType());
            }
        }

        class MessageHandler1 : MessageHandler { }
        class MessageHandler2 : MessageHandler { }

        [Fact]
        public async Task ShouldPublishAsyncMessage()
        {
            var message = new AsyncMessage();
            var serviceProvider = new ServiceProvider(typeof(IAsyncMessageHandler<AsyncMessage>), typeof(AsyncMessageHandler1), typeof(AsyncMessageHandler2));
            var mediator = new Handyman.Mediator.Mediator(serviceProvider.GetService, serviceProvider.GetServices);

            await Task.WhenAll(mediator.Publish(message));

            message.HandlerTypes.Count.ShouldBe(2);
            message.HandlerTypes.ShouldContain(typeof(AsyncMessageHandler1));
            message.HandlerTypes.ShouldContain(typeof(AsyncMessageHandler2));
        }

        class AsyncMessage : IAsyncMessage
        {
            public ConcurrentBag<Type> HandlerTypes { get; } = new ConcurrentBag<Type>();
        }

        abstract class AsyncMessageHandler : IAsyncMessageHandler<AsyncMessage>
        {
            public Task Handle(AsyncMessage message)
            {
                message.HandlerTypes.Add(GetType());
                return Task.CompletedTask;
            }
        }

        class AsyncMessageHandler1 : AsyncMessageHandler { }
        class AsyncMessageHandler2 : AsyncMessageHandler { }
    }
}