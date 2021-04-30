using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Handyman.Mediator.Pipeline.RequestHandlerExperiment
{
    [AttributeUsage(AttributeTargets.Class)]
    public class RequestHandlerExperimentAttribute : RequestPipelineBuilderAttribute
    {
        private readonly Type _baselineHandlerType;

        public RequestHandlerExperimentAttribute(Type baselineHandlerType)
        {
            _baselineHandlerType = baselineHandlerType ?? throw new ArgumentNullException(nameof(baselineHandlerType));
        }

        public Type[]? ExperimentalHandlerTypes { get; set; }
        public string? Name { get; set; }
        public string[]? Tags { get; set; }

        public override async Task Execute<TRequest, TResponse>(RequestPipelineBuilderContext<TRequest, TResponse> pipelineBuilderContext, RequestContext<TRequest> requestContext)
        {
            var metadata = new RequestHandlerExperimentMetadata
            {
                BaselineHandlerType = _baselineHandlerType,
                ExperimentalHandlerTypes = ExperimentalHandlerTypes ?? Type.EmptyTypes,
                Name = Name,
                Tags = Tags
            };

            var handlers = pipelineBuilderContext.Handlers;

            if (handlers.Count <= 1)
                return;

            var serviceProvider = requestContext.ServiceProvider;

            var toggle = serviceProvider.GetRequiredService<IRequestHandlerExperimentToggle>();
            var isEnabled = await toggle.IsEnabled<TRequest, TResponse>(metadata, requestContext).WithGloballyConfiguredAwait();

            var experimentalHandlerTypes = GetExperimentalHandlerTypes(handlers, metadata);

            if (isEnabled == false)
            {
                handlers.RemoveAll(x => experimentalHandlerTypes.Contains(x.GetType()));
                return;
            }

            var baselineHandler = GetBaselineHandler(handlers, metadata);
            var experimentalHandlers = handlers.Where(x => experimentalHandlerTypes.Contains(x.GetType())).ToList();
            var observers = serviceProvider.GetServices<IRequestHandlerExperimentObserver>().ToListOptimized();

            handlers.Clear();

            handlers.Add(new RequestHandlerExperimentHandler<TRequest, TResponse>(baselineHandler, experimentalHandlers, observers));
        }

        private IRequestHandler<TRequest, TResponse> GetBaselineHandler<TRequest, TResponse>(List<IRequestHandler<TRequest, TResponse>> handlers, RequestHandlerExperimentMetadata metadata) where TRequest : IRequest<TResponse>
        {
            return handlers.Single(x => x.GetType() == metadata.BaselineHandlerType);
        }

        private Type[] GetExperimentalHandlerTypes<TRequest, TResponse>(List<IRequestHandler<TRequest, TResponse>> handlers, RequestHandlerExperimentMetadata metadata) where TRequest : IRequest<TResponse>
        {
            if (metadata.ExperimentalHandlerTypes.Length != 0)
                return metadata.ExperimentalHandlerTypes;

            return handlers.Select(x => x.GetType())
                .Where(x => x != metadata.BaselineHandlerType)
                .ToArray();
        }
    }
}