using Handyman.Mediator.RequestPipelineCustomization;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Handyman.Mediator.Tests.RequestPipelineCustomization
{
    public class RequestFilterToggleTests
    {
        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task ShouldToggleRequestFilter(bool toggleEnabled)
        {
            var toggle = new RequestFilterToggle { Enabled = toggleEnabled };
            var toggledFilter = new ToggledEnabledRequestFilter();
            var fallbackFilter = new ToggleDisabledRequestFilter();

            var services = new ServiceCollection();

            services.AddScoped<IMediator>(x => new Mediator(x));
            services.AddSingleton<IRequestFilterToggle>(toggle);
            services.AddSingleton<IRequestFilter<Request, object>>(toggledFilter);
            services.AddSingleton<IRequestFilter<Request, object>>(fallbackFilter);
            services.AddTransient<IRequestHandler<Request, object>, RequestHandler>();

            var mediator = services.BuildServiceProvider().GetService<IMediator>();

            await mediator.Send(new Request());

            toggle.ToggleMetaData.Tags.ShouldBe(new[] { "foo" });
            toggle.ToggleMetaData.ToggleDisabledFilterType.ShouldBe(typeof(ToggleDisabledRequestFilter));
            toggle.ToggleMetaData.ToggleEnabledFilterType.ShouldBe(typeof(ToggledEnabledRequestFilter));
            toggle.ToggleMetaData.ToggleName.ShouldBe("test");

            toggledFilter.Executed.ShouldBe(toggleEnabled);
            fallbackFilter.Executed.ShouldBe(!toggleEnabled);
        }

        [RequestFilterToggle(typeof(ToggledEnabledRequestFilter), ToggleDisabledFilterType = typeof(ToggleDisabledRequestFilter), ToggleName = "test", Tags = new[] { "foo" })]
        private class Request : IRequest<object> { }

        private class ToggledEnabledRequestFilter : IRequestFilter<Request, object>
        {
            public bool Executed { get; set; }

            public Task<object> Execute(RequestPipelineContext<Request> context, RequestFilterExecutionDelegate<object> next)
            {
                Executed = true;
                return next();
            }
        }

        private class ToggleDisabledRequestFilter : IRequestFilter<Request, object>
        {
            public bool Executed { get; set; }

            public Task<object> Execute(RequestPipelineContext<Request> context, RequestFilterExecutionDelegate<object> next)
            {
                Executed = true;
                return next();
            }
        }

        private class RequestFilterToggle : IRequestFilterToggle
        {
            public bool Enabled { get; set; }
            public RequestFilterToggleMetaData ToggleMetaData { get; set; }

            public Task<bool> IsEnabled<TRequest, TResponse>(RequestFilterToggleMetaData toggleMetaData,
                RequestPipelineContext<TRequest> context)
                where TRequest : IRequest<TResponse>
            {
                ToggleMetaData = toggleMetaData;
                return Task.FromResult(Enabled);
            }
        }

        private class RequestHandler : RequestHandler<Request, object>
        {
            protected override object Handle(Request request, CancellationToken cancellationToken)
            {
                return null;
            }
        }
    }
}