using System;
using System.Threading;
using System.Threading.Tasks;
using Handyman.Mediator.RequestPipelineCustomization;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;

namespace Handyman.Mediator.Tests.RequestPipelineCustomization
{
    public class RequestHandlerToggleTests
    {
        [Fact]
        public async Task ShouldToggleRequestHandler()
        {
            var toggle = new RequestHandlerToggle<Request, Type>();

            var services = new ServiceCollection();

            services.AddScoped<IMediator>(x => new Mediator(x));
            services.AddTransient<IRequestHandler<Request, Type>, DefaultRequestHandler>();
            services.AddTransient<IRequestHandler<Request, Type>, ToggledRequestHandler>();
            services.AddSingleton<IRequestHandlerToggle<Request, Type>>(toggle);

            var mediator = services.BuildServiceProvider().GetRequiredService<IMediator>();

            (await mediator.Send(new Request())).ShouldBe(typeof(DefaultRequestHandler));

            toggle.Enabled = true;

            (await mediator.Send(new Request())).ShouldBe(typeof(ToggledRequestHandler));
        }

        [RequestHandlerToggle(typeof(ToggledRequestHandler), DefaultHandlerType = typeof(DefaultRequestHandler))]
        private class Request : IRequest<Type> { }

        private class DefaultRequestHandler : IRequestHandler<Request, Type>
        {
            public Task<Type> Handle(Request request, CancellationToken cancellationToken)
            {
                return Task.FromResult(GetType());
            }
        }

        private class ToggledRequestHandler : IRequestHandler<Request, Type>
        {
            public Task<Type> Handle(Request request, CancellationToken cancellationToken)
            {
                return Task.FromResult(GetType());
            }
        }

        private class RequestHandlerToggle<TRequest, TResponse> : IRequestHandlerToggle<TRequest, TResponse>
            where TRequest : IRequest<TResponse>
        {
            public bool Enabled { get; set; }

            public Task<bool> IsEnabled(TRequest request)
            {
                return Task.FromResult(Enabled);
            }
        }
    }
}