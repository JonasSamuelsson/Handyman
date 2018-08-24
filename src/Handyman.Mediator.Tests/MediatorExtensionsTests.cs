using Shouldly;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Handyman.Mediator.Tests
{
    public class MediatorExtensionsTests
    {
        [Fact]
        public async Task ShouldPublishEventWithoutCancellationToken()
        {
            var services = new ServiceProvider();
            services.Add<IEventHandler<TestEvent>>(() => new TestEventHandler());
            var @event = new TestEvent();
            var tasks = new Mediator(services).Publish(@event);
            await Task.WhenAll(tasks);
            @event.Handeled.ShouldBeTrue();
        }

        [Fact]
        public async Task ShouldSendRequestWithoutCancellationToken()
        {
            var services = new ServiceProvider();
            services.Add<IRequestHandler<TestRequest, string>>(() => new TestRequestHandler());
            (await new Mediator(services).Send(new TestRequest { Response = "success" })).ShouldBe("success");
        }

        private class TestEvent : IEvent
        {
            public bool Handeled { get; set; }
        }

        private class TestEventHandler : SynchronousEventHandler<TestEvent>
        {
            protected override void Handle(TestEvent @event)
            {
                @event.Handeled = true;
            }
        }

        private class TestRequest : IRequest<string>
        {
            public string Response { get; set; }
        }

        private class TestRequestHandler : SynchronousRequestHandler<TestRequest, string>
        {
            protected override string Handle(TestRequest request, CancellationToken cancellationToken) => request.Response;
        }
    }
}