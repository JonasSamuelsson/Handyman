using System;
using System.Collections.Concurrent;

namespace Handyman.Mediator.Internals
{
    internal static class RequestPipelineFactory
    {
        private static readonly ConcurrentDictionary<Type, object> Cache = new ConcurrentDictionary<Type, object>();

        internal static RequestPipeline<TResponse> GetRequestPipeline<TResponse>(IRequest<TResponse> request)
        {
            return (RequestPipeline<TResponse>)Cache.GetOrAdd(request.GetType(), CreateRequestPipeline<TResponse>);
        }

        private static object CreateRequestPipeline<TResponse>(Type requestType)
        {
            var responseType = typeof(TResponse);
            var requestProcessorType = typeof(RequestPipeline<,>).MakeGenericType(requestType, responseType);
            return Activator.CreateInstance(requestProcessorType);
        }
    }
}