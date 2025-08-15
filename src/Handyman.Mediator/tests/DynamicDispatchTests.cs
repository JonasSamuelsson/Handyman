using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Handyman.Mediator.Tests
{
    public class DynamicDispatchTests
    {
        [Fact]
        public async Task ShouldPublishEvent()
        {
            var serviceProvider = new ServiceCollection().BuildServiceProvider();
            var dynamicMediator = new DynamicMediator(new Mediator(serviceProvider));

            object @event = new Event();

            await dynamicMediator.Publish(@event, CancellationToken.None);
        }

        private class Event : IEvent
        {
        }

        [Fact]
        public void ShouldThrowIfEventDoesNotImplementIEvent()
        {
            var serviceProvider = new ServiceCollection().BuildServiceProvider();
            var dynamicMediator = new DynamicMediator(new Mediator(serviceProvider));

            object @event = new object();

            Should.Throw<InvalidCastException>(() => dynamicMediator.Publish(@event, CancellationToken.None));
        }

        [Fact]
        public async Task ShouldSendRequest()
        {
            var serviceProvider = new ServiceCollection()
                .AddTransient<IRequestHandler<Request, string>, RequestHandler>()
                .BuildServiceProvider();
            var dynamicMediator = new DynamicMediator(new Mediator(serviceProvider));

            object request = new Request();

            (await dynamicMediator.Send(request, CancellationToken.None)).ShouldBe("success");
        }

        private class Request : IRequest<string>
        {
        }

        private class RequestHandler : SyncRequestHandler<Request, string>
        {
            public override string Handle(Request request, CancellationToken cancellationToken) => "success";
        }

        [Fact]
        public void ShouldThrowIfRequestDoesNotImplementIRequest()
        {
            var serviceProvider = new ServiceCollection().BuildServiceProvider();
            var dynamicMediator = new DynamicMediator(new Mediator(serviceProvider));

            object request = new object();

            Should.Throw<InvalidCastException>(() => dynamicMediator.Send(request, CancellationToken.None));
        }

        [Fact]
        public void ShouldThrowIfRequestHasMultipleImplementationsOfIRequest()
        {
            var serviceProvider = new ServiceCollection().BuildServiceProvider();
            var dynamicMediator = new DynamicMediator(new Mediator(serviceProvider));

            object request = new FailRequest();

            Should.Throw<InvalidCastException>(() => dynamicMediator.Send(request, CancellationToken.None));
        }

        private class FailRequest : IRequest<int>, IRequest<string>
        {
        }
    }
}