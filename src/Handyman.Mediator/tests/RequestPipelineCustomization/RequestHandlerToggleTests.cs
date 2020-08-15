using Handyman.Mediator.RequestPipelineCustomization;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
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
            var toggle = new RequestHandlerToggle { Enabled = toggleEnabled };
            var toggledHandler = new ToggleEnabledRequestHandler();
            var fallbackHandler = new ToggleDisabledRequestHandler();

            var services = new ServiceCollection();

            services.AddScoped<IMediator>(x => new Mediator(x));
            services.AddSingleton<IRequestHandlerToggle>(toggle);
            services.AddSingleton<IRequestHandler<Request, object>>(toggledHandler);
            services.AddSingleton<IRequestHandler<Request, object>>(fallbackHandler);

            await services.BuildServiceProvider().GetService<IMediator>().Send(new Request());

            toggle.ToggleMetaData.Name.ShouldBe("test");
            toggle.ToggleMetaData.Tags.ShouldBe(new[] { "foo" });
            toggle.ToggleMetaData.ToggleDisabledHandlerType.ShouldBe(typeof(ToggleDisabledRequestHandler));
            toggle.ToggleMetaData.ToggleEnabledHandlerType.ShouldBe(typeof(ToggleEnabledRequestHandler));

            toggledHandler.Executed.ShouldBe(toggleEnabled);
            fallbackHandler.Executed.ShouldBe(!toggleEnabled);
        }

        [RequestHandlerToggle(typeof(ToggleEnabledRequestHandler), ToggleDisabledHandlerType = typeof(ToggleDisabledRequestHandler), Name = "test", Tags = new[] { "foo" })]
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
            public RequestHandlerToggleMetaData ToggleMetaData { get; set; }

            public Task<bool> IsEnabled<TRequest, TResponse>(RequestHandlerToggleMetaData toggleMetaData,
                RequestPipelineContext<TRequest> context)
                where TRequest : IRequest<TResponse>
            {
                ToggleMetaData = toggleMetaData;
                return Task.FromResult(Enabled);
            }
        }
    }
}