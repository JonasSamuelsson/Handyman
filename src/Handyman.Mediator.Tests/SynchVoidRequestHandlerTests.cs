using System.Threading;
using System.Threading.Tasks;
using Maestro;
using Shouldly;
using Xunit;

namespace Handyman.Mediator.Tests
{
    public class SynchVoidRequestHandlerTests
    {
        [Fact]
        public async Task UseSynchVoidRequestHandler()
        {
            var handler = new SynchVoidRequestHandler();
            var container = new Container(x => x.Add<IRequestHandler<VoidRequest, Void>>().Instance(handler));
            var mediator = new Mediator(container.GetService);
            await mediator.Send(new VoidRequest(), CancellationToken.None);
            handler.Executed.ShouldBeTrue();
        }

        private class VoidRequest : IRequest { }

        private class SynchVoidRequestHandler : SynchVoidRequestHandler<VoidRequest>
        {
            public bool Executed { get; set; }

            protected override void Handle(VoidRequest request, CancellationToken cancellationToken)
            {
                Executed = true;
            }
        }
    }
}