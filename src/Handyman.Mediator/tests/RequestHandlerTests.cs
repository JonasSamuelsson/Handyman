using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;

namespace Handyman.Mediator.Tests
{
    public class RequestHandlerTests
    {
        [Fact]
        public async Task AsyncVoidImplementation()
        {
            var handler = new AsyncVoidHandler();
            var services = new ServiceCollection().AddSingleton<IRequestHandler<VoidRequest, Void>>(handler);
            var mediator = new Mediator(services.BuildServiceProvider());
            await mediator.Send(new VoidRequest());
        }

        [Fact]
        public async Task SyncVoidImplementation()
        {
            var handler = new SyncVoidHandler();
            var services = new ServiceCollection().AddSingleton<IRequestHandler<VoidRequest, Void>>(handler);
            var mediator = new Mediator(services.BuildServiceProvider());
            await mediator.Send(new VoidRequest());
        }

        [Fact]
        public async Task SyncImplementation()
        {
            var handler = new SyncHandler();
            var services = new ServiceCollection().AddSingleton<IRequestHandler<StringRequest, string>>(handler);
            var mediator = new Mediator(services.BuildServiceProvider());
            (await mediator.Send(new StringRequest())).ShouldBe("success");
        }

        private class VoidRequest : IRequest { }

        private class AsyncVoidHandler : RequestHandler<VoidRequest>
        {
            protected override Task HandleAsync(VoidRequest request, CancellationToken cancellationToken)
            {
                return Task.CompletedTask;
            }
        }

        private class SyncVoidHandler : RequestHandler<VoidRequest>
        {
            protected override void Handle(VoidRequest request, CancellationToken cancellationToken)
            {
            }
        }

        private class StringRequest : IRequest<string> { }

        private class SyncHandler : RequestHandler<StringRequest, string>
        {
            protected override string Handle(StringRequest request, CancellationToken cancellationToken)
            {
                return "success";
            }
        }
    }
}