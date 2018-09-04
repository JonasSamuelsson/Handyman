using Maestro;
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
            var container = new Container();
            container.Configure(x => x.Add<IEventHandler<TestEvent>>().Factory(() => new TestEventHandler()));
            var @event = new TestEvent();
            var tasks = new Mediator(container.GetService).Publish(@event);
            await Task.WhenAll(tasks);
            @event.Handeled.ShouldBeTrue();
        }

        [Fact]
        public async Task ShouldSendRequestWithoutCancellationToken()
        {
            var container = new Container();
            container.Configure(x => x.Add<IRequestHandler<TestRequest, string>>().Factory(() => new TestRequestHandler()));
            (await new Mediator(container.GetService).Send(new TestRequest { Response = "success" })).ShouldBe("success");
        }

        private class TestEvent : IEvent
        {
            public bool Handeled { get; set; }
        }

        private class TestEventHandler : SynchEventHandler<TestEvent>
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

        private class TestRequestHandler : SynchRequestHandler<TestRequest, string>
        {
            protected override string Handle(TestRequest request, CancellationToken cancellationToken) => request.Response;
        }
    }
}