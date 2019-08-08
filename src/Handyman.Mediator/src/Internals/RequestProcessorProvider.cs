using System;
using System.Collections.Concurrent;

namespace Handyman.Mediator.Internals
{
    internal static class RequestProcessorProvider
    {
        private static readonly ConcurrentDictionary<Type, object> Cache = new ConcurrentDictionary<Type, object>();

        internal static RequestPipeline<TResponse> GetRequestProcessor<TResponse>(IRequest<TResponse> request)
        {
            return (RequestPipeline<TResponse>)Cache.GetOrAdd(request.GetType(), CreateRequestProcessor<TResponse>);
        }

        private static object CreateRequestProcessor<TResponse>(Type requestType)
        {
            var responseType = typeof(TResponse);
            var requestProcessorType = typeof(RequestPipeline<,>).MakeGenericType(requestType, responseType);
            return Activator.CreateInstance(requestProcessorType);
        }
    }
}