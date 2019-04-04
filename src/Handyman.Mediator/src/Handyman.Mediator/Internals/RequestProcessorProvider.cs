using System;
using System.Collections.Concurrent;

namespace Handyman.Mediator.Internals
{
    internal static class RequestProcessorProvider
    {
        private static readonly ConcurrentDictionary<Type, object> Cache = new ConcurrentDictionary<Type, object>();

        internal static RequestProcessor<TResponse> GetRequestProcessor<TResponse>(IRequest<TResponse> request)
        {
            return (RequestProcessor<TResponse>)Cache.GetOrAdd(request.GetType(), CreateRequestProcessor<TResponse>);
        }

        private static object CreateRequestProcessor<TResponse>(Type requestType)
        {
            var responseType = typeof(TResponse);
            var requestProcessorType = typeof(RequestProcessor<,>).MakeGenericType(requestType, responseType);
            return Activator.CreateInstance(requestProcessorType);
        }
    }
}