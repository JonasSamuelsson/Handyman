using Handyman.Mediator.Pipeline.RequestHandlerToggle;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Handyman.Mediator.Tests.Pipeline.RequestHandlerToggle
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

            toggle.ToggleMetadata.Name.ShouldBe("test");
            toggle.ToggleMetadata.Tags.ShouldBe(new[] { "foo" });
            toggle.ToggleMetadata.ToggleDisabledHandlerTypes.ShouldBe(new[] { typeof(ToggleDisabledRequestHandler) });
            toggle.ToggleMetadata.ToggleEnabledHandlerTypes.ShouldBe(new[] { typeof(ToggleEnabledRequestHandler) });

            toggledHandler.Executed.ShouldBe(toggleEnabled);
            fallbackHandler.Executed.ShouldBe(!toggleEnabled);
        }

        [RequestHandlerToggle(typeof(ToggleEnabledRequestHandler), ToggleDisabledHandlerTypes = new[] { typeof(ToggleDisabledRequestHandler) }, Name = "test", Tags = new[] { "foo" })]
        private class Request : IRequest<object>
        {
        }

        private class ToggleEnabledRequestHandler : SyncRequestHandler<Request, object>
        {
            public bool Executed { get; set; }

            public override object Handle(Request request, CancellationToken cancellationToken)
            {
                Executed = true;
                return null;
            }
        }

        private class ToggleDisabledRequestHandler : SyncRequestHandler<Request, object>
        {
            public bool Executed { get; set; }

            public override object Handle(Request request, CancellationToken cancellationToken)
            {
                Executed = true;
                return null;
            }
        }

        private class RequestHandlerToggle : IRequestHandlerToggle
        {
            public bool Enabled { get; set; }
            public RequestHandlerToggleMetadata ToggleMetadata { get; set; }

            public Task<bool> IsEnabled<TRequest, TResponse>(RequestHandlerToggleMetadata toggleMetadata,
                RequestContext<TRequest> requestContext)
                where TRequest : IRequest<TResponse>
            {
                ToggleMetadata = toggleMetadata;
                return Task.FromResult(Enabled);
            }
        }
    }
}