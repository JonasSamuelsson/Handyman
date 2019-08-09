using Maestro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Handyman.Mediator.Samples.WhenAnyRequestHandler
{
    public class Program
    {
        public static async Task Main()
        {
            var container = new Container(x =>
            {
                x.Add<IRequestHandler<Request, string>>().Factory(() => new RequestHandler("foo"));
                x.Add<IRequestHandler<Request, string>>().Factory(() => new RequestHandler("bar"));
            });

            var config = new Configuration
            {
                RequestHandlerProvider = new RequestHandlerProvider()
            };

            var mediator = new Mediator(container.GetService, config);

            while (true)
            {
                Console.Write("press enter to execute");
                Console.ReadLine();
                Console.WriteLine(await mediator.Send(new Request()));
                Console.WriteLine();
            }
        }
    }

    public class RequestHandlerProvider : IRequestHandlerProvider
    {
        public IRequestHandler<TRequest, TResponse> GetHandler<TRequest, TResponse>(ServiceProvider serviceProvider) where TRequest : IRequest<TResponse>
        {
            if (typeof(TRequest).GetCustomAttributes<WhenAnyAttribute>().Any())
            {
                var handlers = GetService<IEnumerable<IRequestHandler<TRequest, TResponse>>>(serviceProvider);
                return new WhenAnyHandler<TRequest, TResponse>(handlers);
            }

            return GetService<IRequestHandler<TRequest, TResponse>>(serviceProvider);
        }

        private static T GetService<T>(ServiceProvider serviceProvider)
        {
            return (T)serviceProvider.Invoke(typeof(T));
        }
    }

    public class WhenAnyHandler<TRequest, TResponse> : IRequestHandler<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        private readonly IEnumerable<IRequestHandler<TRequest, TResponse>> _handlers;

        public WhenAnyHandler(IEnumerable<IRequestHandler<TRequest, TResponse>> handlers)
        {
            _handlers = handlers;
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken)
        {
            // note that this isn't production ready as it doesn't do things like cancel the other handlers once the first one has completed etc
            return await await Task.WhenAny(_handlers.Select(x => x.Handle(request, cancellationToken)));
        }
    }

    public class WhenAnyAttribute : Attribute { }

    [WhenAny]
    public class Request : IRequest<string> { }

    public class RequestHandler : IRequestHandler<Request, string>
    {
        private static readonly Random Random = new Random();

        private readonly string _name;

        public RequestHandler(string name)
        {
            _name = name;
        }

        public async Task<string> Handle(Request request, CancellationToken cancellationToken)
        {
            var delay = Random.Next(100);
            await Task.Delay(delay, cancellationToken);
            return $"handled by {_name} in {delay} ms";
        }
    }
}
