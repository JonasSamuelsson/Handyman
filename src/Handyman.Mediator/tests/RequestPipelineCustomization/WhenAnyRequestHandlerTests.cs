using Handyman.Mediator.RequestPipelineCustomization;
using Maestro;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Handyman.Mediator.Tests.RequestPipelineCustomization
{
    public class WhenAnyRequestHandlerTests
    {
        [Fact]
        public async Task ShouldCallAllHandlers()
        {
            var handler1 = new RequestHandler(Task.FromResult("success"));
            var handler2 = new RequestHandler(Task.FromResult("success"));

            var services = new ServiceCollection();

            services.AddSingleton<IRequestHandler<Request, string>>(handler1);
            services.AddSingleton<IRequestHandler<Request, string>>(handler2);

            var mediator = new Mediator(services.BuildServiceProvider());

            await mediator.Send(new Request());

            handler1.Executed.ShouldBeTrue();
            handler2.Executed.ShouldBeTrue();
        }

        [Theory]
        [InlineData("1")]
        [InlineData("2")]
        public async Task ShouldReturnWhenAnyHandlerCompletes(string result)
        {
            var cts1 = new TaskCompletionSource<string>();
            var cts2 = new TaskCompletionSource<string>();

            var handler1 = new RequestHandler(cts1.Task);
            var handler2 = new RequestHandler(cts2.Task);

            var services = new ServiceCollection();

            services.AddSingleton<IRequestHandler<Request, string>>(handler1);
            services.AddSingleton<IRequestHandler<Request, string>>(handler2);

            var mediator = new Mediator(services.BuildServiceProvider());

            var task = mediator.Send(new Request());

            task.Status.ShouldNotBe(TaskStatus.RanToCompletion);

            cts1.SetResult(result);

            (await task).ShouldBe(result);
        }

        [Fact]
        public async Task ShouldReturnSuccessEvenIfSomeHandlersFail()
        {
            var tcs1 = new TaskCompletionSource<string>();
            var tcs2 = new TaskCompletionSource<string>();

            var handler1 = new RequestHandler(tcs1.Task);
            var handler2 = new RequestHandler(tcs2.Task);

            var services = new ServiceCollection();

            services.AddSingleton<IRequestHandler<Request, string>>(handler1);
            services.AddSingleton<IRequestHandler<Request, string>>(handler2);

            var mediator = new Mediator(services.BuildServiceProvider());

            var task = mediator.Send(new Request());

            await Task.Delay(50);

            tcs1.SetException(new Exception());

            await Task.Delay(50);

            tcs2.SetResult("success");

            (await task).ShouldBe("success");
        }

        [Fact]
        public async Task OnceTheFirstHandlerCompletesOtherHandlersShouldBeCancelled()
        {
            var tcs1 = new TaskCompletionSource<string>();
            var tcs2 = new TaskCompletionSource<string>();

            var handler1 = new RequestHandler(tcs1.Task);
            var handler2 = new RequestHandler(tcs2.Task);

            var services = new ServiceCollection();

            services.AddSingleton<IRequestHandler<Request, string>>(handler1);
            services.AddSingleton<IRequestHandler<Request, string>>(handler2);

            var mediator = new Mediator(services.BuildServiceProvider());

            var task = mediator.Send(new Request());

            tcs1.SetResult("success");

            await task;

            var timeout = Task.Delay(100);

            while (!timeout.IsCompletedSuccessfully() && !handler2.Cancelled)
            {
                await Task.Delay(1);
            }

            handler2.Cancelled.ShouldBeTrue();
        }

        [WhenAnyRequestHandler]
        private class Request : IRequest<string> { }

        private class RequestHandler : IRequestHandler<Request, string>
        {
            private readonly Task<string> _task;

            public RequestHandler(Task<string> task)
            {
                _task = task;
            }

            public bool Cancelled { get; set; }
            public bool Executed { get; set; }

            public async Task<string> Handle(Request request, CancellationToken cancellationToken)
            {
                Executed = true;

                while (!_task.IsCompleted)
                {
                    if (!cancellationToken.IsCancellationRequested)
                    {
                        await Task.Delay(1);
                        continue;
                    }

                    Cancelled = true;
                    return null;
                }

                return await _task;
            }
        }
    }
}
