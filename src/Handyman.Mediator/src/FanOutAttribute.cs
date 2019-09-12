using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Handyman.Mediator.Internals;

namespace Handyman.Mediator
{
    [AttributeUsage(AttributeTargets.Class)]
    public class FanOutAttribute : RequestHandlerProviderAttribute
    {
        public override IRequestHandler<TRequest, TResponse> GetHandler<TRequest, TResponse>(ServiceProvider serviceProvider)
        {
            var handlers = serviceProvider.GetServices<IRequestHandler<TRequest, TResponse>>().ToListOptimized();

            if (handlers.Count == 0)
                throw new InvalidOperationException("Request handlers not found.");

            return new RequestHandler<TRequest, TResponse>(handlers);
        }

        private class RequestHandler<TRequest, TResponse> : IRequestHandler<TRequest, TResponse> where TRequest : IRequest<TResponse>
        {
            private readonly IEnumerable<IRequestHandler<TRequest, TResponse>> _handlers;

            public RequestHandler(IEnumerable<IRequestHandler<TRequest, TResponse>> handlers)
            {
                _handlers = handlers.ToListOptimized();
            }

            public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken)
            {
                var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                var tasks = _handlers.Select(x => x.Handle(request, cts.Token)).ToList();
                var task = await Task.WhenAny(tasks).ConfigureAwait(false);
                var response = await task.ConfigureAwait(false);
                cts.Cancel();
                return response;
            }
        }
    }
}