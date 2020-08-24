using System;
using System.Threading;
using System.Threading.Tasks;
using Handyman.Mediator.Pipelines;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;

namespace Handyman.Mediator.Tests
{
    public class RequestProcessingCancellationTests
    {
        [Fact]
        public async Task ShouldNotInvokeHandlerIfAlreadyCancelled()
        {
            var cts = new CancellationTokenSource();
            var handler = new RequestHandler();

            var services = new ServiceCollection();

            services.AddSingleton<IRequestHandler<Request, Void>>(handler);

            var mediator = new Mediator(services.BuildServiceProvider());

            cts.Cancel();

            (await Should.ThrowAsync<Exception>(Exec(() => mediator.Send(new Request(), cts.Token))))
                .Message.ShouldBe("oce");

            handler.Executed.ShouldBeFalse();
        }

        [Fact]
        public async Task ShouldNotInvokeFiltersIfAlreadyCancelled()
        {
            var cts = new CancellationTokenSource();
            var filter = new RequestFilter(cts);
            var handler = new RequestHandler();

            var services = new ServiceCollection();

            services.AddSingleton<IRequestFilter<Request, Void>>(filter);
            services.AddSingleton<IRequestHandler<Request, Void>>(handler);

            var mediator = new Mediator(services.BuildServiceProvider());

            cts.Cancel();

            (await Should.ThrowAsync<Exception>(Exec(() => mediator.Send(new Request(), cts.Token))))
                .Message.ShouldBe("oce");

            filter.Executed.ShouldBeFalse();
            handler.Executed.ShouldBeFalse();
        }

        [Fact]
        public async Task ShouldStopProcessingIfCancelledAfterFilters()
        {
            var cts = new CancellationTokenSource();
            var filter = new RequestFilter(cts);
            var handler = new RequestHandler();

            var services = new ServiceCollection();

            services.AddSingleton<IRequestFilter<Request, Void>>(filter);
            services.AddSingleton<IRequestHandler<Request, Void>>(handler);

            var mediator = new Mediator(services.BuildServiceProvider());

            (await Should.ThrowAsync<Exception>(Exec(() => mediator.Send(new Request(), cts.Token))))
            .Message.ShouldBe("oce");

            filter.Executed.ShouldBeTrue();
            handler.Executed.ShouldBeFalse();
        }

        [Fact]
        public async Task ShouldStopProcessingIfCancelledDuringFilters()
        {
            var cts = new CancellationTokenSource();
            var filter1 = new RequestFilter(cts);
            var filter2 = new RequestFilter(cts);
            var handler = new RequestHandler();

            var services = new ServiceCollection();

            services.AddSingleton<IRequestFilter<Request, Void>>(filter1);
            services.AddSingleton<IRequestFilter<Request, Void>>(filter2);
            services.AddSingleton<IRequestHandler<Request, Void>>(handler);

            var mediator = new Mediator(services.BuildServiceProvider());

            (await Should.ThrowAsync<Exception>(Exec(() => mediator.Send(new Request(), cts.Token))))
            .Message.ShouldBe("oce");

            filter1.Executed.ShouldBeTrue();
            filter2.Executed.ShouldBeFalse();
            handler.Executed.ShouldBeFalse();
        }

        private static async Task Exec(Func<Task> f)
        {
            try
            {
                await f();
            }
            catch (OperationCanceledException exception)
            {
                throw new Exception("oce", exception);
            }
        }

        private class Request : IRequest { }

        private class RequestFilter : IRequestFilter<Request, Void>
        {
            private readonly CancellationTokenSource _cancellationTokenSource;

            public RequestFilter(CancellationTokenSource cancellationTokenSource)
            {
                _cancellationTokenSource = cancellationTokenSource;
            }

            public bool Executed { get; set; }

            public Task<Void> Execute(RequestPipelineContext<Request> context, RequestFilterExecutionDelegate<Void> next)
            {
                _cancellationTokenSource.Cancel();
                Executed = true;
                return next();
            }
        }

        private class RequestHandler : RequestHandler<Request>
        {
            public bool Executed { get; set; }

            protected override void Handle(Request request, CancellationToken cancellationToken)
            {
                Executed = true;
            }
        }
    }
}
