using Handyman.Mediator.RequestPipelineCustomization;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Handyman.Mediator.Internals
{
    internal class RequestHandlerExperimentExecutionStrategy : IRequestHandlerExecutionStrategy
    {
        private readonly RequestHandlerExperimentMetaData _experimentMetaData;

        public RequestHandlerExperimentExecutionStrategy(RequestHandlerExperimentMetaData experimentMetaData)
        {
            _experimentMetaData = experimentMetaData;
        }

        public async Task<TResponse> Execute<TRequest, TResponse>(List<IRequestHandler<TRequest, TResponse>> handlers, RequestPipelineContext<TRequest> context)
            where TRequest : IRequest<TResponse>
        {
            if (handlers.Count <= 1)
            {
                return await DefaultRequestHandlerExecutionStrategy.Instance.Execute(handlers, context).ConfigureAwait();
            }

            var baselineHandler = GetBaselineHandler(handlers);

            var toggle = context.ServiceProvider.GetRequiredService<IRequestHandlerExperimentToggle>();

            var request = context.Request;
            var cancellationToken = context.CancellationToken;

            if (!await toggle.IsEnabled<TRequest, TResponse>(_experimentMetaData, context).ConfigureAwait())
            {
                return await baselineHandler.Handle(request, cancellationToken).ConfigureAwait();
            }

            var observer = context.ServiceProvider.GetRequiredService<IRequestHandlerExperimentObserver>();
            var tasks = handlers.Select(handler => Execute(handler, request, cancellationToken)).ToListOptimized();
            var executions = await Task.WhenAll(tasks).ConfigureAwait();
            var baselineExecution = executions.Single(x => x.Handler == baselineHandler);
            var experimentExecutions = executions.Where(x => x != baselineExecution).ToList();

            var experiment = new RequestHandlerExperiment<TRequest, TResponse>
            {
                BaselineExecution = baselineExecution,
                CancellationToken = cancellationToken,
                ExperimentalExecutions = experimentExecutions,
                Request = request
            };

            await observer.Observe(experiment).ConfigureAwait();

            return await baselineExecution.Task.ConfigureAwait();
        }

        private IRequestHandler<TRequest, TResponse> GetBaselineHandler<TRequest, TResponse>(List<IRequestHandler<TRequest, TResponse>> handlers)
            where TRequest : IRequest<TResponse>
        {
            IRequestHandler<TRequest, TResponse> baselineHandler = null;

            foreach (var handler in handlers)
            {
                if (handler.GetType() != _experimentMetaData.BaselineHandlerType)
                    continue;

                if (baselineHandler != null)
                    throw new InvalidOperationException($"{typeof(TRequest).FullName} experiment has multiple baseline handlers.");

                baselineHandler = handler;
            }

            if (baselineHandler == null)
                throw new InvalidOperationException($"{typeof(TRequest).FullName} experiment does not have a baseline handler.");

            return baselineHandler;
        }

        private static async Task<RequestHandlerExperimentExecution<TRequest, TResponse>> Execute<TRequest, TResponse>(IRequestHandler<TRequest, TResponse> handler, TRequest request, CancellationToken cancellationToken)
            where TRequest : IRequest<TResponse>
        {
            // if the the code in handler.Handle(...) throws before doing any async/await the task will not be returned, hence the outer try/catch

            var stopwatch = Stopwatch.StartNew();

            try
            {
                var task = handler.Handle(request, cancellationToken);

                try
                {
                    await task.ConfigureAwait();
                }
                catch
                {
                    ; // intentionally empty
                }

                return new RequestHandlerExperimentExecution<TRequest, TResponse>
                {
                    Duration = stopwatch.Elapsed,
                    Handler = handler,
                    Task = task
                };
            }
            catch (Exception exception)
            {
                return new RequestHandlerExperimentExecution<TRequest, TResponse>
                {
                    Duration = stopwatch.Elapsed,
                    Handler = handler,
                    Task = Task.FromException<TResponse>(exception)
                };
            }
        }
    }
}