using Handyman.Mediator;
using Maestro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Mediator.CustomRequestProcessing
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            var container = new Container(x =>
            {
                x.Add<IMediator>().Factory(ctx => new Handyman.Mediator.Mediator(ctx.Container.GetService,
                    new Configuration
                    {
                        RequestHandlerProvider = new RequestHandlerProvider()
                    }));
                x.Add<IRequestHandler<Request, string>>().Type<FixedDurationRequestHandler>();
                x.Add<IRequestHandler<Request, string>>().Type<RandomDurationRequestHandler>().Singleton();
            });

            var mediator = container.GetService<IMediator>();

            while (true)
            {
                Console.Write("press enter to execute");
                Console.ReadLine();
                Console.WriteLine(await mediator.Send(new Request()));
                Console.WriteLine();
            }
        }

        private class Request : IRequest<string> { }

        private class FixedDurationRequestHandler : IRequestHandler<Request, string>
        {
            public async Task<string> Handle(Request request, CancellationToken cancellationToken)
            {
                var delay = 50;
                await Task.Delay(delay, cancellationToken);
                return $"fixed:{delay}";
            }
        }

        private class RandomDurationRequestHandler : IRequestHandler<Request, string>
        {
            private static readonly Random Random = new Random();

            public async Task<string> Handle(Request request, CancellationToken cancellationToken)
            {
                var delay = Random.Next(100);
                await Task.Delay(delay, cancellationToken);
                return $"random:{delay}";
            }
        }

        private class RequestHandlerProvider : IRequestHandlerProvider
        {
            public IRequestHandler<TRequest, TResponse> GetHandler<TRequest, TResponse>(ServiceProvider serviceProvider)
                where TRequest : IRequest<TResponse>
            {
                var type = typeof(IEnumerable<IRequestHandler<TRequest, TResponse>>);
                var handlers = (IEnumerable<IRequestHandler<TRequest, TResponse>>)serviceProvider.Invoke(type);
                return new WhenAnyRequestHandler<TRequest, TResponse>(handlers);
            }
        }

        private class WhenAnyRequestHandler<TRequest, TResponse> : IRequestHandler<TRequest, TResponse>
            where TRequest : IRequest<TResponse>
        {
            private readonly IEnumerable<IRequestHandler<TRequest, TResponse>> _handlers;

            public WhenAnyRequestHandler(IEnumerable<IRequestHandler<TRequest, TResponse>> handlers)
            {
                _handlers = handlers;
            }

            public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken)
            {
                var tasks = _handlers.Select(handler => handler.Handle(request, cancellationToken)).ToArray();
                return await (await Task.WhenAny(tasks));
            }
        }
    }
}
