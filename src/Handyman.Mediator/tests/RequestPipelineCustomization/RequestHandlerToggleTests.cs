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
        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task ShouldToggleRequestHandler(bool toggleEnabled)
        {
            var toggle = new RequestHandlerToggle { Enabled = toggleEnabled };
            var toggledHandler = new ToggleEnabledRequestHandler();
            var fallbackHandler = new ToggleDisabledRequestHandler();

            var services = new ServiceCollection();

            services.AddScoped<IMediator>(x => new Mediator(x));
            services.AddSingleton<IRequestHandlerToggle>(toggle);
            services.AddSingleton<IRequestHandler<Request, object>>(toggledHandler);
            services.AddSingleton<IRequestHandler<Request, object>>(fallbackHandler);

            await services.BuildServiceProvider().GetService<IMediator>().Send(new Request());

            toggle.RequestHandlerType.ShouldBe(typeof(ToggleEnabledRequestHandler));
            toggledHandler.Executed.ShouldBe(toggleEnabled);
            fallbackHandler.Executed.ShouldBe(!toggleEnabled);
        }

        [RequestHandlerToggle(typeof(ToggleEnabledRequestHandler), ToggleDisabledHandlerType = typeof(ToggleDisabledRequestHandler))]
        private class Request : IRequest<object> { }

        private class ToggleEnabledRequestHandler : RequestHandler<Request, object>
        {
            public bool Executed { get; set; }

            protected override object Handle(Request request, CancellationToken cancellationToken)
            {
                Executed = true;
                return null;
            }
        }

        private class ToggleDisabledRequestHandler : RequestHandler<Request, object>
        {
            public bool Executed { get; set; }

            protected override object Handle(Request request, CancellationToken cancellationToken)
            {
                Executed = true;
                return null;
            }
        }

        private class RequestHandlerToggle : IRequestHandlerToggle
        {
            public bool Enabled { get; set; }
            public Type RequestHandlerType { get; set; }

            public Task<bool> IsEnabled<TRequest, TResponse>(Type requestHandlerType, RequestPipelineContext<TRequest> context)
                where TRequest : IRequest<TResponse>
            {
                RequestHandlerType = requestHandlerType;
                return Task.FromResult(Enabled);
            }
        }
    }
}