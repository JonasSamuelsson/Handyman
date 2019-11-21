using Handyman.Mediator.RequestPipelineCustomization;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Handyman.Mediator.Tests.RequestPipelineCustomization
{
    public class RequestHandlerToggleTests
    {
        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task ShouldToggleRequestHandler(bool toggleEnabled)
        {
            var toggle = new RequestHandlerToggle<Request, object> { Enabled = toggleEnabled };
            var toggledHandler = new ToggledRequestHandler();
            var fallbackHandler = new FallbackRequestHandler();

            var services = new ServiceCollection();

            services.AddScoped<IMediator>(x => new Mediator(x));
            services.AddSingleton<IRequestHandlerToggle<Request, object>>(toggle);
            services.AddSingleton<IRequestHandler<Request, object>>(toggledHandler);
            services.AddSingleton<IRequestHandler<Request, object>>(fallbackHandler);

            await services.BuildServiceProvider().GetService<IMediator>().Send(new Request());

            toggle.RequestHandlerType.ShouldBe(typeof(ToggledRequestHandler));
            toggledHandler.Executed.ShouldBe(toggleEnabled);
            fallbackHandler.Executed.ShouldBe(!toggleEnabled);
        }

        [RequestHandlerToggle(typeof(ToggledRequestHandler), FallbackHandlerType = typeof(FallbackRequestHandler))]
        private class Request : IRequest<object> { }

        private class ToggledRequestHandler : RequestHandler<Request, object>
        {
            public bool Executed { get; set; }

            protected override object Handle(Request request, CancellationToken cancellationToken)
            {
                Executed = true;
                return null;
            }
        }

        private class FallbackRequestHandler : RequestHandler<Request, object>
        {
            public bool Executed { get; set; }

            protected override object Handle(Request request, CancellationToken cancellationToken)
            {
                Executed = true;
                return null;
            }
        }

        private class RequestHandlerToggle<TRequest, TResponse> : IRequestHandlerToggle<TRequest, TResponse>
            where TRequest : IRequest<TResponse>
        {
            public bool Enabled { get; set; }
            public Type RequestHandlerType { get; set; }

            public Task<bool> IsEnabled(Type requestHandlerType, RequestPipelineContext<TRequest> context)
            {
                RequestHandlerType = requestHandlerType;
                return Task.FromResult(Enabled);
            }
        }
    }
}