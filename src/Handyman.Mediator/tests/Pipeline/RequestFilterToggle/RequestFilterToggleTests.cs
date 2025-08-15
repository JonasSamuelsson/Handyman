using Handyman.Mediator.Pipeline.RequestFilterToggle;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Handyman.Mediator.Tests.Pipeline.RequestFilterToggle
{
    public class RequestFilterToggleTests
    {
        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task ShouldToggleRequestFilter(bool toggleEnabled)
        {
            var toggle = new RequestFilterToggle { Enabled = toggleEnabled };
            var toggledFilter = new ToggleEnabledRequestFilter();
            var fallbackFilter = new ToggleDisabledRequestFilter();

            var services = new ServiceCollection();

            services.AddScoped<IMediator>(x => new Mediator(x));
            services.AddSingleton<IRequestFilterToggle>(toggle);
            services.AddSingleton<IRequestFilter<Request, object>>(toggledFilter);
            services.AddSingleton<IRequestFilter<Request, object>>(fallbackFilter);
            services.AddTransient<IRequestHandler<Request, object>, RequestHandler>();

            var mediator = services.BuildServiceProvider().GetService<IMediator>();

            await mediator.Send(new Request());

            toggle.ToggleMetadata.Name.ShouldBe("test");
            toggle.ToggleMetadata.Tags.ShouldBe(new[] { "foo" });
            toggle.ToggleMetadata.ToggleDisabledFilterTypes.ShouldBe(new[] { typeof(ToggleDisabledRequestFilter) });
            toggle.ToggleMetadata.ToggleEnabledFilterTypes.ShouldBe(new[] { typeof(ToggleEnabledRequestFilter) });

            toggledFilter.Executed.ShouldBe(toggleEnabled);
            fallbackFilter.Executed.ShouldBe(!toggleEnabled);
        }

        [RequestFilterToggle(typeof(ToggleEnabledRequestFilter), ToggleDisabledFilterTypes = new[] { typeof(ToggleDisabledRequestFilter) }, Name = "test", Tags = new[] { "foo" })]
        private class Request : IRequest<object>
        {
        }

        private class ToggleEnabledRequestFilter : IRequestFilter<Request, object>
        {
            public bool Executed { get; set; }

            public Task<object> Execute(RequestContext<Request> requestContext, RequestFilterExecutionDelegate<object> next)
            {
                Executed = true;
                return next();
            }
        }

        private class ToggleDisabledRequestFilter : IRequestFilter<Request, object>
        {
            public bool Executed { get; set; }

            public Task<object> Execute(RequestContext<Request> requestContext, RequestFilterExecutionDelegate<object> next)
            {
                Executed = true;
                return next();
            }
        }

        private class RequestFilterToggle : IRequestFilterToggle
        {
            public bool Enabled { get; set; }
            public RequestFilterToggleMetadata ToggleMetadata { get; set; }

            public Task<bool> IsEnabled<TRequest, TResponse>(RequestFilterToggleMetadata toggleMetadata,
                RequestContext<TRequest> requestContext)
                where TRequest : IRequest<TResponse>
            {
                ToggleMetadata = toggleMetadata;
                return Task.FromResult(Enabled);
            }
        }

        private class RequestHandler : SyncRequestHandler<Request, object>
        {
            public override object Handle(Request request, CancellationToken cancellationToken)
            {
                return null;
            }
        }
    }
}