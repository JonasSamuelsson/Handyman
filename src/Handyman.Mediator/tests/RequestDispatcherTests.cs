using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;

namespace Handyman.Mediator.Tests
{
    public class RequestDispatcherTests
    {
        [Fact]
        public async Task ShouldSendRequestWithResponse()
        {
            var services = new ServiceCollection()
                .AddTransient<IMediator>(x => new Mediator(x))
                .AddTransient<IRequestHandler<RequestWithResponse, string>, Handler>()
                .AddTransient(typeof(IRequestDispatcher<,>), typeof(RequestDispatcher<,>))
                .BuildServiceProvider();

            var dispatcher = services.GetRequiredService<IRequestDispatcher<RequestWithResponse, string>>();

            (await dispatcher.Send(new RequestWithResponse(), CancellationToken.None)).ShouldBe("success");
        }

        [Fact]
        public async Task ShouldSendRequestWithoutResponse()
        {
            var handler = new Handler();

            var services = new ServiceCollection()
                .AddTransient<IMediator>(x => new Mediator(x))
                .AddSingleton<IRequestHandler<RequestWithoutResponse, Void>>(handler)
                .AddTransient(typeof(IRequestDispatcher<>), typeof(RequestDispatcher<>))
                .BuildServiceProvider();

            var dispatcher = services.GetRequiredService<IRequestDispatcher<RequestWithoutResponse>>();

            await dispatcher.Send(new RequestWithoutResponse(), CancellationToken.None);

            handler.Executed.ShouldBeTrue();
        }

        private class RequestWithResponse : IRequest<string> { }

        private class RequestWithoutResponse : IRequest { }

        private class Handler : IRequestHandler<RequestWithResponse, string>, IRequestHandler<RequestWithoutResponse>
        {
            public bool Executed { get; set; }

            public Task<string> Handle(RequestWithResponse request, CancellationToken cancellationToken)
            {
                return Task.FromResult("success");
            }

            public Task<Void> Handle(RequestWithoutResponse request, CancellationToken cancellationToken)
            {
                Executed = true;
                return Task.FromResult(Void.Instance);
            }
        }
    }
}