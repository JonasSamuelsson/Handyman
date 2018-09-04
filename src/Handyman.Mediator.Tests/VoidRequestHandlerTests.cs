﻿using Maestro;
using Shouldly;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Handyman.Mediator.Tests
{
    public class VoidRequestHandlerTests
    {
        [Fact]
        public async Task UseVoidRequestHandler()
        {
            var handler = new VoidRequestHandler();
            var container = new Container(x => x.Add<IRequestHandler<VoidRequest, Void>>().Instance(handler));
            var mediator = new Mediator(container.GetService);
            await mediator.Send(new VoidRequest(), CancellationToken.None);
            handler.Executed.ShouldBeTrue();
        }

        private class VoidRequest : IRequest { }

        private class VoidRequestHandler : VoidRequestHandler<VoidRequest>
        {
            public bool Executed { get; set; }

            protected override Task Handle(VoidRequest request, CancellationToken cancellationToken)
            {
                Executed = true;
                return Task.CompletedTask;
            }
        }
    }
}