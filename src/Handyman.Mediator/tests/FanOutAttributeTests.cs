using Maestro;
using Shouldly;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Handyman.Mediator.Tests
{
    public class FanOutAttributeTests
    {
        [Fact]
        public async Task ShouldCallAllHandlers()
        {
            var handler1 = new RequestHandler(Task.FromResult("success"));
            var handler2 = new RequestHandler(Task.FromResult("success"));

            var container = new Container(x =>
            {
                x.Add<IRequestHandler<Request, string>>().Instance(handler1);
                x.Add<IRequestHandler<Request, string>>().Instance(handler2);
            });

            var mediator = new Mediator(container.GetService);

            await mediator.Send(new Request());

            handler1.Executed.ShouldBeTrue();
            handler2.Executed.ShouldBeTrue();
        }

        [Fact]
        public async Task ShouldReturnWhenAnyHandlerCompletes()
        {
            var cts1 = new TaskCompletionSource<string>();
            var cts2 = new TaskCompletionSource<string>();

            var handler1 = new RequestHandler(cts1.Task);
            var handler2 = new RequestHandler(cts2.Task);

            var container = new Container(x =>
            {
                x.Add<IRequestHandler<Request, string>>().Instance(handler1);
                x.Add<IRequestHandler<Request, string>>().Instance(handler2);
            });

            var mediator = new Mediator(container.GetService);

            var task = mediator.Send(new Request());

            task.Status.ShouldNotBe(TaskStatus.RanToCompletion);

            cts1.SetResult("1");

            (await task).ShouldBe("1");

            cts1 = new TaskCompletionSource<string>();
            cts2 = new TaskCompletionSource<string>();

            handler1 = new RequestHandler(cts1.Task);
            handler2 = new RequestHandler(cts2.Task);

            container = new Container(x =>
            {
                x.Add<IRequestHandler<Request, string>>().Instance(handler1);
                x.Add<IRequestHandler<Request, string>>().Instance(handler2);
            });

            mediator = new Mediator(container.GetService);

            task = mediator.Send(new Request());

            task.Status.ShouldNotBe(TaskStatus.RanToCompletion);

            cts2.SetResult("2");

            (await task).ShouldBe("2");
        }

        [Fact]
        public async Task ShouldReturnSuccessEvenIfSomeHandlersFail()
        {
            var cts1 = new TaskCompletionSource<string>();
            var cts2 = new TaskCompletionSource<string>();

            var handler1 = new RequestHandler(cts1.Task);
            var handler2 = new RequestHandler(cts2.Task);

            var container = new Container(x =>
            {
                x.Add<IRequestHandler<Request, string>>().Instance(handler1);
                x.Add<IRequestHandler<Request, string>>().Instance(handler2);
            });

            var mediator = new Mediator(container.GetService);

            var task = mediator.Send(new Request());

            cts1.SetException(new Exception());
            cts2.SetResult("success");

            (await task).ShouldBe("success");
        }

        [Fact]
        public async Task OnceTheFirstHandlerCompletesOtherHandlersShouldGetACancellationSignal()
        {
            var cts1 = new TaskCompletionSource<string>();
            var cts2 = new TaskCompletionSource<string>();

            var handler1 = new RequestHandler(cts1.Task);
            var handler2 = new RequestHandler(cts2.Task);

            var container = new Container(x =>
            {
                x.Add<IRequestHandler<Request, string>>().Instance(handler1);
                x.Add<IRequestHandler<Request, string>>().Instance(handler2);
            });

            var mediator = new Mediator(container.GetService);

            mediator.Send(new Request());

            cts1.SetResult("success");

            await Task.Delay(10);

            handler1.Cancelled.ShouldBeFalse();
            handler2.Cancelled.ShouldBeTrue();
        }

        [FanOut]
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
