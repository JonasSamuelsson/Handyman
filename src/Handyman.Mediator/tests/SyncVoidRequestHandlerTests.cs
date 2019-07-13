using Maestro;
using Shouldly;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Handyman.Mediator.Tests
{
    public class SyncVoidRequestHandlerTests
    {
        [Fact]
        public async Task UseSyncVoidRequestHandler()
        {
            var handler = new SyncRequestHandler();
            var container = new Container(x => x.Add<IRequestHandler<VoidRequest, Void>>().Instance(handler));
            var mediator = new Mediator(container.GetService);
            await mediator.Send(new VoidRequest(), CancellationToken.None);
            handler.Executed.ShouldBeTrue();
        }

        private class VoidRequest : IRequest { }

        private class SyncRequestHandler : SyncRequestHandler<VoidRequest>
        {
            public bool Executed { get; set; }

            protected override void Handle(VoidRequest request, CancellationToken cancellationToken)
            {
                Executed = true;
            }
        }
    }
}