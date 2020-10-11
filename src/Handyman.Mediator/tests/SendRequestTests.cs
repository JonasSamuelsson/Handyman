using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Handyman.Mediator.Tests
{
    public class SendRequestTests
    {
        [Fact]
        public async Task ShouldSendRequestUsingMediator()
        {
            var mediator = new ServiceCollection()
                .AddTransient<IMediator>(sp => new Mediator(sp))
                .AddTransient<IRequestHandler<Request, string>, Handler>()
                .BuildServiceProvider()
                .GetRequiredService<IMediator>();

            (await mediator.Send(new Request(), CancellationToken.None)).ShouldBe("success");
        }

        [Fact]
        public async Task ShouldSendRequestUsingNonGenericSender()
        {
            var sender = new ServiceCollection()
                .AddTransient<ISender>(sp => sp.GetRequiredService<IMediator>())
                .AddTransient<IMediator>(sp => new Mediator(sp))
                .AddTransient<IRequestHandler<Request, string>, Handler>()
                .BuildServiceProvider()
                .GetRequiredService<ISender>();

            (await sender.Send(new Request(), CancellationToken.None)).ShouldBe("success");
        }

        [Fact]
        public async Task ShouldSendRequestUsingGenericSender()
        {
            var sender = new ServiceCollection()
                .AddTransient(typeof(ISender<,>), typeof(Sender<,>))
                .AddTransient<IMediator>(sp => new Mediator(sp))
                .AddTransient<IRequestHandler<Request, string>, Handler>()
                .BuildServiceProvider()
                .GetRequiredService<ISender<Request, string>>();

            (await sender.Send(new Request(), CancellationToken.None)).ShouldBe("success");
        }

        [Fact]
        public async Task ShouldSendVoidRequestUsingNonGenericSender()
        {
            var sender = new ServiceCollection()
                .AddTransient(typeof(ISender<>), typeof(Sender<>))
                .AddTransient<IMediator>(sp => new Mediator(sp))
                .AddTransient<IRequestHandler<VoidRequest, Void>, Handler>()
                .BuildServiceProvider()
                .GetRequiredService<ISender<VoidRequest>>();

            await sender.Send(new VoidRequest(), CancellationToken.None);
        }

        private class Request : IRequest<string> { }

        private class VoidRequest : IRequest { }

        private class Handler : IRequestHandler<Request, string>, IRequestHandler<VoidRequest>
        {
            public Task<string> Handle(Request request, CancellationToken cancellationToken)
            {
                return Task.FromResult("success");
            }

            public Task<Void> Handle(VoidRequest request, CancellationToken cancellationToken)
            {
                return Task.FromResult(Void.Instance);
            }
        }
    }
}