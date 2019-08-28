using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Handyman.Mediator;

namespace ExperimentRequestHandler
{
    public class ExperimentAttribute : RequestHandlerProviderAttribute
    {
        private readonly Type _primaryHandlerType;

        public ExperimentAttribute(Type primaryHandlerType)
        {
            _primaryHandlerType = primaryHandlerType;
        }

        public override IRequestHandler<TRequest, TResponse> GetHandler<TRequest, TResponse>(ServiceProvider serviceProvider)
        {
            var handlers = (IEnumerable<IRequestHandler<TRequest, TResponse>>)serviceProvider(typeof(IEnumerable<IRequestHandler<TRequest, TResponse>>));
            return new ExperimentHandler<TRequest, TResponse>(_primaryHandlerType, handlers);
        }

        private class ExperimentHandler<TRequest, TResponse> : IRequestHandler<TRequest, TResponse> where TRequest : IRequest<TResponse>
        {
            private readonly Type _primaryHandlerType;
            private readonly IEnumerable<IRequestHandler<TRequest, TResponse>> _handlers;

            public ExperimentHandler(Type primaryHandlerType, IEnumerable<IRequestHandler<TRequest, TResponse>> handlers)
            {
                _primaryHandlerType = primaryHandlerType;
                _handlers = handlers.ToList();
            }

            public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken)
            {
                var results = _handlers
                    .Select(x => new
                    {
                        HandlerType = x.GetType(),
                        Task = x.Handle(request, cancellationToken)
                    })
                    .ToList();

                await Task.WhenAll(results.Select(x => x.Task));

                var response = results.First(x => x.HandlerType == _primaryHandlerType).Task.Result;

                Console.WriteLine($" - {_primaryHandlerType.Name} {response}");

                var color = Console.ForegroundColor;
                foreach (var result in results.Where(x => x.HandlerType != _primaryHandlerType))
                {
                    var experimentResponse = result.Task.Result;

                    if (!EqualityComparer<TResponse>.Default.Equals(response, experimentResponse))
                        Console.ForegroundColor = ConsoleColor.Red;

                    Console.WriteLine($" - {result.HandlerType.Name}: {experimentResponse}");

                    Console.ForegroundColor = color;
                }

                return response;
            }
        }
    }
}